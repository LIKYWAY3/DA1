using ASPtestShop.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ASPtestShop.Services.BackgroundServices
{
    /// <summary>
    /// UC-19: Tự động hủy đơn quá hạn – SystemScheduler Background Service
    /// Quét định kỳ các đơn hàng Pending/Unpaid quá hạn thời gian quy định
    /// và tự động hoàn trả số lượng tồn kho + coupon lượt dùng.
    /// </summary>
    public class OrderTimeoutBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<OrderTimeoutBackgroundService> _logger;

        public OrderTimeoutBackgroundService(
            IServiceScopeFactory scopeFactory,
            IConfiguration config,
            ILogger<OrderTimeoutBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _config = config;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OrderTimeoutBackgroundService (UC-19) đã khởi động.");

            // Đọc cấu hình chu kỳ quét (mặc định 5 phút)
            var intervalMinutes = _config.GetValue<int>("OrderTimeoutSettings:CheckIntervalMinutes", 5);
            if (intervalMinutes <= 0) intervalMinutes = 5;

            var timeoutHours = _config.GetValue<int>("OrderTimeoutSettings:TimeoutHours", 24);
            var onlineTimeoutMinutes = _config.GetValue<int>("OrderTimeoutSettings:OnlinePaymentTimeoutMinutes", 30);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug("OrderTimeoutBackgroundService: Bắt đầu quét đơn hàng quá hạn...");

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                        var result = await orderService.AutoCancelExpiredOrdersAsync(timeoutHours, onlineTimeoutMinutes);

                        if (result.CancelledCount > 0)
                        {
                            _logger.LogWarning("OrderTimeoutBackgroundService: {Message} Các mã đơn: {OrderCodes}",
                                result.Message,
                                string.Join(", ", result.CancelledOrderCodes));
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi xảy ra trong quá trình chạy OrderTimeoutBackgroundService.");
                }

                // Chờ cho chu kỳ tiếp theo
                await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
            }

            _logger.LogInformation("OrderTimeoutBackgroundService (UC-19) đã dừng.");
        }
    }
}
