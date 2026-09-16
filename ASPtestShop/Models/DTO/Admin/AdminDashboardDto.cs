namespace ASPtestShop.Models.DTO.Admin
{
    public class AdminDashboardDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }
        public List<AdminRecentOrderDto> RecentOrders { get; set; } = new();
        public List<AdminDailyRevenueDto> DailyRevenues { get; set; } = new();
    }

    public class AdminRecentOrderDto
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public decimal FinalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class AdminDailyRevenueDto
    {
        public string Date { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }
}
