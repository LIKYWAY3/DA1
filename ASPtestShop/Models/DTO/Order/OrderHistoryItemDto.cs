namespace ASPtestShop.Models.DTO.Order
{
    public class OrderHistoryItemDto
    {
        public int OrderItemId { get; set; }

        public int ProductId { get; set; }

        public string ProductNameSnapshot { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal LineTotal { get; set; }

        public string? ImageUrl { get; set; }
    }
}