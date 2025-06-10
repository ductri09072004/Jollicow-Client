using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

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

        public MenuController(IHttpClientFactory httpClientFactory, ILogger<MenuController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet("/menu")]
        public async Task<IActionResult> Menu(string id_table, string restaurant_id, string id_category)
        {
            // Khai báo
            var firebaseService = new FirebaseService();
            var allCategories = await firebaseService.GetAllCategoriesAsync();

            // Xử lý đường dẫn
            if (string.IsNullOrEmpty(id_table) || string.IsNullOrEmpty(restaurant_id) || string.IsNullOrEmpty(id_category))
            {
                _logger.LogWarning("Missing parameters: id_table={IdTable}, restaurant_id={IdRestaurant}", id_table, restaurant_id);
                return BadRequest("Thiếu thông tin.");
            }

            // Mặc định lấy category đầu tiên
            if (string.IsNullOrEmpty(id_category) && allCategories.Any())
            {
                id_category = allCategories.First().id_category;
            }

            // Gọi API
            var client = _httpClientFactory.CreateClient();
            var postData = new
            {
                id_table = id_table,
                restaurant_id = restaurant_id
            };

            try
            {
                var response = await client.PostAsJsonAsync("https://jollicowfe-production.up.railway.app/api/tables/checkauth", postData);
                _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthModels>();
                    _logger.LogInformation("API Response Content: {Content}", JsonSerializer.Serialize(result));

                    if (result?.exists == true)
                    {
                        var dishes = await firebaseService.GetAllDishesAsync();
                        var categories = await firebaseService.GetAllCategoriesAsync();

                        if (!string.IsNullOrEmpty(id_category))
                        {
                            _logger.LogInformation("Cate IDs: {IDs}", string.Join(", ", dishes.Select(d => d.id_category)));
                            dishes = dishes.Where(d => d.id_category == id_category).ToList();
                        }

                        var menuViewModel = new MenuViewModel
                        {
                            Dishes = dishes,
                            Categories = categories,
                            SelectedCategoryID = id_category,
                            IdTable = id_table,
                            RestaurantId = restaurant_id
                        };

                        return View(menuViewModel);
                    }
                    else
                    {
                        ViewBag.Error = "Thông tin bàn hoặc nhà hàng không hợp lệ.";
                        return View("Error");
                    }
                }
                else
                {
                    _logger.LogError("API call failed with status code: {StatusCode}", response.StatusCode);
                    ViewBag.Error = "Không thể kết nối tới hệ thống.";
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking table authentication");
                ViewBag.Error = "Đã xảy ra lỗi khi kết nối tới hệ thống.";
                return View("Error");
            }
        }
    }
}
