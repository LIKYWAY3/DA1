using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Refund;
using ASPtestShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations
{
    /// <summary>
    /// UC-20: Yêu cầu hoàn tiền (Refund Service Implementation)
    /// Xử lý nghiệp vụ yêu cầu hoàn tiền từ phía khách hàng và phê duyệt từ phía Quản trị viên.
    /// </summary>
    public class RefundService : IRefundService
    {
        private readonly AppDbContext _context;

        public RefundService(AppDbContext context)
        {
            _context = context;
        }

        // =======================================================================
        // Khách hàng: Gửi yêu cầu hoàn tiền cho đơn đã thanh toán
        // =======================================================================
        public async Task<RefundResultDto> RequestRefundAsync(string userId, CreateRefundRequestDto dto)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == dto.OrderId && o.UserId == userId);

            if (order == null)
            {
                return new RefundResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng hoặc đơn hàng không thuộc về bạn."
                };
            }

            // Chỉ cho phép yêu cầu hoàn tiền nếu đơn hàng ĐÃ THANH TOÁN
            if (!string.Equals(order.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase))
            {
                return new RefundResultDto
                {
                    Success = false,
                    Message = $"Chỉ có thể yêu cầu hoàn tiền cho đơn hàng đã thanh toán (Paid). Trạng thái hiện tại: '{order.PaymentStatus}'."
                };
            }

            // Kiểm tra xem đơn hàng đã có yêu cầu hoàn tiền nào đang chờ duyệt hoặc đã duyệt chưa
            var existingRequest = await _context.RefundRequests
                .FirstOrDefaultAsync(r => r.OrderId == order.OrderId && (r.Status == "Pending" || r.Status == "Approved"));

            if (existingRequest != null)
            {
                return new RefundResultDto
                {
                    Success = false,
                    Message = $"Đơn hàng này đã có yêu cầu hoàn tiền ở trạng thái '{existingRequest.Status}'!"
                };
            }

            // Nếu khách hàng không nhập tài khoản ngân hàng, tìm tài khoản mặc định của họ trong BankAccounts
            var bankName = dto.BankName;
            var accountNumber = dto.AccountNumber;
            var accountHolderName = dto.AccountHolderName;

            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                var defaultBank = await _context.BankAccounts
                    .FirstOrDefaultAsync(b => b.UserId == userId);

                if (defaultBank != null)
                {
                    bankName = bankName ?? defaultBank.BankName;
                    accountNumber = defaultBank.AccountNumber;
                    accountHolderName = accountHolderName ?? defaultBank.AccountName;
                }
            }

            var refundRequest = new RefundRequest
            {
                OrderId           = order.OrderId,
                UserId            = userId,
                Amount            = order.FinalAmount,
                Reason            = dto.Reason,
                Status            = "Pending",
                BankName          = bankName,
                AccountNumber     = accountNumber,
                AccountHolderName = accountHolderName,
                CreatedAt         = DateTime.UtcNow
            };

            _context.RefundRequests.Add(refundRequest);

            // Cập nhật trạng thái thanh toán của Order sang RefundPending
            order.PaymentStatus = "RefundPending";

            await _context.SaveChangesAsync();

            return new RefundResultDto
            {
                Success         = true,
                Message         = "Gửi yêu cầu hoàn tiền thành công! Yêu cầu của bạn đang chờ quản trị viên xét duyệt.",
                RefundRequestId = refundRequest.RefundRequestId,
                Status          = refundRequest.Status
            };
        }

        // =======================================================================
        // Khách hàng: Lấy danh sách yêu cầu hoàn tiền của mình
        // =======================================================================
        public async Task<List<RefundRequestDto>> GetMyRefundRequestsAsync(string userId)
        {
            return await _context.RefundRequests
                .Include(r => r.Order)
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new RefundRequestDto
                {
                    RefundRequestId   = r.RefundRequestId,
                    OrderId           = r.OrderId,
                    OrderCode         = r.Order.OrderCode,
                    UserId            = r.UserId,
                    UserName          = r.User.UserName ?? "",
                    Amount            = r.Amount,
                    Reason            = r.Reason,
                    Status            = r.Status,
                    BankName          = r.BankName,
                    AccountNumber     = r.AccountNumber,
                    AccountHolderName = r.AccountHolderName,
                    AdminNote         = r.AdminNote,
                    CreatedAt         = r.CreatedAt,
                    ProcessedAt       = r.ProcessedAt,
                    ProcessedBy       = r.ProcessedBy
                })
                .ToListAsync();
        }

        // =======================================================================
        // Lấy yêu cầu hoàn tiền theo OrderId
        // =======================================================================
        public async Task<RefundRequestDto?> GetRefundRequestByOrderIdAsync(int orderId, string userId)
        {
            var r = await _context.RefundRequests
                .Include(x => x.Order)
                .Include(x => x.User)
                .Where(x => x.OrderId == orderId && (string.IsNullOrEmpty(userId) || x.UserId == userId))
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (r == null) return null;

            return new RefundRequestDto
            {
                RefundRequestId   = r.RefundRequestId,
                OrderId           = r.OrderId,
                OrderCode         = r.Order.OrderCode,
                UserId            = r.UserId,
                UserName          = r.User.UserName ?? "",
                Amount            = r.Amount,
                Reason            = r.Reason,
                Status            = r.Status,
                BankName          = r.BankName,
                AccountNumber     = r.AccountNumber,
                AccountHolderName = r.AccountHolderName,
                AdminNote         = r.AdminNote,
                CreatedAt         = r.CreatedAt,
                ProcessedAt       = r.ProcessedAt,
                ProcessedBy       = r.ProcessedBy
            };
        }

        // =======================================================================
        // Admin: Lấy toàn bộ danh sách yêu cầu hoàn tiền
        // =======================================================================
        public async Task<List<RefundRequestDto>> GetAllRefundRequestsAsync()
        {
            return await _context.RefundRequests
                .Include(r => r.Order)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new RefundRequestDto
                {
                    RefundRequestId   = r.RefundRequestId,
                    OrderId           = r.OrderId,
                    OrderCode         = r.Order.OrderCode,
                    UserId            = r.UserId,
                    UserName          = r.User.UserName ?? "",
                    Amount            = r.Amount,
                    Reason            = r.Reason,
                    Status            = r.Status,
                    BankName          = r.BankName,
                    AccountNumber     = r.AccountNumber,
                    AccountHolderName = r.AccountHolderName,
                    AdminNote         = r.AdminNote,
                    CreatedAt         = r.CreatedAt,
                    ProcessedAt       = r.ProcessedAt,
                    ProcessedBy       = r.ProcessedBy
                })
                .ToListAsync();
        }

        // =======================================================================
        // Admin: Phê duyệt hoặc từ chối yêu cầu hoàn tiền
        // =======================================================================
        public async Task<RefundResultDto> ProcessRefundAsync(int refundRequestId, string adminUserId, ProcessRefundDto dto)
        {
            var request = await _context.RefundRequests
                .Include(r => r.Order)
                    .ThenInclude(o => o.OrderItems)
                .FirstOrDefaultAsync(r => r.RefundRequestId == refundRequestId);

            if (request == null)
            {
                return new RefundResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy yêu cầu hoàn tiền."
                };
            }

            if (request.Status != "Pending")
            {
                return new RefundResultDto
                {
                    Success = false,
                    Message = $"Yêu cầu hoàn tiền này đã được xử lý trước đó với kết quả '{request.Status}'."
                };
            }

            request.ProcessedAt = DateTime.UtcNow;
            request.ProcessedBy = adminUserId;
            request.AdminNote   = dto.AdminNote;
            request.Status      = dto.Status;

            if (dto.Status == "Approved")
            {
                // Cập nhật thanh toán của đơn thành Refunded
                request.Order.PaymentStatus = "Refunded";

                // Nếu đơn hàng chưa Completed / chưa Cancelled, tự động hủy đơn và hoàn lại kho
                if (request.Order.OrderStatus != "Completed" && request.Order.OrderStatus != "Cancelled")
                {
                    request.Order.OrderStatus = "Cancelled";
                    request.Order.Note = string.IsNullOrWhiteSpace(request.Order.Note)
                        ? "Đơn hàng đã được duyệt hoàn tiền và hủy"
                        : $"{request.Order.Note} | Đã duyệt hoàn tiền và hủy";

                    foreach (var item in request.Order.OrderItems)
                    {
                        await _context.Database.ExecuteSqlInterpolatedAsync(
                            $"UPDATE Products SET StockQuantity = StockQuantity + {item.Quantity} WHERE ProductId = {item.ProductId}"
                        );
                    }

                    // Hoàn lại lượt dùng Coupon nếu đơn hàng có áp dụng
                    if (request.Order.CouponId.HasValue)
                    {
                        await _context.Database.ExecuteSqlInterpolatedAsync(
                            $"UPDATE Coupons SET UsedCount = CASE WHEN UsedCount > 0 THEN UsedCount - 1 ELSE 0 END WHERE CouponId = {request.Order.CouponId.Value}"
                        );
                    }
                }
            }
            else // Rejected
            {
                // Trả lại trạng thái Paid cho Order
                request.Order.PaymentStatus = "Paid";
            }

            await _context.SaveChangesAsync();

            return new RefundResultDto
            {
                Success         = true,
                Message         = dto.Status == "Approved" ? "Đã phê duyệt hoàn tiền thành công!" : "Đã từ chối yêu cầu hoàn tiền.",
                RefundRequestId = request.RefundRequestId,
                Status          = request.Status
            };
        }
    }
}
