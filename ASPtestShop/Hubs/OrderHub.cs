using Microsoft.AspNetCore.SignalR;

namespace ASPtestShop.Hubs
{
    public class OrderHub : Hub
    {
        public const string AdminGroup = "Admins";

        /// <summary>
        /// Admin client gọi hàm này sau khi kết nối để nhận thông báo đơn hàng
        /// </summary>
        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, AdminGroup);
        }

        /// <summary>
        /// Admin rời khỏi nhóm
        /// </summary>
        public async Task LeaveAdminGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, AdminGroup);
        }
    }
}
