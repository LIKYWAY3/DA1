using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Product;
using ASPtestShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ASPtestShop.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public ProductService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // =====================================================
        // LẤY TẤT CẢ SẢN PHẨM ĐANG HOẠT ĐỘNG
        // GET: /api/products
        // Dùng cho trang Shop
        // =====================================================
        public async Task<List<ProductListItemDto>> GetProductsAsync()
        {
            const string cacheKey = "Cache_AllActiveProducts";
            if (_cache.TryGetValue(cacheKey, out List<ProductListItemDto>? cached) && cached != null)
            {
                return cached;
            }

            var products = await _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new ProductListItemDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,

                    Price = p.SalePrice ?? p.Price,
                    OriginalPrice = p.Price,

                    ThumbnailUrl = p.ThumbnailUrl,

                    ImageUrl = p.ProductImages
                        .OrderByDescending(img => img.IsPrimary)
                        .ThenBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault(),

                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.CategoryName : null,
                    StockQuantity = p.StockQuantity,
                    CreatedAt = p.CreatedAt,
                    IsFeatured = p.IsFeatured
                })
                .ToListAsync();

            _cache.Set(cacheKey, products, TimeSpan.FromMinutes(3));
            return products;
        }

        // =====================================================
        // LẤY SẢN PHẨM THEO DANH MỤC
        // Bao gồm danh mục đang chọn và toàn bộ danh mục con
        // GET: /api/products/category/{categoryId}
        // =====================================================
        public async Task<List<ProductListItemDto>>
            GetProductsByCategoryAsync(int categoryId)
        {
            if (categoryId <= 0)
            {
                return new List<ProductListItemDto>();
            }

            var allActiveCategories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .Select(c => new
                {
                    c.CategoryId,
                    c.ParentCategoryId
                })
                .ToListAsync();

            var selectedCategoryExists = allActiveCategories
                .Any(c => c.CategoryId == categoryId);

            if (!selectedCategoryExists)
            {
                return new List<ProductListItemDto>();
            }

            var categoryIds = new HashSet<int>
            {
                categoryId
            };

            var queue = new Queue<int>();
            queue.Enqueue(categoryId);

            while (queue.Count > 0)
            {
                var currentCategoryId = queue.Dequeue();

                var childCategoryIds = allActiveCategories
                    .Where(c =>
                        c.ParentCategoryId == currentCategoryId)
                    .Select(c => c.CategoryId)
                    .ToList();

                foreach (var childCategoryId in childCategoryIds)
                {
                    if (categoryIds.Add(childCategoryId))
                    {
                        queue.Enqueue(childCategoryId);
                    }
                }
            }

            return await _context.Products
                .AsNoTracking()
                .Where(p =>
                    p.IsActive
                    && categoryIds.Contains(p.CategoryId))
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new ProductListItemDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,

                    Price = p.SalePrice ?? p.Price,
                    OriginalPrice = p.Price,

                    ThumbnailUrl = p.ThumbnailUrl,

                    ImageUrl = p.ProductImages
                        .OrderByDescending(img => img.IsPrimary)
                        .ThenBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault(),

                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.CategoryName : null,
                    StockQuantity = p.StockQuantity,
                    CreatedAt = p.CreatedAt,
                    IsFeatured = p.IsFeatured
                })
                .ToListAsync();
        }

        // =====================================================
        // LẤY CHI TIẾT SẢN PHẨM
        // GET: /api/products/{id}
        // =====================================================
        public async Task<ProductDetailDto?>
            GetProductByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return await _context.Products
                .AsNoTracking()
                .Where(p =>
                    p.ProductId == id
                    && p.IsActive)
                .Select(p => new ProductDetailDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.Description,

                    Price = p.SalePrice ?? p.Price,
                    OriginalPrice = p.Price,

                    StockQuantity = p.StockQuantity,
                    ThumbnailUrl = p.ThumbnailUrl,

                    Images = p.ProductImages
                        .OrderByDescending(img => img.IsPrimary)
                        .ThenBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        // =====================================================
        // LẤY SẢN PHẨM NỔI BẬT
        // GET: /api/products/featured
        // Dùng cho trang Home
        // =====================================================
        public async Task<List<ProductListItemDto>>
            GetFeaturedProductsAsync()
        {
            const string cacheKey = "Cache_FeaturedProducts";
            if (_cache.TryGetValue(cacheKey, out List<ProductListItemDto>? cached) && cached != null)
            {
                return cached;
            }

            var featured = await _context.Products
                .AsNoTracking()
                .Where(p =>
                    p.IsActive
                    && p.IsFeatured)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new ProductListItemDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,

                    Price = p.SalePrice ?? p.Price,
                    OriginalPrice = p.Price,

                    ThumbnailUrl = p.ThumbnailUrl,

                    ImageUrl = p.ProductImages
                        .OrderByDescending(img => img.IsPrimary)
                        .ThenBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault(),

                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.CategoryName : null,
                    StockQuantity = p.StockQuantity,
                    CreatedAt = p.CreatedAt,
                    IsFeatured = p.IsFeatured
                })
                .ToListAsync();

            _cache.Set(cacheKey, featured, TimeSpan.FromMinutes(5));
            return featured;
        }

        // =====================================================
        // TÌM KIẾM SẢN PHẨM (toàn bộ shop)
        // GET: /api/products/search?keyword=...
        // =====================================================
        public async Task<List<ProductListItemDto>>
            SearchProductsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new List<ProductListItemDto>();
            }

            keyword = keyword.Trim();

            return await _context.Products
                .AsNoTracking()
                .Where(p =>
                    p.IsActive
                    && p.ProductName.Contains(keyword))
                .OrderBy(p => p.ProductName)
                .Select(p => new ProductListItemDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,

                    Price = p.SalePrice ?? p.Price,
                    OriginalPrice = p.Price,

                    ThumbnailUrl = p.ThumbnailUrl,

                    ImageUrl = p.ProductImages
                        .OrderByDescending(img => img.IsPrimary)
                        .ThenBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault(),

                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.CategoryName : null,
                    StockQuantity = p.StockQuantity,
                    CreatedAt = p.CreatedAt,
                    IsFeatured = p.IsFeatured
                })
                .ToListAsync();
        }

        // =====================================================
        // TÌM KIẾM SẢN PHẨM TRONG DANH MỤC CỤ THỂ
        // GET: /api/products/search?keyword=...&categoryId=...
        // Bao gồm danh mục đang chọn và toàn bộ danh mục con
        // =====================================================
        public async Task<List<ProductListItemDto>>
            SearchProductsInCategoryAsync(string keyword, int categoryId)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new List<ProductListItemDto>();
            }

            if (categoryId <= 0)
            {
                return new List<ProductListItemDto>();
            }

            keyword = keyword.Trim();

            // Thu thập tất cả categoryId (bao gồm danh mục con) bằng BFS
            var allActiveCategories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .Select(c => new
                {
                    c.CategoryId,
                    c.ParentCategoryId
                })
                .ToListAsync();

            var selectedCategoryExists = allActiveCategories
                .Any(c => c.CategoryId == categoryId);

            if (!selectedCategoryExists)
            {
                return new List<ProductListItemDto>();
            }

            var categoryIds = new HashSet<int> { categoryId };
            var queue = new Queue<int>();
            queue.Enqueue(categoryId);

            while (queue.Count > 0)
            {
                var currentCategoryId = queue.Dequeue();

                var childCategoryIds = allActiveCategories
                    .Where(c => c.ParentCategoryId == currentCategoryId)
                    .Select(c => c.CategoryId)
                    .ToList();

                foreach (var childCategoryId in childCategoryIds)
                {
                    if (categoryIds.Add(childCategoryId))
                    {
                        queue.Enqueue(childCategoryId);
                    }
                }
            }

            return await _context.Products
                .AsNoTracking()
                .Where(p =>
                    p.IsActive
                    && categoryIds.Contains(p.CategoryId)
                    && p.ProductName.Contains(keyword))
                .OrderBy(p => p.ProductName)
                .Select(p => new ProductListItemDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,

                    Price = p.SalePrice ?? p.Price,
                    OriginalPrice = p.Price,

                    ThumbnailUrl = p.ThumbnailUrl,

                    ImageUrl = p.ProductImages
                        .OrderByDescending(img => img.IsPrimary)
                        .ThenBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault(),

                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.CategoryName : null,
                    StockQuantity = p.StockQuantity,
                    CreatedAt = p.CreatedAt,
                    IsFeatured = p.IsFeatured
                })
                .ToListAsync();
        }
    }
}