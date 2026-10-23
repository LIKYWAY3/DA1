using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.MVC.User
{
    [Route("Shop")]
    public class ShopController : Controller
    {
        // GET: /Shop
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Shop/Detail/2
        [HttpGet("Detail/{id:int}")]
        public IActionResult Detail(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            ViewBag.ProductId = id;

            // Render view chi tiết sản phẩm đầy đủ có khối Đánh giá (Reviews)
            return View("~/Views/Product/Detail.cshtml");
        }
    }
}