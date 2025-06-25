using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Jollicow.Services;

namespace Jollicow.Controllers
{
    public class AuthModels
    {
        public bool exists { get; set; }
    }

    public class MenuController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MenuController> _logger;
        private readonly IMemoryCache _cache;
        private readonly TokenService _tokenService;
        private const string CATEGORIES_CACHE_KEY = "all_categories";
        private const int CACHE_DURATION_MINUTES = 30;

        public MenuController(
            IHttpClientFactory httpClientFactory,
            ILogger<MenuController> logger,
            TokenService tokenService,
            IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _cache = cache;
            _tokenService = tokenService;
        }

        // Đường dẫn tạo mã hóa
        [HttpGet("/menu/generate")]
        public IActionResult GenerateToken(string id_table, string restaurant_id)
        {
            if (string.IsNullOrEmpty(id_table) || string.IsNullOrEmpty(restaurant_id))
            {
                return BadRequest("Thiếu thông tin.");
            }

            var token = _tokenService.GenerateToken(id_table, restaurant_id);
            var url = Url.Action("Menu", new { acsc = token }); // Access code
            return Content($"Link access menu: <a href='{url}'>{url}</a>", "text/html");
        }

        [HttpGet("/menu")]
        public async Task<IActionResult> Menu(string acsc, string id_category)
        {
            var decrypted = _tokenService.TryDecrypt(acsc);
            if (decrypted == null)
            {
                ViewBag.Error = "Link không hợp lệ hoặc đã hết hạn.";
                return View("Error");
            }

            // Lấy thông tin từ mã hóa

            var (id_table, restaurant_id) = decrypted.Value;

            // Kiểm tra session
            var sessionKey = $"auth_{id_table}_{restaurant_id}";

            if (HttpContext.Session.GetString(sessionKey) == "true")
            {
                _logger.LogInformation("Session exists, load menu");
                return await RenderMenu(id_table, restaurant_id, id_category);
            }
            _logger.LogInformation("Session not exists, this is a first time! Check first!");

            // Gọi API
            var client = _httpClientFactory.CreateClient();
            var postData = new
            {
                id_table = id_table,
                restaurant_id = restaurant_id
            };

            var response = await client.PostAsJsonAsync("https://jollicowfe-production.up.railway.app/api/tables/checkauth", postData);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthModels>();
                if (result?.exists == true)
                {
                    HttpContext.Session.SetString("id_table", id_table);
                    HttpContext.Session.SetString("restaurant_id", restaurant_id);
                    HttpContext.Session.SetString(sessionKey, "true");
                    _logger.LogInformation("✅ Session đã gán: {SessionKey}", sessionKey);
                    _logger.LogInformation("Session Keys: " + string.Join(", ", HttpContext.Session.Keys));

                    return await RenderMenu(id_table, restaurant_id, id_category);
                }
            }
            ViewBag.Error = "Không thể xác thực bàn/người dùng.";
            return View("Error");
        }

        private async Task<IActionResult> RenderMenu(string id_table, string restaurant_id, string id_category)
        {
            var firebaseService = new FirebaseService();

            var allCategories = await _cache.GetOrCreateAsync(CATEGORIES_CACHE_KEY, async entry =>
             {
                 entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_DURATION_MINUTES);
                 return await firebaseService.GetCategoriesByRestaurantIdAsync(restaurant_id);
             });

            if (string.IsNullOrEmpty(id_category) && allCategories.Any())
            {
                id_category = allCategories.First().id_category;
            }

            var dishes = await firebaseService.GetAllDishesAsync();
            var categories = await firebaseService.GetAllCategoriesAsync();

            _logger.LogInformation("Dishes: {@Dishes}, restaurant_id: {@RestaurantId}", dishes, restaurant_id);
            dishes = dishes.Where(d => d.id_category == id_category && d.restaurant_id == restaurant_id).ToList();



            var menuViewModel = new MenuViewModel
            {
                Dishes = dishes,
                Categories = allCategories,
                SelectedCategoryID = id_category,
                RestaurantId = restaurant_id,
                IdTable = id_table,
            };

            ViewData["IdTable"] = id_table;
            ViewData["RestaurantId"] = restaurant_id;

            return View("Menu", menuViewModel);
        }

        [HttpGet("/menu/dish")]
        public async Task<IActionResult> DishDetail(string acsc, string id_dishes)
        {
            // Lấy thông tin từ mã hóa
            var decrypted = _tokenService.TryDecrypt(acsc);
            if (decrypted == null)
            {
                ViewBag.Error = "Link không hợp lệ hoặc đã hết hạn.";
                return View("Error");
            }

            var (id_table, restaurant_id) = decrypted.Value;


            // Xử lý đường dẫn
            if (string.IsNullOrEmpty(id_table) || string.IsNullOrEmpty(restaurant_id) || string.IsNullOrEmpty(id_dishes))
            {
                _logger.LogWarning("Missing parameters: id_table={IdTable}, restaurant_id={IdRestaurant}, id_dishes={IdDishes}", id_table, restaurant_id, id_dishes);
                return BadRequest("Thiếu thông tin.");
            }

            var firebaseService = new FirebaseService();
            var dish = await firebaseService.GetDishByIdAsync(id_dishes);

            _logger.LogInformation("Dish found: {@Dish}", dish);

            if (dish == null)
            {
                return NotFound();
            }

            ViewData["IdTable"] = id_table;
            ViewData["RestaurantId"] = restaurant_id;
            return View(dish);
        }
    }
}
