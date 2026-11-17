using System.Collections.Generic;

namespace ASPtestShop.Models.DTO.Admin
{
    public class BatchCouponRequestDto
    {
        public List<int> CouponIds { get; set; } = new();
        public bool? IsActive { get; set; }
    }

    public class BatchCouponActionResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
    }
}
