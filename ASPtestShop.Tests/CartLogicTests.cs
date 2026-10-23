using System;
using System.Threading.Tasks;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Cart;
using ASPtestShop.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ASPtestShop.Tests
{
    public class CartLogicTests
    {
        private AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddToCart_ShouldFail_WhenProductIsInactive()
        {
            using var context = CreateInMemoryDbContext();
            var product = new Product
            {
                ProductId = 101,
                ProductName = "Inactive Rod",
                Slug = "inactive-rod",
                Price = 500000,
                StockQuantity = 10,
                IsActive = false
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var cartService = new CartService(context);
            var result = await cartService.AddToCartAsync("user1", new AddToCartDto { ProductId = 101, Quantity = 1 });

            Assert.False(result.Success);
            Assert.Equal("InactiveProduct", result.ErrorCode);
        }

        [Fact]
        public async Task AddToCart_ShouldFail_WhenExceedingStock()
        {
            using var context = CreateInMemoryDbContext();
            var product = new Product
            {
                ProductId = 102,
                ProductName = "Limited Lure",
                Slug = "limited-lure",
                Price = 100000,
                StockQuantity = 3,
                IsActive = true
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var cartService = new CartService(context);
            // Cố gắng mua 5 cái trong khi chỉ còn 3
            var result = await cartService.AddToCartAsync("user1", new AddToCartDto { ProductId = 102, Quantity = 5 });

            Assert.False(result.Success);
            Assert.Equal("OutOfStock", result.ErrorCode);
        }

        [Fact]
        public async Task UpdateCartItem_ShouldFail_WhenExceedingStock()
        {
            using var context = CreateInMemoryDbContext();
            var product = new Product
            {
                ProductId = 103,
                ProductName = "Limited Reel",
                Slug = "limited-reel",
                Price = 200000,
                StockQuantity = 2,
                IsActive = true
            };
            context.Products.Add(product);

            var cart = new Cart { CartId = 1, UserId = "user1" };
            var cartItem = new CartItem { CartItemId = 1, CartId = 1, ProductId = 103, Quantity = 1, UnitPrice = 200000 };
            context.Carts.Add(cart);
            context.CartItems.Add(cartItem);
            await context.SaveChangesAsync();

            var cartService = new CartService(context);
            var result = await cartService.UpdateCartItemAsync("user1", new UpdateCartItemDto { CartItemId = 1, Quantity = 10 });

            Assert.False(result.Success);
            Assert.Equal("OutOfStock", result.ErrorCode);
        }

        [Fact]
        public async Task GetCart_ShouldSyncWithLatestProductPrice()
        {
            using var context = CreateInMemoryDbContext();
            var product = new Product
            {
                ProductId = 104,
                ProductName = "Price Drop Hook",
                Slug = "price-drop-hook",
                Price = 300000, // Giá ban đầu
                StockQuantity = 10,
                IsActive = true
            };
            context.Products.Add(product);

            var cart = new Cart { CartId = 2, UserId = "user2" };
            var cartItem = new CartItem { CartItemId = 2, CartId = 2, ProductId = 104, Quantity = 2, UnitPrice = 300000 };
            context.Carts.Add(cart);
            context.CartItems.Add(cartItem);
            await context.SaveChangesAsync();

            // Admin giảm giá xuống 250000
            product.Price = 250000;
            await context.SaveChangesAsync();

            var cartService = new CartService(context);
            var cartResult = await cartService.GetCartAsync("user2");

            Assert.True(cartResult.Success);
            Assert.Equal(500000, cartResult.TotalAmount); // 2 * 250,000 = 500,000 (thay vì 600,000)
            Assert.Equal(250000, cartResult.Items[0].UnitPrice);
        }
    }
}
