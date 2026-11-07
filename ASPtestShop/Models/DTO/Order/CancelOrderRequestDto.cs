namespace ASPtestShop.Models.DTO.Order
{
    public class CancelOrderRequestDto
    {
        public string? Reason { get; set; }

        public string? BankName { get; set; }

        public string? AccountNumber { get; set; }

        public string? AccountHolderName { get; set; }
    }
}
