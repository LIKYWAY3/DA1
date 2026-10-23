using ASPtestShop.Hubs;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ASPtestShop.Services.Implementations
{
    public class OrderNotificationService : IOrderNotificationService
    {
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly ILogger<OrderNotificationService> _logger;

        public OrderNotificationService(IHubContext<OrderHub> hubContext, ILogger<OrderNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyAdminNewOrderAsync(
            string orderCode,
            string receiverName,
            decimal finalAmount,
            string paymentMethod,
            string paymentStatus)
        {
            try
            {
                var payload = new
                {
                    orderCode = orderCode,
                    receiverName = receiverName,
                    finalAmount = finalAmount,
                    paymentMethod = paymentMethod,
                    paymentStatus = paymentStatus,
                    createdAt = DateTime.UtcNow.AddHours(7).ToString("HH:mm dd/MM/yyyy")
                };

                await _hubContext.Clients.Group(OrderHub.AdminGroup).SendAsync("NewOrderReceived", payload);
                _logger.LogInformation("SignalR: Broadcasted new order notification for {OrderCode} to AdminGroup", orderCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR: Failed to broadcast new order notification for {OrderCode}", orderCode);
            }
        }
    }
}
