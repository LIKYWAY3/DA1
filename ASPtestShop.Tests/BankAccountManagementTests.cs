using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.ViewModels.Profile;
using ASPtestShop.Services.Implementations.User;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace ASPtestShop.Tests
{
    public class BankAccountManagementTests
    {
        private AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new AppDbContext(options);
        }

        private UserAuthService CreateService(AppDbContext context)
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var envMock = new Mock<IWebHostEnvironment>();
            var cacheMock = new Mock<IMemoryCache>();
            var emailServiceMock = new Mock<IEmailService>();
            var httpClient = new HttpClient();

            return new UserAuthService(
                userManagerMock.Object,
                envMock.Object,
                httpClient,
                context,
                cacheMock.Object,
                emailServiceMock.Object
            );
        }

        #region 1. THÊM TÀI KHOẢN NGÂN HÀNG & RÀNG BUỘC KÝ TỰ
        [Fact]
        public async Task AddBankAccount_WhenValidData_ShouldSucceedAndPersistInDb()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var service = CreateService(context);
            var userId = "user-101";

            var model = new AddBankAccountViewModel
            {
                BankName = "Vietcombank (VCB)",
                AccountName = "Nguyen Van A",
                AccountNumber = "0123456789",
                Branch = "Chi nhánh Cần Thơ",
                IsDefault = false
            };

            // Act
            var result = await service.AddBankAccountAsync(userId, model);

            // Assert
            Assert.True(result.Success);
            var savedAccount = await context.BankAccounts.FirstOrDefaultAsync(b => b.UserId == userId);
            Assert.NotNull(savedAccount);
            Assert.Equal("Vietcombank (VCB)", savedAccount.BankName);
            Assert.Equal("NGUYEN VAN A", savedAccount.AccountName); // Chuẩn hóa viết hoa
            Assert.Equal("0123456789", savedAccount.AccountNumber);
            Assert.True(savedAccount.IsDefault); // Tài khoản đầu tiên tự động đặt làm mặc định
        }

        [Theory]
        [InlineData("12345abc")]
        [InlineData("1234-5678")]
        [InlineData("123 456 789")]
        [InlineData("123")] // Quá ngắn (< 6 số)
        [InlineData("!@#$%^&*()")]
        public async Task AddBankAccount_WhenAccountNumberContainsLettersOrSpecialChars_ShouldFail(string invalidAccountNumber)
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var service = CreateService(context);
            var userId = "user-102";

            var model = new AddBankAccountViewModel
            {
                BankName = "MBBank (MB)",
                AccountName = "Tran Van B",
                AccountNumber = invalidAccountNumber,
                Branch = "Chi nhánh Hà Nội"
            };

            // Act
            var result = await service.AddBankAccountAsync(userId, model);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("chữ số", result.Message);
            Assert.Empty(context.BankAccounts.Where(b => b.UserId == userId));
        }
        #endregion

        #region 2. CẬP NHẬT TÀI KHOẢN & CHỐNG TẤN CÔNG IDOR
        [Fact]
        public async Task UpdateBankAccount_WhenAuthorizedOwner_ShouldUpdateSuccessfully()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var service = CreateService(context);
            var userId = "user-201";

            var initialAccount = new BankAccount
            {
                UserId = userId,
                BankName = "Vietcombank (VCB)",
                AccountName = "LE VAN C",
                AccountNumber = "1111222233",
                Branch = "Chi nhánh Quận 1",
                IsDefault = true,
                CreatedAt = DateTime.UtcNow
            };
            context.BankAccounts.Add(initialAccount);
            await context.SaveChangesAsync();

            var updateModel = new AddBankAccountViewModel
            {
                BankName = "Techcombank (TCB)",
                AccountName = "LE VAN C UPDATED",
                AccountNumber = "9999888877",
                Branch = "Chi nhánh Quận 3",
                IsDefault = true
            };

            // Act
            var result = await service.UpdateBankAccountAsync(userId, initialAccount.Id, updateModel);

            // Assert
            Assert.True(result.Success);
            var updated = await context.BankAccounts.FindAsync(initialAccount.Id);
            Assert.NotNull(updated);
            Assert.Equal("Techcombank (TCB)", updated.BankName);
            Assert.Equal("9999888877", updated.AccountNumber);
            Assert.Equal("LE VAN C UPDATED", updated.AccountName);
            Assert.Equal("Chi nhánh Quận 3", updated.Branch);
        }

        [Fact]
        public async Task UpdateBankAccount_WhenDifferentUserAttemptsEdit_ShouldPreventIDOR()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var service = CreateService(context);
            var victimUserId = "victim-user";
            var attackerUserId = "attacker-user";

            var victimAccount = new BankAccount
            {
                UserId = victimUserId,
                BankName = "BIDV",
                AccountName = "NGUYEN VICTIM",
                AccountNumber = "1234567890",
                IsDefault = true,
                CreatedAt = DateTime.UtcNow
            };
            context.BankAccounts.Add(victimAccount);
            await context.SaveChangesAsync();

            var maliciousModel = new AddBankAccountViewModel
            {
                BankName = "Agribank",
                AccountName = "HACKER",
                AccountNumber = "9999999999"
            };

            // Act - Attacker attempts to modify victim's bank account ID
            var result = await service.UpdateBankAccountAsync(attackerUserId, victimAccount.Id, maliciousModel);

            // Assert - IDOR must be rejected
            Assert.False(result.Success);
            Assert.Contains("không có quyền", result.Message);

            // Verify database data remains untampered
            var checkAccount = await context.BankAccounts.FindAsync(victimAccount.Id);
            Assert.NotNull(checkAccount);
            Assert.Equal(victimUserId, checkAccount.UserId);
            Assert.Equal("1234567890", checkAccount.AccountNumber);
            Assert.Equal("BIDV", checkAccount.BankName);
        }
        #endregion

        #region 3. XÓA TÀI KHOẢN MẶC ĐỊNH & TỰ ĐỘNG CHUYỂN MẶC ĐỊNH
        [Fact]
        public async Task DeleteBankAccount_WhenDeletingDefaultAccount_ShouldTransferDefaultToNextAccount()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var service = CreateService(context);
            var userId = "user-301";

            var account1 = new BankAccount
            {
                UserId = userId,
                BankName = "Vietcombank (VCB)",
                AccountName = "PHAM THI D",
                AccountNumber = "1000000001",
                IsDefault = true,
                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
            };
            var account2 = new BankAccount
            {
                UserId = userId,
                BankName = "Techcombank (TCB)",
                AccountName = "PHAM THI D",
                AccountNumber = "2000000002",
                IsDefault = false,
                CreatedAt = DateTime.UtcNow
            };
            context.BankAccounts.AddRange(account1, account2);
            await context.SaveChangesAsync();

            // Act - Delete account1 (default)
            var result = await service.DeleteBankAccountAsync(userId, account1.Id);

            // Assert
            Assert.True(result.Success);
            Assert.Null(await context.BankAccounts.FindAsync(account1.Id));

            // Verify account2 is automatically promoted to Default
            var remainingAccount = await context.BankAccounts.FindAsync(account2.Id);
            Assert.NotNull(remainingAccount);
            Assert.True(remainingAccount.IsDefault);
        }

        [Fact]
        public async Task DeleteBankAccount_WhenAttackerAttemptsDelete_ShouldPreventIDOR()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var service = CreateService(context);
            var ownerId = "owner-user";
            var attackerId = "attacker-user";

            var account = new BankAccount
            {
                UserId = ownerId,
                BankName = "VietinBank (CTG)",
                AccountName = "CHINH CHU",
                AccountNumber = "5555666677",
                IsDefault = true,
                CreatedAt = DateTime.UtcNow
            };
            context.BankAccounts.Add(account);
            await context.SaveChangesAsync();

            // Act
            var result = await service.DeleteBankAccountAsync(attackerId, account.Id);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("không có quyền", result.Message);
            Assert.NotNull(await context.BankAccounts.FindAsync(account.Id));
        }
        #endregion

        #region 4. ĐẢM BẢO DUY NHẤT 1 TÀI KHOẢN MẶC ĐỊNH TẠI MỌI THỜI ĐIỂM
        [Fact]
        public async Task SetDefaultBankAccount_ShouldEnsureOnlySingleAccountIsDefaultAtAnyTime()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var service = CreateService(context);
            var userId = "user-401";

            var acc1 = new BankAccount { UserId = userId, BankName = "VCB", AccountName = "HOANG E", AccountNumber = "1111111111", IsDefault = true, CreatedAt = DateTime.UtcNow };
            var acc2 = new BankAccount { UserId = userId, BankName = "MB", AccountName = "HOANG E", AccountNumber = "2222222222", IsDefault = false, CreatedAt = DateTime.UtcNow };
            var acc3 = new BankAccount { UserId = userId, BankName = "TCB", AccountName = "HOANG E", AccountNumber = "3333333333", IsDefault = false, CreatedAt = DateTime.UtcNow };

            context.BankAccounts.AddRange(acc1, acc2, acc3);
            await context.SaveChangesAsync();

            // Act 1 - Set acc2 as default
            var result1 = await service.SetDefaultBankAccountAsync(userId, acc2.Id);

            // Assert 1
            Assert.True(result1.Success);
            var defaults1 = await context.BankAccounts.Where(b => b.UserId == userId && b.IsDefault).ToListAsync();
            var singleDefault1 = Assert.Single(defaults1);
            Assert.Equal(acc2.Id, singleDefault1.Id);

            // Act 2 - Set acc3 as default
            var result2 = await service.SetDefaultBankAccountAsync(userId, acc3.Id);

            // Assert 2
            Assert.True(result2.Success);
            var defaults2 = await context.BankAccounts.Where(b => b.UserId == userId && b.IsDefault).ToListAsync();
            var singleDefault2 = Assert.Single(defaults2);
            Assert.Equal(acc3.Id, singleDefault2.Id);

            // Verify acc1 and acc2 are strictly false
            var checkAcc1 = await context.BankAccounts.FindAsync(acc1.Id);
            var checkAcc2 = await context.BankAccounts.FindAsync(acc2.Id);
            Assert.NotNull(checkAcc1);
            Assert.NotNull(checkAcc2);
            Assert.False(checkAcc1.IsDefault);
            Assert.False(checkAcc2.IsDefault);
        }
        #endregion
    }
}
