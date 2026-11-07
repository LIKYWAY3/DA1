using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Order;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations.Admin
{
    public class AdminOrderService : IAdminOrderService
    {
        private readonly AppDbContext _context;

        private readonly string[] _validOrderStatuses =
        {
            "Pending",
            "Confirmed",
            "Shipped",
            "Completed",
            "Cancelled"
        };

        private readonly string[] _validPaymentStatuses =
        {
            "Unpaid",
            "Pending",
            "Paid",
            "Failed",
            "RefundPending",
            "Refunded"
        };

        public AdminOrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminOrderListItemDto>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new AdminOrderListItemDto
                {
                    OrderId = o.OrderId,
                    OrderCode = o.OrderCode,
                    UserId = o.UserId,
                    FinalAmount = o.FinalAmount,
                    OrderStatus = o.OrderStatus,
                    PaymentStatus = o.PaymentStatus,
                    PaymentMethod = o.PaymentMethod,
                    ReceiverName = o.ReceiverName,
                    ReceiverPhone = o.ReceiverPhone,
                    Note = o.Note,
                    HasCancelRequest = o.Note != null && o.Note.Contains("[Yêu cầu hủy]") && o.OrderStatus != "Cancelled",
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<AdminOrderDetailDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return null;
            }

            return new AdminOrderDetailDto
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                FinalAmount = order.FinalAmount,
                OrderStatus = order.OrderStatus,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod,
                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                ShippingAddress = order.ShippingAddress,
                Note = order.Note,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(item => new AdminOrderItemDto
                {
                    OrderItemId = item.OrderItemId,
                    ProductId = item.ProductId,
                    ProductNameSnapshot = item.ProductNameSnapshot,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    LineTotal = item.LineTotal
                }).ToList()
            };
        }

        public async Task<AdminOrderActionResultDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
        {
            if (!_validOrderStatuses.Contains(dto.OrderStatus))
            {
                return new AdminOrderActionResultDto
                {
                    Success = false,
                    Message = "Trạng thái đơn hàng không hợp lệ"
                };
            }

            if (!_validPaymentStatuses.Contains(dto.PaymentStatus))
            {
                return new AdminOrderActionResultDto
                {
                    Success = false,
                    Message = "Trạng thái thanh toán không hợp lệ"
                };
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return new AdminOrderActionResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng"
                };
            }

            // RÀNG BUỘC STATE MACHINE: Ngăn chặn thao túng trạng thái kết thúc (Terminal States)
            if (order.OrderStatus == "Cancelled" && dto.OrderStatus != "Cancelled")
            {
                return new AdminOrderActionResultDto
                {
                    Success = false,
                    Message = "Đơn hàng đã hủy là trạng thái kết thúc (terminal state), không thể mở lại. Vui lòng tạo đơn hàng mới."
                };
            }

            if (order.OrderStatus == "Completed" && dto.OrderStatus != "Completed")
            {
                return new AdminOrderActionResultDto
                {
                    Success = false,
                    Message = "Đơn hàng đã hoàn tất không thể chuyển về trạng thái đang xử lý."
                };
            }

            // Nếu đơn hàng chuyển sang Cancelled từ trạng thái khác -> Hoàn lại tồn kho & lượt dùng Coupon
            if (dto.OrderStatus == "Cancelled" && order.OrderStatus != "Cancelled")
            {
                await RestockOrderItemsAndCouponAsync(order);
            }

            order.OrderStatus = dto.OrderStatus;
            order.PaymentStatus = dto.PaymentStatus;

            await _context.SaveChangesAsync();

            return new AdminOrderActionResultDto
            {
                Success = true,
                Message = "Cập nhật trạng thái đơn hàng thành công"
            };
        }

        public async Task<AdminOrderActionResultDto> DeleteCancelledOrderAsync(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                return new AdminOrderActionResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng cần xóa!"
                };
            }

            if (order.OrderStatus != "Cancelled")
            {
                return new AdminOrderActionResultDto
                {
                    Success = false,
                    Message = "Chỉ được phép xóa các đơn hàng có trạng thái Đã hủy (Cancelled)! Các đơn hàng khác phải được giữ lại để đảm bảo dữ liệu doanh thu và thống kê."
                };
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return new AdminOrderActionResultDto
                {
                    Success = true,
                    Message = $"Đã xóa thành công đơn hàng {order.OrderCode}!"
                };
            });
        }

        public async Task<AdminOrderActionResultDto> CleanupAllCancelledOrdersAsync()
        {
            var cancelledOrders = await _context.Orders
                .Where(o => o.OrderStatus == "Cancelled")
                .ToListAsync();

            if (cancelledOrders.Count == 0)
            {
                return new AdminOrderActionResultDto
                {
                    Success = true,
                    Message = "Hiện tại không có đơn hàng nào bị hủy cần dọn dẹp."
                };
            }

            var count = cancelledOrders.Count;
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                _context.Orders.RemoveRange(cancelledOrders);
                await _context.SaveChangesAsync();
                return new AdminOrderActionResultDto
                {
                    Success = true,
                    Message = $"Đã dọn dẹp thành công {count} đơn hàng đã hủy!"
                };
            });
        }
        public async Task<AdminOrderActionResultDto> ApproveCancelOrderAsync(int orderId, string? adminNote = null)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return new AdminOrderActionResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng cần duyệt hủy."
                };
            }

            if (order.OrderStatus == "Cancelled")
            {
                return new AdminOrderActionResultDto
                {
                    Success = false,
                    Message = "Đơn hàng này đã ở trạng thái Đã hủy trước đó."
                };
            }

            if (order.OrderStatus == "Completed")
            {
                return new AdminOrderActionResultDto
                {
                    Success = false,
                    Message = "Đơn hàng đã hoàn tất không thể hủy."
                };
            }

            await RestockOrderItemsAndCouponAsync(order);

            order.OrderStatus = "Cancelled";
            var note = string.IsNullOrWhiteSpace(adminNote) ? "Admin đã phê duyệt hủy đơn" : $"Admin đã phê duyệt hủy đơn: {adminNote}";
            order.Note = string.IsNullOrWhiteSpace(order.Note) ? note : $"{order.Note} | {note}";

            if (string.Equals(order.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase))
            {
                order.PaymentStatus = "RefundPending";
            }

            await _context.SaveChangesAsync();

            return new AdminOrderActionResultDto
            {
                Success = true,
                Message = "Đã phê duyệt hủy đơn hàng và hoàn lại số lượng tồn kho thành công."
            };
        }

        public async Task<AdminOrderActionResultDto> RejectCancelOrderAsync(int orderId, string reason)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                return new AdminOrderActionResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng."
                };
            }

            var rejectNote = $"[Từ chối hủy: {reason.Trim()}]";
            order.Note = string.IsNullOrWhiteSpace(order.Note) ? rejectNote : $"{order.Note} | {rejectNote}";

            await _context.SaveChangesAsync();

            return new AdminOrderActionResultDto
            {
                Success = true,
                Message = "Đã từ chối yêu cầu hủy đơn hàng."
            };
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
    }
}