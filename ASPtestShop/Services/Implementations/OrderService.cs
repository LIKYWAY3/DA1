using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Order;
using ASPtestShop.Models.DTO.Payment;
using ASPtestShop.Services.Interfaces;
using ASPtestShop.Services.PaymentProviders;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IPaymentProviderFactory _paymentProviderFactory;

        public OrderService(
            AppDbContext context,
            IPaymentProviderFactory paymentProviderFactory)
        {
            _context = context;
            _paymentProviderFactory = paymentProviderFactory;
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
                                OrderId = order.OrderId,
                                OrderCode = order.OrderCode,
                                UserId = userId,
                                Amount = order.FinalAmount,
                                PaymentMethod = paymentMethod
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
                        Success = true,
                        Message = "Đặt hàng thành công",
                        OrderId = order.OrderId,
                        OrderCode = order.OrderCode,
                        TotalAmount = order.TotalAmount,
                        DiscountAmount = order.DiscountAmount,
                        ShippingFee = order.ShippingFee,
                        FinalAmount = order.FinalAmount,
                        OrderStatus = order.OrderStatus,
                        PaymentStatus = order.PaymentStatus,
                        PaymentMethod = order.PaymentMethod
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
                                    oi.LineTotal
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

        public async Task<CancelOrderResultDto> CancelOrderAsync(
            string userId,
            int orderId,
            string? cancelReason = null)
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

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await _context.Database.BeginTransactionAsync();

                try
                {
                    // Hoàn lại số lượng tồn kho nguyên tử cho từng sản phẩm
                    foreach (var item in order.OrderItems)
                    {
                        await _context.Database.ExecuteSqlInterpolatedAsync(
                            $"UPDATE Products SET StockQuantity = StockQuantity + {item.Quantity} WHERE ProductId = {item.ProductId}"
                        );
                    }

                    // Hoàn lại lượt dùng Coupon nếu đơn hàng có áp dụng
                    if (order.CouponId.HasValue)
                    {
                        await _context.Database.ExecuteSqlInterpolatedAsync(
                            $"UPDATE Coupons SET UsedCount = CASE WHEN UsedCount > 0 THEN UsedCount - 1 ELSE 0 END WHERE CouponId = {order.CouponId.Value}"
                        );
                    }

                    order.OrderStatus = "Cancelled";
                    if (!string.IsNullOrWhiteSpace(cancelReason))
                    {
                        order.Note = string.IsNullOrWhiteSpace(order.Note)
                            ? $"Lý do hủy: {cancelReason}"
                            : $"{order.Note} | Lý do hủy: {cancelReason}";
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new CancelOrderResultDto
                    {
                        Success = true,
                        Message = "Hủy đơn hàng và hoàn lại số lượng tồn kho thành công!",
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
    }
}