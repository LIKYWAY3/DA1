using System.Collections.Generic;

namespace ASPtestShop.Models.DTO.Order
{
    public class BatchOrderActionResultDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public int SuccessCount { get; set; }

        public int FailedCount { get; set; }

        public List<string> ConfirmedOrderCodes { get; set; } = new List<string>();
    }
}
