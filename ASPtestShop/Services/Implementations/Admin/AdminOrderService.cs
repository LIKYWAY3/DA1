using ASPtestShop.Data;
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
            "Paid",
            "Failed"
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

            // Nếu đơn hàng chuyển sang Cancelled từ trạng thái khác -> Hoàn lại tồn kho & lượt dùng Coupon
            if (dto.OrderStatus == "Cancelled" && order.OrderStatus != "Cancelled")
            {
                foreach (var item in order.OrderItems)
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"UPDATE Products SET StockQuantity = StockQuantity + {item.Quantity} WHERE ProductId = {item.ProductId}"
                    );
                }

                if (order.CouponId.HasValue)
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"UPDATE Coupons SET UsedCount = CASE WHEN UsedCount > 0 THEN UsedCount - 1 ELSE 0 END WHERE CouponId = {order.CouponId.Value}"
                    );
                }
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
    }
}