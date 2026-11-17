using System.Collections.Generic;

namespace ASPtestShop.Models.DTO.Admin
{
    public class BatchReviewRequestDto
    {
        public List<int> ReviewIds { get; set; } = new();
    }

    public class BatchReviewActionResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
    }
}
