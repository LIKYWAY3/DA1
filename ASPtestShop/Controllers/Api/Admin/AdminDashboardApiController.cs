using ASPtestShop.Auth;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api.Admin
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(AuthenticationSchemes = AdminCookieAuth.Scheme, Roles = "Admin")]
    public class AdminDashboardApiController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;

        public AdminDashboardApiController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: /api/admin/dashboard/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            return Ok(stats);
        }
    }
}
