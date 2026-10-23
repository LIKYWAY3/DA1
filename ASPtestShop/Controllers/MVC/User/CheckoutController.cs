using System.Security.Claims;
using System.Text.Json;
using ASPtestShop.Auth;
using ASPtestShop.Data;
using ASPtestShop.Services.Implementations.PaymentProviders;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ASPtestShop.Controllers.MVC.User
{
    [Route("checkout")]
    [Authorize(AuthenticationSchemes = UserCookieAuth.Scheme)]
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IOrderNotificationService _orderNotificationService;

        public CheckoutController(
            ICartService cartService,
            AppDbContext context,
            IConfiguration config,
            IOrderNotificationService orderNotificationService)
        {
            _cartService = cartService;
            _context = context;
            _config = config;
            _orderNotificationService = orderNotificationService;
        }

        // GET: /checkout
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge(UserCookieAuth.Scheme);
            }

            var hasCartItems =
                await _cartService.HasCartItemsAsync(userId);

            if (!hasCartItems)
            {
                TempData["ErrorMessage"] =
                    "Giỏ hàng đang trống. Vui lòng thêm sản phẩm trước khi thanh toán.";

                return RedirectToAction("Index", "Cart");
            }

            return View();
        }

        // GET: /checkout/success
        [HttpGet("success")]
        public IActionResult Success(string? orderCode = null)
        {
            ViewBag.OrderCode = orderCode;
            return View();
        }

        // =======================================================================
        //  VNPay Return (UC-18)
        //  VNPay redirect về đây sau khi khách hàng thanh toán
        // =======================================================================
        [HttpGet("vnpay-return")]
        [AllowAnonymous]
        public async Task<IActionResult> VnPayReturn()
        {
            var queryParams = Request.Query.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.ToString()
            );

            var hashSecret = _config["VnPay:HashSecret"] ?? "";
            var vnPayProvider = new VnPayPaymentProvider(_config);

            // Lấy mã giao dịch và mã đơn hàng từ callback
            queryParams.TryGetValue("vnp_TxnRef", out var orderCode);
            queryParams.TryGetValue("vnp_ResponseCode", out var responseCode);
            queryParams.TryGetValue("vnp_TransactionNo", out var transactionNo);
            queryParams.TryGetValue("vnp_Amount", out var amountStr);

            // Xác minh chữ ký
            var isValid = vnPayProvider.VerifySignature(queryParams, hashSecret);

            if (!isValid)
            {
                TempData["ErrorMessage"] = "Chữ ký VNPay không hợp lệ. Giao dịch bị từ chối!";
                return RedirectToAction("Index", "Home");
            }

            // Tìm đơn hàng theo OrderCode
            var order = await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("Index", "Home");
            }

            var payment = order.Payment;

            if (responseCode == "00") // Giao dịch thành công
            {
                // BẢO MẬT: Kiểm tra tính toàn vẹn số tiền thanh toán (Amount Integrity Check)
                if (long.TryParse(amountStr, out var vnpAmountRaw))
                {
                    var actualVnpAmount = vnpAmountRaw / 100m;
                    if (Math.Abs(actualVnpAmount - order.FinalAmount) > 1m)
                    {
                        TempData["ErrorMessage"] = "Cảnh báo bảo mật: Số tiền thanh toán VNPay không khớp với giá trị đơn hàng! Giao dịch bị tạm giữ.";
                        if (payment != null) payment.PaymentStatus = "Failed";
                        order.PaymentStatus = "Failed";
                        order.Note = $"{order.Note} | Cảnh báo gian lận: Số tiền VNPay ({actualVnpAmount}) lệch so với đơn ({order.FinalAmount})";
                        await _context.SaveChangesAsync();
                        return RedirectToAction("History", "Order");
                    }
                }

                if (payment != null)
                {
                    payment.PaymentStatus = "Paid";
                    payment.TransactionCode = transactionNo;
                    payment.PaidAt = DateTime.Now;
                }
                order.PaymentStatus = "Paid";
                await _context.SaveChangesAsync();

                // GIAI ĐOẠN 1: BẮN THÔNG BÁO REAL-TIME CHO ADMIN (VNPAY PAID)
                _ = _orderNotificationService.NotifyAdminNewOrderAsync(
                    order.OrderCode,
                    order.ReceiverName,
                    order.FinalAmount,
                    "VNPay",
                    "Paid"
                );

                return RedirectToAction("Success", new { orderCode });
            }
            else
            {
                // Thanh toán thất bại hoặc bị hủy
                if (payment != null)
                {
                    payment.PaymentStatus = "Failed";
                }
                order.PaymentStatus = "Failed";
                await _context.SaveChangesAsync();

                TempData["ErrorMessage"] = $"Thanh toán VNPay thất bại (Mã lỗi: {responseCode}). Vui lòng thử lại.";
                return RedirectToAction("History", "Order");
            }
        }

        // =======================================================================
        //  MoMo Return (UC-18)
        //  MoMo redirect về đây sau khi khách hàng thanh toán
        // =======================================================================
        [HttpGet("momo-return")]
        [AllowAnonymous]
        public async Task<IActionResult> MomoReturn()
        {
            var queryParams = Request.Query.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.ToString()
            );

            var secretKey = _config["Momo:SecretKey"] ?? "";
            var momoProvider = new MomoPaymentProvider(_config, HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>());
            var isSignatureValid = momoProvider.VerifySignature(queryParams, secretKey);

            if (!isSignatureValid)
            {
                TempData["ErrorMessage"] = "Chữ ký MoMo không hợp lệ. Giao dịch bị từ chối!";
                return RedirectToAction("Index", "Home");
            }

            queryParams.TryGetValue("orderId",    out var orderCode);
            queryParams.TryGetValue("resultCode", out var resultCodeStr);
            queryParams.TryGetValue("transId",    out var transId);
            queryParams.TryGetValue("amount",     out var momoAmountStr);

            var resultCode = int.TryParse(resultCodeStr, out var rc) ? rc : -1;

            // Tìm đơn hàng
            var order = await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("Index", "Home");
            }

            var payment = order.Payment;

            if (resultCode == 0) // Thành công
            {
                // BẢO MẬT: Kiểm tra tính toàn vẹn số tiền thanh toán MoMo
                if (decimal.TryParse(momoAmountStr, out var momoAmount))
                {
                    if (Math.Abs(momoAmount - order.FinalAmount) > 1m)
                    {
                        TempData["ErrorMessage"] = "Cảnh báo bảo mật: Số tiền thanh toán MoMo không khớp với giá trị đơn hàng!";
                        if (payment != null) payment.PaymentStatus = "Failed";
                        order.PaymentStatus = "Failed";
                        order.Note = $"{order.Note} | Cảnh báo gian lận: Số tiền MoMo ({momoAmount}) lệch so với đơn ({order.FinalAmount})";
                        await _context.SaveChangesAsync();
                        return RedirectToAction("History", "Order");
                    }
                }
                if (payment != null)
                {
                    payment.PaymentStatus = "Paid";
                    payment.TransactionCode = transId;
                    payment.PaidAt = DateTime.Now;
                }
                order.PaymentStatus = "Paid";
                await _context.SaveChangesAsync();

                // GIAI ĐOẠN 1: BẮN THÔNG BÁO REAL-TIME CHO ADMIN (MOMO PAID)
                _ = _orderNotificationService.NotifyAdminNewOrderAsync(
                    order.OrderCode,
                    order.ReceiverName,
                    order.FinalAmount,
                    "MoMo",
                    "Paid"
                );

                return RedirectToAction("Success", new { orderCode });
            }
            else
            {
                if (payment != null)
                {
                    payment.PaymentStatus = "Failed";
                }
                order.PaymentStatus = "Failed";
                await _context.SaveChangesAsync();

                TempData["ErrorMessage"] = $"Thanh toán MoMo thất bại (Mã lỗi: {resultCode}). Vui lòng thử lại.";
                return RedirectToAction("History", "Order");
            }
        }

        // =======================================================================
        //  MoMo IPN Notify (UC-18)
        //  MoMo gọi API này để xác nhận giao dịch (server-to-server)
        // =======================================================================
        [HttpPost("momo-notify")]
        [AllowAnonymous]
        public async Task<IActionResult> MomoNotify()
        {
            try
            {
                using var reader = new System.IO.StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                var secretKey = _config["Momo:SecretKey"] ?? "";
                var momoProvider = new MomoPaymentProvider(_config, HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>());

                var callbackParams = new Dictionary<string, string>();
                foreach (var prop in root.EnumerateObject())
                {
                    callbackParams[prop.Name] = prop.Value.ToString();
                }

                if (!momoProvider.VerifySignature(callbackParams, secretKey))
                {
                    return Ok(new { message = "invalid signature" });
                }

                var resultCode = root.TryGetProperty("resultCode", out var rcProp) ? rcProp.GetInt32() : -1;
                var orderId    = root.TryGetProperty("orderId", out var oidProp) ? oidProp.GetString() : null;
                var transId    = root.TryGetProperty("transId", out var tidProp) ? tidProp.GetInt64().ToString() : null;

                if (resultCode == 0 && !string.IsNullOrEmpty(orderId))
                {
                    var order = await _context.Orders
                        .Include(o => o.Payment)
                        .FirstOrDefaultAsync(o => o.OrderCode == orderId);

                    if (order != null)
                    {
                        var payment = order.Payment;
                        if (payment != null && payment.PaymentStatus != "Paid")
                        {
                            payment.PaymentStatus = "Paid";
                            payment.TransactionCode = transId;
                            payment.PaidAt = DateTime.Now;
                            order.PaymentStatus = "Paid";
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                return Ok(new { message = "ok" });
            }
            catch
            {
                return Ok(new { message = "error" });
            }
        }

        // =======================================================================
        //  ZaloPay Return (UC-18)
        //  ZaloPay redirect về đây sau khi khách hàng thanh toán
        // =======================================================================
        [HttpGet("zalopay-return")]
        [AllowAnonymous]
        public async Task<IActionResult> ZaloPayReturn()
        {
            var queryParams = Request.Query.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.ToString()
            );

            var key2 = _config["ZaloPay:Key2"] ?? "";
            var zaloPayProvider = new ZaloPayPaymentProvider(_config, HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>());

            var isSignatureValid = zaloPayProvider.VerifyReturn(queryParams, key2);
            if (!isSignatureValid)
            {
                TempData["ErrorMessage"] = "Chữ ký ZaloPay không hợp lệ. Giao dịch bị từ chối!";
                return RedirectToAction("Index", "Home");
            }

            queryParams.TryGetValue("apptransid", out var appTransId);
            queryParams.TryGetValue("status",     out var statusStr);
            queryParams.TryGetValue("amount",     out var amountStr);

            var status = int.TryParse(statusStr, out var s) ? s : -1;

            // appTransId có dạng: yyMMdd_OrderCode
            var orderCode = appTransId?.Contains("_") == true
                ? appTransId.Substring(appTransId.IndexOf('_') + 1)
                : appTransId;

            var order = await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("Index", "Home");
            }

            var payment = order.Payment;

            if (status == 1) // Thành công
            {
                // BẢO MẬT: Kiểm tra tính toàn vẹn số tiền thanh toán ZaloPay
                if (decimal.TryParse(amountStr, out var zaloAmount))
                {
                    if (Math.Abs(zaloAmount - order.FinalAmount) > 1m)
                    {
                        TempData["ErrorMessage"] = "Cảnh báo bảo mật: Số tiền thanh toán ZaloPay không khớp với giá trị đơn hàng!";
                        if (payment != null) payment.PaymentStatus = "Failed";
                        order.PaymentStatus = "Failed";
                        order.Note = $"{order.Note} | Cảnh báo gian lận: Số tiền ZaloPay ({zaloAmount}) lệch so với đơn ({order.FinalAmount})";
                        await _context.SaveChangesAsync();
                        return RedirectToAction("History", "Order");
                    }
                }

                if (payment != null)
                {
                    payment.PaymentStatus = "Paid";
                    payment.TransactionCode = appTransId;
                    payment.PaidAt = DateTime.Now;
                }
                order.PaymentStatus = "Paid";
                await _context.SaveChangesAsync();

                return RedirectToAction("Success", new { orderCode });
            }
            else
            {
                if (payment != null)
                {
                    payment.PaymentStatus = "Failed";
                }
                order.PaymentStatus = "Failed";
                await _context.SaveChangesAsync();

                TempData["ErrorMessage"] = "Thanh toán ZaloPay thất bại hoặc bị hủy. Vui lòng thử lại.";
                return RedirectToAction("History", "Order");
            }
        }

        // =======================================================================
        //  ZaloPay Callback IPN (UC-18)
        //  ZaloPay gọi API này để xác nhận giao dịch (server-to-server)
        // =======================================================================
        [HttpPost("zalopay-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> ZaloPayCallback()
        {
            try
            {
                using var reader = new System.IO.StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                var data = root.TryGetProperty("data", out var dataProp) ? dataProp.GetString() : null;
                var mac  = root.TryGetProperty("mac",  out var macProp)  ? macProp.GetString()  : null;

                var key2 = _config["ZaloPay:Key2"] ?? "";
                var zaloPayProvider = new ZaloPayPaymentProvider(_config, HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>());

                if (!zaloPayProvider.VerifyCallback(data ?? "", mac ?? "", key2))
                {
                    return Ok(new { return_code = -1, return_message = "Invalid signature" });
                }

                // Parse data JSON để lấy app_trans_id
                using var dataDoc = JsonDocument.Parse(data ?? "{}");
                var appTransId = dataDoc.RootElement.TryGetProperty("app_trans_id", out var atProp) ? atProp.GetString() : null;

                var orderCode = appTransId?.Contains("_") == true
                    ? appTransId.Substring(appTransId.IndexOf('_') + 1)
                    : appTransId;

                var order = await _context.Orders
                    .Include(o => o.Payment)
                    .FirstOrDefaultAsync(o => o.OrderCode == orderCode);

                if (order != null)
                {
                    var payment = order.Payment;
                    if (payment != null && payment.PaymentStatus != "Paid")
                    {
                        payment.PaymentStatus = "Paid";
                        payment.TransactionCode = appTransId;
                        payment.PaidAt = DateTime.Now;
                        order.PaymentStatus = "Paid";
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok(new { return_code = 1, return_message = "success" });
            }
            catch
            {
                return Ok(new { return_code = -1, return_message = "error" });
            }
        }
    }
}