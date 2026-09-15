using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPtestShop.Data.Entities
{
    public class Coupon : BaseEntity
    {
        public int CouponId { get; set; }

        public ICollection<Order> Orders { get; set; }
        = new List<Order>();

        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(20)]
        public string DiscountType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinOrderAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxDiscountAmount { get; set; }

        public int UsageLimitTotal { get; set; } = 100;

        public int UsageLimitPerCustomer { get; set; } = 1;

        public int UsedCount { get; set; } = 0;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}