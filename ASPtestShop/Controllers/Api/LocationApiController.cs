using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace ASPtestShop.Controllers.Api
{
    [ApiController]
    [Route("api/location")]
    public class LocationApiController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly ILogger<LocationApiController> _logger;

        private const string BaseUrl = "https://provinces.open-api.vn/api";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

        public LocationApiController(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ILogger<LocationApiController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            const string cacheKey = "loc_provinces";

            if (_cache.TryGetValue(cacheKey, out object? cached))
            {
                return Content(cached!.ToString()!, "application/json");
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{BaseUrl}/?depth=1");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(502, new { message = "Không thể tải danh sách tỉnh/thành phố." });
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var provinces = doc.RootElement.EnumerateArray()
                    .Select(p => new
                    {
                        code = p.TryGetProperty("code", out var c) ? c.GetInt32() : 0,
                        name = p.TryGetProperty("name", out var n) ? n.GetString() : ""
                    })
                    .OrderBy(p => p.name)
                    .ToList();

                var result = JsonSerializer.Serialize(provinces);
                _cache.Set(cacheKey, result, CacheDuration);

                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Location] Lỗi khi tải danh sách tỉnh.");
                return StatusCode(502, new { message = "Lỗi kết nối đến nguồn dữ liệu địa giới." });
            }
        }

        [HttpGet("districts/{provinceCode:int}")]
        public async Task<IActionResult> GetDistricts(int provinceCode)
        {
            if (provinceCode <= 0)
                return BadRequest(new { message = "Mã tỉnh không hợp lệ." });

            var cacheKey = $"loc_districts_{provinceCode}";

            if (_cache.TryGetValue(cacheKey, out object? cached))
            {
                return Content(cached!.ToString()!, "application/json");
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{BaseUrl}/p/{provinceCode}?depth=2");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(502, new { message = "Không thể tải danh sách quận/huyện." });
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("districts", out var districtsEl))
                {
                    return Ok(new List<object>());
                }

                var districts = districtsEl.EnumerateArray()
                    .Select(d => new
                    {
                        code = d.TryGetProperty("code", out var c) ? c.GetInt32() : 0,
                        name = d.TryGetProperty("name", out var n) ? n.GetString() : ""
                    })
                    .OrderBy(d => d.name)
                    .ToList();

                var result = JsonSerializer.Serialize(districts);
                _cache.Set(cacheKey, result, CacheDuration);

                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Location] Lỗi khi tải quận/huyện cho tỉnh {ProvinceCode}.", provinceCode);
                return StatusCode(502, new { message = "Lỗi kết nối đến nguồn dữ liệu địa giới." });
            }
        }

        [HttpGet("wards/{districtCode:int}")]
        public async Task<IActionResult> GetWards(int districtCode)
        {
            if (districtCode <= 0)
                return BadRequest(new { message = "Mã huyện không hợp lệ." });

            var cacheKey = $"loc_wards_{districtCode}";

            if (_cache.TryGetValue(cacheKey, out object? cached))
            {
                return Content(cached!.ToString()!, "application/json");
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{BaseUrl}/d/{districtCode}?depth=2");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(502, new { message = "Không thể tải danh sách phường/xã." });
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("wards", out var wardsEl))
                {
                    return Ok(new List<object>());
                }

                var wards = wardsEl.EnumerateArray()
                    .Select(w => new
                    {
                        code = w.TryGetProperty("code", out var c) ? c.GetInt32() : 0,
                        name = w.TryGetProperty("name", out var n) ? n.GetString() : ""
                    })
                    .OrderBy(w => w.name)
                    .ToList();

                var result = JsonSerializer.Serialize(wards);
                _cache.Set(cacheKey, result, CacheDuration);

                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Location] Lỗi khi tải phường/xã cho huyện {DistrictCode}.", districtCode);
                return StatusCode(502, new { message = "Lỗi kết nối đến nguồn dữ liệu địa giới." });
            }
        }
    }
}
