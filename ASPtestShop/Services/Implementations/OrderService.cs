using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Order;
using ASPtestShop.Models.DTO.Payment;
using ASPtestShop.Services.Interfaces;
using ASPtestShop.Services.PaymentProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ASPtestShop.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IPaymentProviderFactory _paymentProviderFactory;
        private readonly IConfiguration _config;

        public OrderService(
            AppDbContext context,
            IPaymentProviderFactory paymentProviderFactory,
            IConfiguration config)
        {
            _context = context;
            _paymentProviderFactory = paymentProviderFactory;
            _config = config;
        }

        public async Task<CheckoutResultDto> CheckoutAsync(
            string userId,
            CheckoutDto checkoutDto)
        {
            if (checkoutDto.CartItemIds == null
                || checkoutDto.CartItemIds.Count == 0)
            {
                return new CheckoutResultDto
                {
                    Success = false,
                    Message = "Vui lòng chọn ít nhất một sản phẩm để thanh toán"
                };
            }

            var requestedCartItemIds = checkoutDto.CartItemIds
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (requestedCartItemIds.Count == 0)
            {
                return new CheckoutResultDto
                {
                    Success = false,
                    Message = "Danh sách sản phẩm được chọn không hợp lệ"
                };
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return new CheckoutResultDto
                {
                    Success = false,
                    Message = "Giỏ hàng trống"
                };
            }

            var selectedCartItems = cart.CartItems
                .Where(item =>
                    requestedCartItemIds.Contains(item.CartItemId))
                .ToList();

            if (selectedCartItems.Count == 0)
            {
                return new CheckoutResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy sản phẩm được chọn trong giỏ hàng"
                };
            }

            if (selectedCartItems.Count != requestedCartItemIds.Count)
            {
                return new CheckoutResultDto
                {
                    Success = false,
                    Message = "Một số sản phẩm được chọn không còn tồn tại trong giỏ hàng. Vui lòng tải lại giỏ hàng"
                };
            }

            var paymentMethod =
                string.IsNullOrWhiteSpace(checkoutDto.PaymentMethod)
                    ? "COD"
                    : checkoutDto.PaymentMethod.Trim().ToUpperInvariant();

            IPaymentProvider provider;

            try
            {
                provider =
                    _paymentProviderFactory.GetProvider(paymentMethod);
            }
            catch (NotSupportedException ex)
            {
                return new CheckoutResultDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }

            var totalAmount = selectedCartItems.Sum(item =>
                item.Quantity
                * (item.Product.SalePrice ?? item.Product.Price)
            );

            var discountAmount = 0m;
            var shippingFee = 0m;
            Coupon? appliedCoupon = null;

            if (!string.IsNullOrWhiteSpace(checkoutDto.CouponCode))
            {
                var couponCode = checkoutDto.CouponCode.Trim().ToUpperInvariant();
                var now = DateTime.UtcNow;
                var coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Code == couponCode && c.IsActive);

                if (coupon == null || now < coupon.StartDate || now > coupon.EndDate)
                {
                    return new CheckoutResultDto
                    {
                        Success = false,
                        Message = "Mã giảm giá không hợp lệ hoặc đã hết hạn sử dụng!"
                    };
                }

                if (coupon.UsedCount >= coupon.UsageLimitTotal)
                {
                    return new CheckoutResultDto
                    {
                        Success = false,
                        Message = "Mã giảm giá đã hết lượt sử dụng!"
                    };
                }

                var userUsedCount = await _context.Orders
                    .CountAsync(o => o.UserId == userId && o.CouponId == coupon.CouponId && o.OrderStatus != "Cancelled");

                if (userUsedCount >= coupon.UsageLimitPerCustomer)
                {
                    return new CheckoutResultDto
                    {
                        Success = false,
                        Message = $"Bạn đã sử dụng hết lượt ({coupon.UsageLimitPerCustomer} lần) của mã giảm giá này!"
                    };
                }

                if (totalAmount < coupon.MinOrderAmount)
                {
                    return new CheckoutResultDto
                    {
                        Success = false,
                        Message = $"Đơn hàng tối thiểu phải từ {coupon.MinOrderAmount:N0}đ để áp dụng mã giảm giá này!"
                    };
                }

                appliedCoupon = coupon;
                if (string.Equals(coupon.DiscountType, "Percentage", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(coupon.DiscountType, "Percent", StringComparison.OrdinalIgnoreCase))
                {
                    discountAmount = totalAmount * (coupon.DiscountValue / 100m);
                    if (coupon.MaxDiscountAmount.HasValue && discountAmount > coupon.MaxDiscountAmount.Value)
                    {
                        discountAmount = coupon.MaxDiscountAmount.Value;
                    }
                }
                else
                {
                    discountAmount = coupon.DiscountValue;
                }

                if (discountAmount > totalAmount)
                {
                    discountAmount = totalAmount;
                }
            }

            var finalAmount =
                totalAmount - discountAmount + shippingFee;
            if (finalAmount < 0) finalAmount = 0;

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await _context.Database.BeginTransactionAsync();

                try
                {
                    // Chống bán vượt kho (Overselling) - Trừ tồn kho nguyên tử
                    foreach (var item in selectedCartItems)
                    {
                        var affected = await _context.Database.ExecuteSqlInterpolatedAsync(
                            $"UPDATE Products SET StockQuantity = StockQuantity - {item.Quantity} WHERE ProductId = {item.ProductId} AND StockQuantity >= {item.Quantity}"
                        );

                        if (affected == 0)
                        {
                            await transaction.RollbackAsync();
                            return new CheckoutResultDto
                            {
                                Success = false,
                                Message = $"Sản phẩm '{item.Product.ProductName}' không đủ số lượng trong kho!"
                            };
                        }
                    }

                    // Cập nhật tăng UsedCount của Coupon nguyên tử nếu có
                    if (appliedCoupon != null)
                    {
                        var couponAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                            $"UPDATE Coupons SET UsedCount = UsedCount + 1 WHERE CouponId = {appliedCoupon.CouponId} AND UsedCount < {appliedCoupon.UsageLimitTotal}"
                        );

                        if (couponAffected == 0)
                        {
                            await transaction.RollbackAsync();
                            return new CheckoutResultDto
                            {
                                Success = false,
                                Message = "Mã giảm giá vừa hết lượt sử dụng trong lúc xử lý!"
                            };
                        }
                    }

                    var order = new Order
                    {
                        OrderCode =
                            "OD"
                            + DateTime.Now.ToString("yyyyMMddHHmmssfff"),

                        UserId = userId,
                        CouponId = appliedCoupon?.CouponId,
                        CouponCodeSnapshot = appliedCoupon?.Code,

                        TotalAmount = totalAmount,
                        DiscountAmount = discountAmount,
                        ShippingFee = shippingFee,
                        FinalAmount = finalAmount,

                        OrderStatus = "Pending",
                        PaymentStatus = "Unpaid",
                        PaymentMethod = paymentMethod,

                        ReceiverName = checkoutDto.ReceiverName,
                        ReceiverPhone = checkoutDto.ReceiverPhone,
                        ShippingAddress = checkoutDto.ShippingAddress,
                        Note = checkoutDto.Note
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    var orderItems = selectedCartItems
                        .Select(item =>
                        {
                            var unitPrice =
                                item.Product.SalePrice
                                ?? item.Product.Price;

                            return new OrderItem
                            {
                                OrderId = order.OrderId,
                                ProductId = item.ProductId,

                                ProductNameSnapshot =
                                    item.Product.ProductName,

                                UnitPrice = unitPrice,
                                Quantity = item.Quantity,
                                LineTotal =
                                    item.Quantity * unitPrice
                            };
                        })
                        .ToList();

                    _context.OrderItems.AddRange(orderItems);

                    var paymentResult =
                        await provider.CreatePaymentAsync(
                            new CreatePaymentRequestDto
                            {
                                OrderId          = order.OrderId,
                                OrderCode        = order.OrderCode,
                                UserId           = userId,
                                Amount           = order.FinalAmount,
                                PaymentMethod    = paymentMethod,
                                ReturnUrl        = BuildReturnUrl(checkoutDto, paymentMethod),
                                IpAddress        = checkoutDto.IpAddress ?? "127.0.0.1",
                                OrderDescription = $"Thanh toan don hang {order.OrderCode}"
                            }
                        );

                    if (!paymentResult.IsSuccess)
                    {
                        await transaction.RollbackAsync();

                        return new CheckoutResultDto
                        {
                            Success = false,
                            Message = paymentResult.Message
                        };
                    }

                    var payment = new Payment
                    {
                        OrderId = order.OrderId,
                        PaymentMethod =
                            paymentResult.PaymentMethod,

                        PaymentStatus =
                            paymentResult.PaymentStatus,

                        TransactionCode =
                            paymentResult.TransactionCode,

                        PaidAt =
                            paymentResult.PaymentStatus == "Paid"
                                ? DateTime.Now
                                : null
                    };

                    _context.Payments.Add(payment);

                    order.PaymentMethod =
                        paymentResult.PaymentMethod;

                    order.PaymentStatus =
                        paymentResult.PaymentStatus;

                    // Chỉ xóa những sản phẩm đã được chọn và thanh toán.
                    _context.CartItems.RemoveRange(
                        selectedCartItems
                    );

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new CheckoutResultDto
                    {
                        Success          = true,
                        Message          = paymentResult.RequiresRedirect
                            ? $"Đơn hàng đã được tạo. Vui lòng thanh toán qua {paymentMethod}."
                            : "Đặt hàng thành công",
                        OrderId          = order.OrderId,
                        OrderCode        = order.OrderCode,
                        TotalAmount      = order.TotalAmount,
                        DiscountAmount   = order.DiscountAmount,
                        ShippingFee      = order.ShippingFee,
                        FinalAmount      = order.FinalAmount,
                        OrderStatus      = order.OrderStatus,
                        PaymentStatus    = order.PaymentStatus,
                        PaymentMethod    = order.PaymentMethod,
                        PaymentUrl       = paymentResult.PaymentUrl,
                        RequiresRedirect = paymentResult.RequiresRedirect
                    };
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<List<OrderHistoryDto>> GetOrderHistoryAsync(
            string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderHistoryDto
                {
                    OrderId = o.OrderId,
                    OrderCode = o.OrderCode,
                    TotalAmount = o.TotalAmount,
                    DiscountAmount = o.DiscountAmount,
                    ShippingFee = o.ShippingFee,
                    FinalAmount = o.FinalAmount,
                    OrderStatus = o.OrderStatus,
                    PaymentStatus = o.PaymentStatus,
                    PaymentMethod = o.PaymentMethod,
                    ReceiverName = o.ReceiverName,
                    ReceiverPhone = o.ReceiverPhone,
                    ShippingAddress = o.ShippingAddress,
                    Note = o.Note,
                    CreatedAt = o.CreatedAt,

                    Items = o.OrderItems
                        .Select(oi =>
                            new OrderHistoryItemDto
                            {
                                OrderItemId =
                                    oi.OrderItemId,

                                ProductId =
                                    oi.ProductId,

                                ProductNameSnapshot =
                                    oi.ProductNameSnapshot,

                                UnitPrice =
                                    oi.UnitPrice,

                                Quantity =
                                    oi.Quantity,

                                LineTotal =
                                    oi.LineTotal,

                                ImageUrl = oi.Product != null && oi.Product.ProductImages.Any()
                                    ? oi.Product.ProductImages.OrderByDescending(pi => pi.IsPrimary).Select(pi => pi.ImageUrl).FirstOrDefault()
                                    : null
                            })
                        .ToList()
                })
                .ToListAsync();

            return orders;
        }

        public async Task<OrderHistoryDto?> GetOrderDetailAsync(
            string userId,
            int orderId)
        {
            var order = await _context.Orders
                .Where(o =>
                    o.OrderId == orderId
                    && o.UserId == userId)
                .Select(o => new OrderHistoryDto
                {
                    OrderId = o.OrderId,
                    OrderCode = o.OrderCode,
                    TotalAmount = o.TotalAmount,
                    DiscountAmount = o.DiscountAmount,
                    ShippingFee = o.ShippingFee,
                    FinalAmount = o.FinalAmount,
                    OrderStatus = o.OrderStatus,
                    PaymentStatus = o.PaymentStatus,
                    PaymentMethod = o.PaymentMethod,
                    ReceiverName = o.ReceiverName,
                    ReceiverPhone = o.ReceiverPhone,
                    ShippingAddress = o.ShippingAddress,
                    Note = o.Note,
                    CreatedAt = o.CreatedAt,

                    Items = o.OrderItems
                        .Select(oi =>
                            new OrderHistoryItemDto
                            {
                                OrderItemId =
                                    oi.OrderItemId,

                                ProductId =
                                    oi.ProductId,

                                ProductNameSnapshot =
                                    oi.ProductNameSnapshot,

                                UnitPrice =
                                    oi.UnitPrice,

                                Quantity =
                                    oi.Quantity,

                                LineTotal =
                                    oi.LineTotal
                            })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return order;
        }

        private async Task RestockOrderItemsAndCouponAsync(Order order)
        {
            if (order.OrderItems != null && order.OrderItems.Any())
            {
                foreach (var item in order.OrderItems)
                {
                    if (_context.Database.IsRelational())
                    {
                        await _context.Database.ExecuteSqlInterpolatedAsync(
                            $"UPDATE Products SET StockQuantity = StockQuantity + {item.Quantity} WHERE ProductId = {item.ProductId}"
                        );
                    }
                    else
                    {
                        var product = await _context.Products.FindAsync(item.ProductId);
                        if (product != null)
                        {
                            product.StockQuantity += item.Quantity;
                        }
                    }
                }
            }

            if (order.CouponId.HasValue)
            {
                if (_context.Database.IsRelational())
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"UPDATE Coupons SET UsedCount = CASE WHEN UsedCount > 0 THEN UsedCount - 1 ELSE 0 END WHERE CouponId = {order.CouponId.Value}"
                    );
                }
                else
                {
                    var coupon = await _context.Coupons.FindAsync(order.CouponId.Value);
                    if (coupon != null && coupon.UsedCount > 0)
                    {
                        coupon.UsedCount -= 1;
                    }
                }
            }
        }

        public Task<CancelOrderResultDto> CancelOrderAsync(
            string userId,
            int orderId,
            string? cancelReason = null)
        {
            return CancelOrderAsync(userId, orderId, new CancelOrderRequestDto { Reason = cancelReason });
        }

        public async Task<CancelOrderResultDto> CancelOrderAsync(
            string userId,
            int orderId,
            CancelOrderRequestDto dto)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                return new CancelOrderResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng cần hủy!"
                };
            }

            if (order.OrderStatus != "Pending")
            {
                return new CancelOrderResultDto
                {
                    Success = false,
                    Message = $"Chỉ có thể hủy đơn hàng ở trạng thái 'Chờ xử lý' (Pending). Trạng thái hiện tại: '{order.OrderStatus}'"
                };
            }

            var cancelReason = dto?.Reason;
            var wasPaid = string.Equals(order.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase);

            if (_context.Database.IsRelational())
            {
                var strategy = _context.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        await RestockOrderItemsAndCouponAsync(order);

                        order.OrderStatus = "Cancelled";
                        if (!string.IsNullOrWhiteSpace(cancelReason))
                        {
                            order.Note = string.IsNullOrWhiteSpace(order.Note)
                                ? $"Lý do hủy: {cancelReason}"
                                : $"{order.Note} | Lý do hủy: {cancelReason}";
                        }

                        if (wasPaid)
                        {
                            order.PaymentStatus = "RefundPending";
                            var refundRequest = new RefundRequest
                            {
                                OrderId = order.OrderId,
                                UserId = userId,
                                Amount = order.FinalAmount,
                                Reason = string.IsNullOrWhiteSpace(cancelReason) ? "Khách hủy đơn đã thanh toán online" : cancelReason,
                                Status = "Pending",
                                BankName = dto?.BankName,
                                AccountNumber = dto?.AccountNumber,
                                AccountHolderName = dto?.AccountHolderName,
                                CreatedAt = DateTime.UtcNow
                            };
                            _context.RefundRequests.Add(refundRequest);
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return new CancelOrderResultDto
                        {
                            Success = true,
                            Message = wasPaid
                                ? "Hủy đơn hàng thành công! Đơn hàng đã được chuyển sang trạng thái chờ hoàn tiền, quản trị viên sẽ xử lý sớm nhất."
                                : "Hủy đơn hàng và hoàn lại số lượng tồn kho thành công!",
                            OrderId = order.OrderId,
                            OrderStatus = order.OrderStatus
                        };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return new CancelOrderResultDto
                        {
                            Success = false,
                            Message = "Đã xảy ra lỗi trong quá trình hủy đơn: " + ex.Message
                        };
                    }
                });
            }
            else
            {
                await RestockOrderItemsAndCouponAsync(order);

                order.OrderStatus = "Cancelled";
                if (!string.IsNullOrWhiteSpace(cancelReason))
                {
                    order.Note = string.IsNullOrWhiteSpace(order.Note)
                        ? $"Lý do hủy: {cancelReason}"
                        : $"{order.Note} | Lý do hủy: {cancelReason}";
                }

                if (wasPaid)
                {
                    order.PaymentStatus = "RefundPending";
                    var refundRequest = new RefundRequest
                    {
                        OrderId = order.OrderId,
                        UserId = userId,
                        Amount = order.FinalAmount,
                        Reason = string.IsNullOrWhiteSpace(cancelReason) ? "Khách hủy đơn đã thanh toán online" : cancelReason,
                        Status = "Pending",
                        BankName = dto?.BankName,
                        AccountNumber = dto?.AccountNumber,
                        AccountHolderName = dto?.AccountHolderName,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.RefundRequests.Add(refundRequest);
                }

                await _context.SaveChangesAsync();

                return new CancelOrderResultDto
                {
                    Success = true,
                    Message = wasPaid
                        ? "Hủy đơn hàng thành công! Đơn hàng đã được chuyển sang trạng thái chờ hoàn tiền, quản trị viên sẽ xử lý sớm nhất."
                        : "Hủy đơn hàng và hoàn lại số lượng tồn kho thành công!",
                    OrderId = order.OrderId,
                    OrderStatus = order.OrderStatus
                };
            }
        }

        public async Task<CancelOrderResultDto> RequestCancelOrderAsync(
            string userId,
            int orderId,
            string reason)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                return new CancelOrderResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng cần gửi yêu cầu hủy!"
                };
            }

            if (order.OrderStatus != "Confirmed")
            {
                return new CancelOrderResultDto
                {
                    Success = false,
                    Message = $"Chỉ có thể gửi yêu cầu hủy cho đơn hàng đã được xác nhận (Confirmed). Trạng thái hiện tại: '{order.OrderStatus}'"
                };
            }

            var requestTag = $"[Yêu cầu hủy]: {reason.Trim()}";
            if (!string.IsNullOrWhiteSpace(order.Note) && order.Note.Contains("[Yêu cầu hủy]"))
            {
                return new CancelOrderResultDto
                {
                    Success = false,
                    Message = "Đơn hàng này đã có yêu cầu hủy đang chờ xử lý từ trước!"
                };
            }

            order.Note = string.IsNullOrWhiteSpace(order.Note)
                ? requestTag
                : $"{order.Note} | {requestTag}";

            await _context.SaveChangesAsync();

            return new CancelOrderResultDto
            {
                Success = true,
                Message = "Gửi yêu cầu hủy đơn hàng thành công! Quản trị viên sẽ xem xét duyệt hủy cho bạn.",
                OrderId = order.OrderId,
                OrderStatus = order.OrderStatus
            };
        }

        // =======================================================================
        // UC-19: Tự động hủy đơn hàng quá hạn (SystemScheduler)
        // =======================================================================
        public async Task<AutoCancelResultDto> AutoCancelExpiredOrdersAsync(int timeoutHours = 24, int onlineTimeoutMinutes = 30)
        {
            var result = new AutoCancelResultDto();
            var now = DateTime.UtcNow;
            var generalDeadline = now.AddHours(-timeoutHours);
            var onlineDeadline = now.AddMinutes(-onlineTimeoutMinutes);

            // Tìm các đơn hàng thỏa mãn điều kiện quá hạn:
            // 1. Đơn Pending quá timeoutHours
            // 2. Đơn Pending chọn Online (VNPay/MoMo/ZaloPay) nhưng Unpaid quá onlineTimeoutMinutes
            var expiredOrders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.OrderStatus == "Pending" && (o.PaymentStatus == "Unpaid" || o.PaymentStatus == "Pending") &&
                    (
                        o.CreatedAt <= generalDeadline ||
                        (!string.IsNullOrEmpty(o.PaymentMethod) && o.PaymentMethod != "COD" && o.CreatedAt <= onlineDeadline)
                    )
                )
                .ToListAsync();

            if (!expiredOrders.Any())
            {
                result.Message = "Không có đơn hàng nào quá hạn cần hủy.";
                return result;
            }

            foreach (var order in expiredOrders)
            {
                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        await RestockOrderItemsAndCouponAsync(order);

                        order.OrderStatus = "Cancelled";
                        var cancelNote = "Hệ thống tự động hủy do quá hạn thanh toán";
                        order.Note = string.IsNullOrWhiteSpace(order.Note)
                            ? cancelNote
                            : $"{order.Note} | {cancelNote}";

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        result.CancelledCount++;
                        result.CancelledOrderCodes.Add(order.OrderCode);
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                    }
                });
            }

            result.Message = $"Đã tự động hủy {result.CancelledCount} đơn hàng quá hạn thanh toán.";
            return result;
        }

        // ── Private helpers ─────────────────────────────────────────────────────

        /// <summary>
        /// Xây dựng ReturnUrl đầy đủ cho cổng thanh toán dựa trên PaymentMethod.
        /// Ưu tiên BaseUrl từ CheckoutDto (client gửi lên), fallback về config.
        /// </summary>
        private string BuildReturnUrl(CheckoutDto checkoutDto, string paymentMethod)
        {
            var baseUrl = checkoutDto.BaseUrl?.TrimEnd('/') ?? string.Empty;

            var relativeReturnUrl = paymentMethod.ToUpperInvariant() switch
            {
                "VNPAY"   => _config["VnPay:ReturnUrl"]   ?? "/checkout/vnpay-return",
                "MOMO"    => _config["MoMo:ReturnUrl"]    ?? "/checkout/momo-return",
                "ZALOPAY" => _config["ZaloPay:ReturnUrl"] ?? "/checkout/zalopay-return",
                _         => "/checkout/success"
            };

            // Nếu relativeReturnUrl đã là URL tuyệt đối thì dùng luôn
            if (relativeReturnUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return relativeReturnUrl;

            return $"{baseUrl}{relativeReturnUrl}";
        }
    }
}