using ASPtestShop.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        // ==========================================
        // DBSETS: E-COMMERCE
        // ==========================================
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }

        // ==========================================
        // DBSETS: CHAT SYSTEM
        // ==========================================
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatAttachment> ChatAttachments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // BỎ: Cấu hình Index cho UserName và Email vì IdentityDbContext đã tự làm.
            base.OnModelCreating(builder);

            // ==========================================
            // INDEXES (E-COMMERCE & CHAT)
            // ==========================================
            builder.Entity<Product>()
                .HasIndex(p => p.Slug)
                .IsUnique();

            builder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .IsUnique();

            builder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.ProductId })
                .IsUnique();

            builder.Entity<Coupon>()
                .HasIndex(c => c.Code)
                .IsUnique();

            builder.Entity<Payment>()
                .HasIndex(p => p.OrderId)
                .IsUnique();

            builder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.OrderItemId })
                .IsUnique()
                .HasFilter("[OrderItemId] IS NOT NULL AND [UserId] IS NOT NULL");

            builder.Entity<Conversation>()
                .HasIndex(c => c.UserId);

            builder.Entity<ChatMessage>()
                .HasIndex(cm => cm.ConversationId);

            builder.Entity<ChatMessage>()
                .HasIndex(cm => cm.Intent);


            // ==========================================
            // RELATIONSHIPS: E-COMMERCE SYSTEM
            // ==========================================

            // Category Self Relationship (Cha - Con)
            builder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product - Category
            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProductImage - Product
            builder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cart - User 
            // Lưu ý: Giữ Restrict từ Code 2 để tránh lỗi Multiple Cascade Paths với bảng User
            builder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // CartItem - Cart
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // CartItem - Product
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order - User
            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order - Coupon
            builder.Entity<Order>()
                .HasOne(o => o.Coupon)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CouponId)
                .OnDelete(DeleteBehavior.SetNull);

            // Order Item - Order
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order Item - Product
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order - Payment (One-to-One)
            builder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review - Product
            builder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review - User
            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review - OrderItem
            builder.Entity<Review>()
                .HasOne(r => r.OrderItem)
                .WithMany()
                .HasForeignKey(r => r.OrderItemId)
                .OnDelete(DeleteBehavior.SetNull);

            // BankAccount - User
            builder.Entity<BankAccount>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // ==========================================
            // RELATIONSHIPS: CHAT SYSTEM
            // ==========================================

            // Conversation -> User
            builder.Entity<Conversation>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Conversation -> ChatMessages (1 - N)
            builder.Entity<Conversation>()
                .HasMany(c => c.ChatMessages)
                .WithOne(cm => cm.Conversation)
                .HasForeignKey(cm => cm.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // ChatMessage -> ChatAttachments (1 - N)
            builder.Entity<ChatMessage>()
                .HasMany(cm => cm.Attachments)
                .WithOne(a => a.ChatMessage)
                .HasForeignKey(a => a.ChatMessageId)
                .OnDelete(DeleteBehavior.Cascade);


            // ==========================================
            // ENUM CONVERSIONS (Mã hóa Enum thành Int trong DB)
            // ==========================================
            builder.Entity<ChatMessage>()
                .Property(cm => cm.SenderType)
                .HasConversion<int>();

            builder.Entity<ChatMessage>()
                .Property(cm => cm.MessageType)
                .HasConversion<int>();

            builder.Entity<ChatAttachment>()
                .Property(ca => ca.AttachmentType)
                .HasConversion<int>();


            // ==========================================
            // CHECK CONSTRAINTS (TÍNH TOÀN VẸN DỮ LIỆU CSDL)
            // ==========================================
            builder.Entity<Product>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint("CK_Product_StockQuantity", "[StockQuantity] >= 0"));
            });

            builder.Entity<Review>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "[Rating] >= 1 AND [Rating] <= 5"));
            });

            builder.Entity<CartItem>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint("CK_CartItem_Quantity", "[Quantity] > 0"));
            });

            builder.Entity<OrderItem>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint("CK_OrderItem_Quantity", "[Quantity] > 0"));
            });

            builder.Entity<Order>(entity =>
            {
                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Order_TotalAmount", "[TotalAmount] >= 0");
                    t.HasCheckConstraint("CK_Order_FinalAmount", "[FinalAmount] >= 0");
                    t.HasCheckConstraint("CK_Order_DiscountAmount", "[DiscountAmount] >= 0");
                });
            });

            builder.Entity<Coupon>(entity =>
            {
                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Coupon_UsedCount", "[UsedCount] >= 0");
                    t.HasCheckConstraint("CK_Coupon_UsageLimitTotal", "[UsageLimitTotal] >= 0");
                });
            });
        }
    }
}