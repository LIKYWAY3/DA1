namespace ASPtestShop.Models.DTO.Order
{
    public class AutoCancelResultDto
    {
        public bool Success { get; set; } = true;
        public int CancelledCount { get; set; }
        public List<string> CancelledOrderCodes { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
