namespace ASPtestShop.Models.DTO.Order
{
    public class CancelOrderResultDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public int? OrderId { get; set; }

        public string? OrderStatus { get; set; }
    }
}
