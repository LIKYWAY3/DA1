using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.DTO.Review
{
    public class CreateReviewDto
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm cần đánh giá")]
        public int ProductId { get; set; }

        [Range(1, 5, ErrorMessage = "Số sao đánh giá phải từ 1 đến 5")]
        public int Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "Nội dung nhận xét tối đa 1000 ký tự")]
        public string? Comment { get; set; }
    }

    public class ProductReviewsSummaryDto
    {
        public int ProductId { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> StarCounts { get; set; } = new();
        public List<ProductReviewItemDto> Reviews { get; set; } = new();
    }

    public class ProductReviewItemDto
    {
        public int ReviewId { get; set; }
        public string? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
