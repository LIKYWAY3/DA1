using System.Collections.Generic;

namespace ASPtestShop.Models.DTO.Refund
{
    public class BatchProcessRefundRequestDto
    {
        public List<int> RefundRequestIds { get; set; } = new();
        public string Status { get; set; } = "Approved"; // Approved, Rejected
        public string? TransactionCode { get; set; }
        public string? AdminNote { get; set; }
    }

    public class BatchRefundActionResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
    }
}
