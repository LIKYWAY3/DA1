using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ASPtestShop.Services.Implementations.PaymentProviders;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ASPtestShop.Tests
{
    public class PaymentSecurityTests
    {
        [Fact]
        public void MomoVerifySignature_ShouldRejectForgedSignature()
        {
            var config = new ConfigurationBuilder().Build();
            var momo = new MomoPaymentProvider(config, null!);
            var secretKey = "test_secret_key";

            var forgedParams = new Dictionary<string, string>
            {
                { "partnerCode", "MOMO" },
                { "orderId", "OD123" },
                { "requestId", "REQ123" },
                { "amount", "100000" },
                { "orderInfo", "Test" },
                { "orderType", "momo_wallet" },
                { "transId", "999999" },
                { "resultCode", "0" },
                { "message", "Successful" },
                { "payType", "qr" },
                { "responseTime", "1700000000000" },
                { "extraData", "" },
                { "signature", "forged_invalid_signature_hex" }
            };

            var isValid = momo.VerifySignature(forgedParams, secretKey);

            Assert.False(isValid);
        }

        [Fact]
        public void ZaloPayVerifyReturn_ShouldRejectForgedOrMissingChecksum()
        {
            var config = new ConfigurationBuilder().Build();
            var zalo = new ZaloPayPaymentProvider(config, null!);
            var key2 = "test_key_2";

            var forgedParams = new Dictionary<string, string>
            {
                { "appid", "2553" },
                { "apptransid", "261020_OD123" },
                { "pmcid", "1" },
                { "bankcode", "CC" },
                { "amount", "100000" },
                { "discountamount", "0" },
                { "status", "1" },
                { "checksum", "forged_checksum" }
            };

            var isValid = zalo.VerifyReturn(forgedParams, key2);

            Assert.False(isValid);
        }

        [Fact]
        public void ZaloPayVerifyReturn_ShouldAcceptValidChecksum()
        {
            var config = new ConfigurationBuilder().Build();
            var zalo = new ZaloPayPaymentProvider(config, null!);
            var key2 = "test_key_2";

            var appId = "2553";
            var appTransId = "261020_OD123";
            var pmcId = "1";
            var bankCode = "CC";
            var amount = "100000";
            var discountAmount = "0";
            var status = "1";

            var rawData = $"{appId}|{appTransId}|{pmcId}|{bankCode}|{amount}|{discountAmount}|{status}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key2));
            var expectedChecksum = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData))).ToLowerInvariant();

            var validParams = new Dictionary<string, string>
            {
                { "appid", appId },
                { "apptransid", appTransId },
                { "pmcid", pmcId },
                { "bankcode", bankCode },
                { "amount", amount },
                { "discountamount", discountAmount },
                { "status", status },
                { "checksum", expectedChecksum }
            };

            var isValid = zalo.VerifyReturn(validParams, key2);

            Assert.True(isValid);
        }
        [Fact]
        public async Task VietQrPaymentProvider_ShouldGenerateValidPaymentUrl()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "VietQR:BankId", "MB" },
                { "VietQR:AccountNo", "0987654321" },
                { "VietQR:AccountName", "FUU FISHING STORE" },
                { "VietQR:Template", "compact2" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            var provider = new VietQrPaymentProvider(config);

            var request = new ASPtestShop.Models.DTO.Payment.CreatePaymentRequestDto
            {
                OrderId = 10,
                OrderCode = "FUU12345678",
                Amount = 350000,
                PaymentMethod = "VIETQR"
            };

            var result = await provider.CreatePaymentAsync(request);

            Assert.True(result.IsSuccess);
            Assert.Equal("VIETQR", result.PaymentMethod);
            Assert.Equal("Unpaid", result.PaymentStatus);
            Assert.False(result.RequiresRedirect);
            Assert.NotNull(result.PaymentUrl);
            Assert.Contains("img.vietqr.io/image/MB-0987654321-compact2.png", result.PaymentUrl);
            Assert.Contains("amount=350000", result.PaymentUrl);
            Assert.Contains("addInfo=FUU12345678", result.PaymentUrl);
        }

        [Fact]
        public async Task VnPayPaymentProvider_ShouldNotContainLiteralBaseUrlInReturnUrl()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "VnPay:TmnCode", "TESTTMN" },
                { "VnPay:HashSecret", "TESTSECRET1234567890" },
                { "VnPay:BaseUrl", "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html" },
                { "VnPay:ReturnUrl", "/checkout/vnpay-return" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            var provider = new VnPayPaymentProvider(config);

            var request = new ASPtestShop.Models.DTO.Payment.CreatePaymentRequestDto
            {
                OrderId = 1,
                OrderCode = "TESTORDER01",
                Amount = 100000,
                PaymentMethod = "VNPAY",
                ReturnUrl = "/checkout/vnpay-return",
                IpAddress = "127.0.0.1"
            };

            var result = await provider.CreatePaymentAsync(request);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.PaymentUrl);
            Assert.DoesNotContain("{BaseUrl}", result.PaymentUrl);
            Assert.Contains("vnp_ReturnUrl=", result.PaymentUrl);
        }
    }
}
