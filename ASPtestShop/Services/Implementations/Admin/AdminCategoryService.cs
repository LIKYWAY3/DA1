using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Category;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ASPtestShop.Services.Implementations.Admin
{
    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly AppDbContext _context;

        public AdminCategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminCategoryResultDto>> GetCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Products)
                .Include(c => c.SubCategories)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new AdminCategoryResultDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Slug = c.Slug,
                    Description = c.Description,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = c.ParentCategory != null
                        ? c.ParentCategory.CategoryName
                        : null,
                    ProductCount = c.Products.Count,
                    SubCategoryCount = c.SubCategories.Count,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return categories;
        }

        public async Task<AdminCategoryResultDto?> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Products)
                .Include(c => c.SubCategories)
                .Where(c => c.CategoryId == categoryId)
                .Select(c => new AdminCategoryResultDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Slug = c.Slug,
                    Description = c.Description,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = c.ParentCategory != null
                        ? c.ParentCategory.CategoryName
                        : null,
                    ProductCount = c.Products.Count,
                    SubCategoryCount = c.SubCategories.Count,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync();

            return category;
        }

        public async Task<AdminCategoryActionResultDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var slug = string.IsNullOrWhiteSpace(dto.Slug)
                ? GenerateSlug(dto.CategoryName)
                : GenerateSlug(dto.Slug);

            var slugExists = await _context.Categories
                .AnyAsync(c => c.Slug == slug);

            if (slugExists)
            {
                return new AdminCategoryActionResultDto
                {
                    Success = false,
                    Message = "Slug danh mục đã tồn tại"
                };
            }

            if (dto.ParentCategoryId.HasValue)
            {
                var parentExists = await _context.Categories
                    .AnyAsync(c =>
                        c.CategoryId == dto.ParentCategoryId.Value &&
                        c.IsActive);

                if (!parentExists)
                {
                    return new AdminCategoryActionResultDto
                    {
                        Success = false,
                        Message = "Danh mục cha không tồn tại hoặc đã bị ẩn"
                    };
                }
            }

            var category = new Category
            {
                CategoryName = dto.CategoryName,
                Slug = slug,
                Description = dto.Description,
                ParentCategoryId = dto.ParentCategoryId,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var resultCategory = await GetCategoryByIdAsync(category.CategoryId);

            return new AdminCategoryActionResultDto
            {
                Success = true,
                Message = "Thêm danh mục thành công",
                Category = resultCategory
            };
        }

        public async Task<AdminCategoryActionResultDto> UpdateCategoryAsync(int categoryId, UpdateCategoryDto dto)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null)
            {
                return new AdminCategoryActionResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy danh mục"
                };
            }

            if (!string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                category.CategoryName = dto.CategoryName;
            }

            if (!string.IsNullOrWhiteSpace(dto.Slug))
            {
                var newSlug = GenerateSlug(dto.Slug);

                var slugExists = await _context.Categories
                    .AnyAsync(c => c.Slug == newSlug && c.CategoryId != categoryId);

                if (slugExists)
                {
                    return new AdminCategoryActionResultDto
                    {
                        Success = false,
                        Message = "Slug danh mục đã tồn tại"
                    };
                }

                category.Slug = newSlug;
            }

            if (dto.Description != null)
            {
                category.Description = dto.Description;
            }

            if (dto.ParentCategoryId.HasValue)
            {
                var newParentId = dto.ParentCategoryId.Value;

                if (newParentId == categoryId)
                {
                    return new AdminCategoryActionResultDto
                    {
                        Success = false,
                        Message = "Danh mục không thể chọn chính nó làm danh mục cha"
                    };
                }

                // BR-30 / BR-30B: Kiểm tra chu trình lặp tổ tiên (Ancestor Traversal Check)
                int? traceParentId = newParentId;
                while (traceParentId.HasValue)
                {
                    if (traceParentId.Value == categoryId)
                    {
                        return new AdminCategoryActionResultDto
                        {
                            Success = false,
                            Message = "Không thể chọn danh mục con hoặc cháu làm danh mục cha (ngăn ngừa chu trình lặp vô hạn)"
                        };
                    }

                    var traceCat = await _context.Categories
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.CategoryId == traceParentId.Value);

                    if (traceCat == null || !traceCat.IsActive)
                    {
                        return new AdminCategoryActionResultDto
                        {
                            Success = false,
                            Message = "Danh mục cha không tồn tại hoặc đã bị ẩn"
                        };
                    }

                    traceParentId = traceCat.ParentCategoryId;
                }

                category.ParentCategoryId = newParentId;
            }
            else
            {
                // Cho phép chuyển thành danh mục gốc (Root Category)
                category.ParentCategoryId = null;
            }

            if (dto.IsActive.HasValue)
            {
                category.IsActive = dto.IsActive.Value;
            }

            category.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var resultCategory = await GetCategoryByIdAsync(category.CategoryId);

            return new AdminCategoryActionResultDto
            {
                Success = true,
                Message = "Cập nhật danh mục thành công",
                Category = resultCategory
            };
        }

        public async Task<AdminCategoryActionResultDto> DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null)
            {
                return new AdminCategoryActionResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy danh mục"
                };
            }

            if (category.Products.Any(p => p.IsActive))
            {
                return new AdminCategoryActionResultDto
                {
                    Success = false,
                    Message = "Không thể ẩn danh mục vì vẫn còn sản phẩm đang hoạt động"
                };
            }

            if (category.SubCategories.Any(c => c.IsActive))
            {
                return new AdminCategoryActionResultDto
                {
                    Success = false,
                    Message = "Không thể ẩn danh mục vì vẫn còn danh mục con đang hoạt động"
                };
            }

            category.IsActive = false;
            category.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var resultCategory = await GetCategoryByIdAsync(category.CategoryId);

            return new AdminCategoryActionResultDto
            {
                Success = true,
                Message = "Ẩn danh mục thành công",
                Category = resultCategory
            };
        }

        public async Task<AdminCategoryActionResultDto> MigrateTaxonomyAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            var catMap = categories.ToDictionary(c => c.CategoryId);

            // 1. Cập nhật các danh mục gốc và con hiện có
            void UpdateCat(int id, string name, string slug, string desc)
            {
                if (catMap.TryGetValue(id, out var cat))
                {
                    cat.CategoryName = name;
                    cat.Slug = slug;
                    cat.Description = desc;
                    cat.UpdatedAt = DateTime.Now;
                }
            }

            UpdateCat(1, "Cần câu", "can-cau", "Cần câu lure, cần câu tay, cần câu lục và cần câu đa năng");
            UpdateCat(5, "Cần câu Tay - Đài", "can-cau-tay-dai", "Cần câu đơn, cần câu đài carbon cao cấp");
            UpdateCat(6, "Cần câu Đa năng - Đáy", "can-cau-da-nang-day", "Cần câu sông, câu biển ngâm đáy đa năng");
            UpdateCat(7, "Cần câu Biển - Jigging", "can-cau-bien-jigging", "Cần câu biển, cần câu lure biển và jigging");
            UpdateCat(8, "Cần câu Lure", "can-cau-lure", "Cần câu lure máy đứng và máy ngang chính hãng Shimano, Daiwa, Abu Garcia");
            UpdateCat(9, "Cần câu Lục - Surf", "can-cau-luc-surf", "Cần câu lục xa bờ, cần câu bãi biển surfcasting");

            UpdateCat(2, "Mồi câu", "moi-cau", "Mồi giả câu lure, mồi cám đài và tinh mùi dẫn dụ");
            UpdateCat(10, "Mồi giả Hard Bait", "moi-gia-hard-bait", "Mồi cá giả cứng, Minnow, Crankbait, Popper, Pencil");
            UpdateCat(11, "Mồi mềm Soft Bait", "moi-mem-soft-bait", "Mồi giả mềm silicone, giun mềm, nhái hơi, tôm mềm");
            UpdateCat(12, "Mồi sắt - Kim loại", "moi-sat-kim-loai", "Mồi jig kim loại, thìa spoon, spinner, blade bait");
            UpdateCat(13, "Mồi cám & Bột câu đài", "moi-cam-bot-cau-dai", "Mồi cám ủ, bột câu cá chép, rô phi, trôi, mè chất lượng cao");
            UpdateCat(14, "Hương liệu & Tinh mùi dẫn dụ", "huong-lieu-tinh-mui-dan-du", "Tinh chất dụ cá, hương bắp, tinh mùi tanh cám đài");

            UpdateCat(3, "Dây câu", "day-cau", "Dây dù PE, dây cước nylon, dây fluorocarbon và dây leader");
            UpdateCat(15, "Dây PE / Dù bện", "day-pe-du-ben", "Dây dù câu cá PE X4, X8, X9 siêu bền chống mài mòn");
            UpdateCat(16, "Dây Cước Nylon", "day-cuoc-nylon", "Dây câu cước monofilament mềm mại, dẻo dai");
            UpdateCat(17, "Dây Fluorocarbon", "day-fluorocarbon", "Dây câu tàng hình chống trầy xước trong nước");
            UpdateCat(18, "Dây Shock Leader", "day-shock-leader", "Dây dẫn đầu cần chống sốc, chịu tải cao");
            UpdateCat(19, "Dây thẻo buộc sẵn", "day-theo-buoc-san", "Bộ thẻo câu đài, thẻo câu lục, thẻo chống cắn buộc sẵn");

            UpdateCat(4, "Phụ kiện", "phu-kien", "Lưỡi câu, phao, hộp túi đựng đồ và dụng cụ hỗ trợ câu cá");
            UpdateCat(20, "Lưỡi câu cá", "luoi-cau-ca", "Lưỡi câu đơn, lưỡi chinu, iseama, lưỡi ba chấu");
            UpdateCat(21, "Phao câu & Đèn đêm", "phao-cau-den-dem", "Phao đài nano, phao điện câu đêm, phao lục");
            UpdateCat(22, "Hộp & Túi đựng đồ câu", "hop-va-tui-dung-do-cau", "Thùng đựng cá, hộp mồi lure, bao đựng cần");
            UpdateCat(23, "Kìm & Dụng cụ gỡ cá", "kim-va-dung-cu-go-ca", "Kìm gỡ cá, kìm cắt dây PE, kéo, dụng cụ thắt nút");
            UpdateCat(24, "Giá đỡ cần & Ghế câu", "gia-do-can-va-ghe-cau", "Giá chống cần inox, ghế xếp câu cá dã ngoại");

            // 2. Thêm Danh mục gốc "Máy câu" nếu chưa có
            var mayCau = categories.FirstOrDefault(c => c.Slug == "may-cau");
            if (mayCau == null)
            {
                mayCau = new Category
                {
                    CategoryName = "Máy câu",
                    Slug = "may-cau",
                    Description = "Máy câu đứng, máy câu ngang và máy câu biển chính hãng",
                    ParentCategoryId = null,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.Categories.Add(mayCau);
                await _context.SaveChangesAsync();

                var sub1 = new Category
                {
                    CategoryName = "Máy câu đứng (Spinning)",
                    Slug = "may-cau-dung-spinning",
                    Description = "Máy câu đứng đa năng từ size 1000 đến 6000",
                    ParentCategoryId = mayCau.CategoryId,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                var sub2 = new Category
                {
                    CategoryName = "Máy câu ngang (Baitcasting)",
                    Slug = "may-cau-ngang-baitcasting",
                    Description = "Máy câu ngang chuyên câu lure săn mồi",
                    ParentCategoryId = mayCau.CategoryId,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                var sub3 = new Category
                {
                    CategoryName = "Máy câu biển & Đáy",
                    Slug = "may-cau-bien-day",
                    Description = "Máy câu biển chịu mặn, máy trolling tải lớn",
                    ParentCategoryId = mayCau.CategoryId,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Categories.AddRange(sub1, sub2, sub3);
                await _context.SaveChangesAsync();

                // Thêm sản phẩm máy câu mẫu nếu chưa có
                var reelProds = new List<Product>
                {
                    new Product
                    {
                        ProductName = "Máy câu đứng Shimano Stradic FL 2500HG",
                        Slug = "may-cau-dung-shimano-stradic-fl-2500hg",
                        ShortDescription = "Máy câu đứng cao cấp công nghệ Hagane Body, X-Protect chống nước",
                        Description = "Shimano Stradic FL 2500HG là dòng máy câu đứng huyền thoại của Shimano, trang bị bánh răng Micromodule II, Silent Drive cho chuyển động êm ái tuyệt đối.",
                        Price = 3850000,
                        SalePrice = 3650000,
                        StockQuantity = 15,
                        CategoryId = sub1.CategoryId,
                        IsFeatured = true,
                        IsActive = true,
                        ThumbnailUrl = "/images/products/product-1.png",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Product
                    {
                        ProductName = "Máy câu ngang Daiwa Tatula SV TW 103HL",
                        Slug = "may-cau-ngang-daiwa-tatula-sv-tw-103hl",
                        ShortDescription = "Máy câu ngang chuyên lure công nghệ SV Spool chống rối dây",
                        Description = "Daiwa Tatula SV TW trang bị hệ thống dẫn dây T-Wing System (TWS) và Spool SV nhôm siêu nhẹ, hạn chế tối đa rối dây khi ném mồi nhẹ.",
                        Price = 4200000,
                        SalePrice = 3990000,
                        StockQuantity = 12,
                        CategoryId = sub2.CategoryId,
                        IsFeatured = true,
                        IsActive = true,
                        ThumbnailUrl = "/images/products/product-2.png",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Product
                    {
                        ProductName = "Máy câu đứng Daiwa BG MQ 4000D-H",
                        Slug = "may-cau-dung-daiwa-bg-mq-4000d-h",
                        ShortDescription = "Máy câu biển Monocoque Body siêu cứng cáp, lực kéo 12kg",
                        Description = "Daiwa BG MQ là dòng máy bạo lực câu bờ biển, câu đầm ghềnh với khung thân đúc liền khối Monocoque Body chịu tải vượt trội.",
                        Price = 2950000,
                        SalePrice = 2750000,
                        StockQuantity = 10,
                        CategoryId = sub1.CategoryId,
                        IsFeatured = false,
                        IsActive = true,
                        ThumbnailUrl = "/images/products/product-3.png",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };
                _context.Products.AddRange(reelProds);
            }

            // 3. Chuyển đổi CategoryId của sản phẩm
            var products = await _context.Products.ToListAsync();
            var prodMap = products.ToDictionary(p => p.ProductId);

            // Cần câu Lure chuyên dụng (Abu 1..4, Daiwa 6..9, Shimano 16..17) -> CatID 8
            var lureRodIds = new[] { 1, 2, 3, 4, 6, 7, 8, 9, 16, 17 };
            foreach (var pid in lureRodIds)
            {
                if (prodMap.TryGetValue(pid, out var prod))
                {
                    prod.CategoryId = 8;
                    prod.UpdatedAt = DateTime.Now;
                }
            }

            // Cần đa năng / câu đáy (Abu 5, Daiwa 10, Shimano 18, 19, 20) -> CatID 6
            var daNangRodIds = new[] { 5, 10, 18, 19, 20 };
            foreach (var pid in daNangRodIds)
            {
                if (prodMap.TryGetValue(pid, out var prod))
                {
                    prod.CategoryId = 6;
                    prod.UpdatedAt = DateTime.Now;
                }
            }

            // Cám rô phi (76, 77, 78, 79) -> CatID 13 (Mồi cám & Bột câu đài)
            var camIds = new[] { 76, 77, 78, 79 };
            foreach (var pid in camIds)
            {
                if (prodMap.TryGetValue(pid, out var prod))
                {
                    prod.CategoryId = 13;
                    prod.UpdatedAt = DateTime.Now;
                }
            }

            // Tinh chất dụ chép (75) -> CatID 14 (Hương liệu & Tinh mùi dẫn dụ)
            if (prodMap.TryGetValue(75, out var tinhChat))
            {
                tinhChat.CategoryId = 14;
                tinhChat.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return new AdminCategoryActionResultDto
            {
                Success = true,
                Message = "Tối ưu hóa cấu trúc cây danh mục và điều chỉnh sản phẩm thành công"
            };
        }

        private static string GenerateSlug(string value)
        {
            value = value.ToLower().Trim();

            value = Regex.Replace(value, @"\s+", "-");

            value = Regex.Replace(value, @"[^a-z0-9\-]", "");

            value = Regex.Replace(value, @"-+", "-");

            return value.Trim('-');
        }
    }
}