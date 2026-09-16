using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Admin;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations.Admin
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminDashboardService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<AdminDashboardDto> GetDashboardStatsAsync()
        {
            var now = DateTime.Now;
            var today = now.Date;
            var sevenDaysAgo = today.AddDays(-6);
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

            // Các đơn được tính doanh thu là đơn đã thanh toán hoặc đã giao hoàn tất
            var validOrdersQuery = _context.Orders.AsNoTracking()
                .Where(o => o.PaymentStatus == "Paid" || o.OrderStatus == "Delivered" || o.OrderStatus == "Completed");

            var totalRevenue = await validOrdersQuery
                .SumAsync(o => (decimal?)o.FinalAmount) ?? 0m;

            var monthRevenue = await validOrdersQuery
                .Where(o => o.CreatedAt >= firstDayOfMonth)
                .SumAsync(o => (decimal?)o.FinalAmount) ?? 0m;

            var todayRevenue = await validOrdersQuery
                .Where(o => o.CreatedAt >= today)
                .SumAsync(o => (decimal?)o.FinalAmount) ?? 0m;

            var totalOrders = await _context.Orders.CountAsync();
            var pendingOrders = await _context.Orders.CountAsync(o => o.OrderStatus == "Pending");
            var completedOrders = await _context.Orders.CountAsync(o => o.OrderStatus == "Completed" || o.OrderStatus == "Delivered");
            var totalProducts = await _context.Products.CountAsync(p => p.IsActive);
            var totalCustomers = await _userManager.Users.CountAsync();

            var recentOrders = await _context.Orders.AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .Select(o => new AdminRecentOrderDto
                {
                    OrderId = o.OrderId,
                    OrderCode = o.OrderCode,
                    ReceiverName = o.ReceiverName,
                    FinalAmount = o.FinalAmount,
                    OrderStatus = o.OrderStatus,
                    PaymentStatus = o.PaymentStatus,
                    PaymentMethod = o.PaymentMethod,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            // Lấy dữ liệu 7 ngày gần nhất để vẽ biểu đồ doanh thu
            var recent7DaysOrders = await validOrdersQuery
                .Where(o => o.CreatedAt >= sevenDaysAgo)
                .Select(o => new { o.CreatedAt, o.FinalAmount })
                .ToListAsync();

            var dailyRevenues = new List<AdminDailyRevenueDto>();
            for (int i = 0; i < 7; i++)
            {
                var day = sevenDaysAgo.AddDays(i);
                var dayOrders = recent7DaysOrders.Where(o => o.CreatedAt.Date == day.Date).ToList();
                dailyRevenues.Add(new AdminDailyRevenueDto
                {
                    Date = day.ToString("dd/MM"),
                    Revenue = dayOrders.Sum(o => o.FinalAmount),
                    OrderCount = dayOrders.Count
                });
            }

            return new AdminDashboardDto
            {
                TotalRevenue = totalRevenue,
                MonthRevenue = monthRevenue,
                TodayRevenue = todayRevenue,
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders,
                TotalProducts = totalProducts,
                TotalCustomers = totalCustomers,
                RecentOrders = recentOrders,
                DailyRevenues = dailyRevenues
            };
        }
    }
}
