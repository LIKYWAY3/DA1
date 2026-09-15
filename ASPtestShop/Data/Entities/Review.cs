using System.ComponentModel.DataAnnotations;
using ASPtestShop.Data;
namespace ASPtestShop.Data.Entities
{
    public class Review : BaseEntity
    {
        public int ReviewId { get; set; }

        public int ProductId { get; set; }

        public string? UserId { get; set; }

        public int? OrderItemId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public bool IsApproved { get; set; } = false;

        public Product Product { get; set; } = null!;

        public ApplicationUser? User { get; set; }

        public OrderItem? OrderItem { get; set; }
    }
}