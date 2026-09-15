using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASPtestShop.Migrations
{
    /// <inheritdoc />
    public partial class SyncPTTKEntitiesAndConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews");

            migrationBuilder.AddColumn<int>(
                name: "OrderItemId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CouponCodeSnapshot",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsageLimitPerCustomer",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsageLimitTotal",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsedCount",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Branch = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccounts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OrderItemId",
                table: "Reviews",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId_OrderItemId",
                table: "Reviews",
                columns: new[] { "UserId", "OrderItemId" },
                unique: true,
                filter: "[OrderItemId] IS NOT NULL AND [UserId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Review_Rating",
                table: "Reviews",
                sql: "[Rating] >= 1 AND [Rating] <= 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Product_StockQuantity",
                table: "Products",
                sql: "[StockQuantity] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Order_DiscountAmount",
                table: "Orders",
                sql: "[DiscountAmount] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Order_FinalAmount",
                table: "Orders",
                sql: "[FinalAmount] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Order_TotalAmount",
                table: "Orders",
                sql: "[TotalAmount] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderItem_Quantity",
                table: "OrderItems",
                sql: "[Quantity] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Coupon_UsageLimitTotal",
                table: "Coupons",
                sql: "[UsageLimitTotal] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Coupon_UsedCount",
                table: "Coupons",
                sql: "[UsedCount] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CartItem_Quantity",
                table: "CartItems",
                sql: "[Quantity] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_UserId",
                table: "BankAccounts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_OrderItems_OrderItemId",
                table: "Reviews",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "OrderItemId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_OrderItems_OrderItemId",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_OrderItemId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_UserId_OrderItemId",
                table: "Reviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Review_Rating",
                table: "Reviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Product_StockQuantity",
                table: "Products");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Order_DiscountAmount",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Order_FinalAmount",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Order_TotalAmount",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderItem_Quantity",
                table: "OrderItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Coupon_UsageLimitTotal",
                table: "Coupons");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Coupon_UsedCount",
                table: "Coupons");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CartItem_Quantity",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CouponCodeSnapshot",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UsageLimitPerCustomer",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "UsageLimitTotal",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "UsedCount",
                table: "Coupons");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");
        }
    }
}
