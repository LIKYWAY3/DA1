namespace ASPtestShop.Models.DTO.Order
{
    public class AdminOrderListItemDto
    {
        public int OrderId { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public decimal FinalAmount { get; set; }

        public string OrderStatus { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public string ReceiverName { get; set; } = string.Empty;

        public string ReceiverPhone { get; set; } = string.Empty;

        public string? Note { get; set; }

        public bool HasCancelRequest { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
