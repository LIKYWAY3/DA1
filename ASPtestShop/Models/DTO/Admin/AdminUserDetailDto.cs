namespace ASPtestShop.Models.DTO.Admin
{
    public class AdminUserDetailDto
    {
        // Thông tin cơ bản
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string? Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PrimaryAddress { get; set; }

        // Bảo mật & Khóa
        public bool IsLockedOut { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }

        // Thống kê
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalReviews { get; set; }

        // Sổ địa chỉ
        public List<AdminUserAddressItemDto> Addresses { get; set; } = new();

        // Lịch sử đơn hàng
        public List<AdminUserOrderSummaryDto> RecentOrders { get; set; } = new();
    }

    public class AdminUserAddressItemDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string SpecificAddress { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }

    public class AdminUserOrderSummaryDto
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public string? CouponCode { get; set; }
    }
}
