using ASPtestShop.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.MVC.Admin
{
    [Route("admin/refunds")]
    [Authorize(AuthenticationSchemes = AdminCookieAuth.Scheme, Roles = "Admin")]
    public class AdminRefundController : Controller
    {
        // GET: /admin/refunds
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
