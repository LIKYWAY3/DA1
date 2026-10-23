namespace ASPtestShop.Services.Interfaces
{
    public interface IOrderNotificationService
    {
        Task NotifyAdminNewOrderAsync(string orderCode, string receiverName, decimal finalAmount, string paymentMethod, string paymentStatus);
    }
}
