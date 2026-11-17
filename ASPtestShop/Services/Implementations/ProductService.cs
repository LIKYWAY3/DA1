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
        // HỖ TRỢ XÓA DẤU TIẾNG VIỆT ĐỂ TÌM KIẾM KHÔNG DẤU & CÓ DẤU
        // =====================================================
        private static string RemoveVietnameseDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            string normalized = text.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();
            foreach (char c in normalized)
            {
                var uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC).Replace('đ', 'd').Replace('Đ', 'D');
        }

        // =====================================================
        // TÌM KIẾM SẢN PHẨM THÔNG MINH (toàn bộ shop)
        // Hỗ trợ: Không dấu, có dấu, đa từ khóa tách rời, tìm theo tên & danh mục
        // GET: /api/products/search?keyword=...
        // =====================================================
        public async Task<List<ProductListItemDto>> SearchProductsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new List<ProductListItemDto>();
            }

            keyword = keyword.Trim();
            var allProducts = await GetProductsAsync();

            var tokens = keyword
                .Split(new[] { ' ', ',', '-', '+', '.', '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim().ToLower())
                .Where(t => t.Length > 0)
                .Distinct()
                .ToList();

            if (tokens.Count == 0) return new List<ProductListItemDto>();

            var normTokens = tokens.Select(t => RemoveVietnameseDiacritics(t).ToLower()).ToList();
            var rawKw = keyword.ToLower();
            var normKw = RemoveVietnameseDiacritics(rawKw).ToLower();

            var matched = allProducts.Where(p =>
            {
                var rawName = (p.ProductName ?? "").ToLower();
                var normName = RemoveVietnameseDiacritics(rawName).ToLower();

                var rawCat = (p.CategoryName ?? "").ToLower();
                var normCat = RemoveVietnameseDiacritics(rawCat).ToLower();

                var rawCombined = $"{rawName} {rawCat}";
                var normCombined = $"{normName} {normCat}";

                for (int i = 0; i < tokens.Count; i++)
                {
                    var tok = tokens[i];
                    var normTok = normTokens[i];

                    if (!rawCombined.Contains(tok) && !normCombined.Contains(normTok))
                    {
                        return false;
                    }
                }
                return true;
            })
            .OrderByDescending(p =>
            {
                var rawName = (p.ProductName ?? "").ToLower();
                var normName = RemoveVietnameseDiacritics(rawName).ToLower();

                if (rawName == rawKw || normName == normKw) return 100;
                if (rawName.StartsWith(rawKw) || normName.StartsWith(normKw)) return 80;
                if (rawName.Contains(rawKw) || normName.Contains(normKw)) return 60;

                var rawCat = (p.CategoryName ?? "").ToLower();
                var normCat = RemoveVietnameseDiacritics(rawCat).ToLower();
                if (rawCat.Contains(rawKw) || normCat.Contains(normKw)) return 40;
                return 10;
            })
            .ThenBy(p => p.ProductName)
            .ToList();

            return matched;
        }

        // =====================================================
        // TÌM KIẾM SẢN PHẨM TRONG DANH MỤC CỤ THỂ
        // GET: /api/products/search?keyword=...&categoryId=...
        // =====================================================
        public async Task<List<ProductListItemDto>> SearchProductsInCategoryAsync(string keyword, int categoryId)
        {
            if (string.IsNullOrWhiteSpace(keyword) || categoryId <= 0)
            {
                return new List<ProductListItemDto>();
            }

            keyword = keyword.Trim();

            var allActiveCategories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .Select(c => new
                {
                    c.CategoryId,
                    c.ParentCategoryId
                })
                .ToListAsync();

            var selectedCategoryExists = allActiveCategories.Any(c => c.CategoryId == categoryId);
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

            var allCategoryProducts = (await GetProductsAsync())
                .Where(p => categoryIds.Contains(p.CategoryId))
                .ToList();

            var tokens = keyword
                .Split(new[] { ' ', ',', '-', '+', '.', '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim().ToLower())
                .Where(t => t.Length > 0)
                .Distinct()
                .ToList();

            if (tokens.Count == 0) return allCategoryProducts;

            var normTokens = tokens.Select(t => RemoveVietnameseDiacritics(t).ToLower()).ToList();
            var rawKw = keyword.ToLower();
            var normKw = RemoveVietnameseDiacritics(rawKw).ToLower();

            var matched = allCategoryProducts.Where(p =>
            {
                var rawName = (p.ProductName ?? "").ToLower();
                var normName = RemoveVietnameseDiacritics(rawName).ToLower();

                var rawCat = (p.CategoryName ?? "").ToLower();
                var normCat = RemoveVietnameseDiacritics(rawCat).ToLower();

                var rawCombined = $"{rawName} {rawCat}";
                var normCombined = $"{normName} {normCat}";

                for (int i = 0; i < tokens.Count; i++)
                {
                    var tok = tokens[i];
                    var normTok = normTokens[i];

                    if (!rawCombined.Contains(tok) && !normCombined.Contains(normTok))
                    {
                        return false;
                    }
                }
                return true;
            })
            .OrderByDescending(p =>
            {
                var rawName = (p.ProductName ?? "").ToLower();
                var normName = RemoveVietnameseDiacritics(rawName).ToLower();

                if (rawName == rawKw || normName == normKw) return 100;
                if (rawName.StartsWith(rawKw) || normName.StartsWith(normKw)) return 80;
                if (rawName.Contains(rawKw) || normName.Contains(normKw)) return 60;
                return 10;
            })
            .ThenBy(p => p.ProductName)
            .ToList();

            return matched;
        }
    }
}