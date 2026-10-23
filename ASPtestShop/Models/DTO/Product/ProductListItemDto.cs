namespace ASPtestShop.Models.DTO.Product
{
    // DTO dùng cho danh sách sản phẩm:
    // GET /api/products
    // GET /api/products/category/{categoryId}
    // GET /api/products/featured
    // GET /api/products/search?keyword=abc
    public class ProductListItemDto
    {
        // Mã sản phẩm
        public int ProductId { get; set; }

        // Tên sản phẩm
        public string ProductName { get; set; } = string.Empty;

        // Giá bán thực tế
        // Nếu có SalePrice thì lấy SalePrice
        // Nếu không có SalePrice thì lấy Price gốc
        public decimal Price { get; set; }

        // Giá gốc ban đầu
        public decimal OriginalPrice { get; set; }

        // Ảnh đại diện lưu trực tiếp trong Product
        public string? ThumbnailUrl { get; set; }

        // Ảnh đầu tiên trong bảng ProductImages, sắp xếp theo SortOrder
        public string? ImageUrl { get; set; }

        // Mã danh mục
        public int CategoryId { get; set; }

        // Tên danh mục
        public string? CategoryName { get; set; }

        // Số lượng tồn kho
        public int StockQuantity { get; set; }

        // Thời gian tạo
        public DateTime CreatedAt { get; set; }

        // Sản phẩm nổi bật
        public bool IsFeatured { get; set; }
    }
}
