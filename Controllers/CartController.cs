using Microsoft.AspNetCore.Mvc;

namespace Jollicow.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;

        public CartController(ILogger<CartController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/cart/cartdetail")]
        public IActionResult CartDetail(string id_table, string restaurant_id)
        {
            if (string.IsNullOrEmpty(id_table) || string.IsNullOrEmpty(restaurant_id))
            {
                _logger.LogWarning("Missing parameters: id_table={IdTable}, restaurant_id={IdRestaurant}", id_table, restaurant_id);
                return BadRequest("Thiếu thông tin.");
            }

            ViewData["IdTable"] = id_table;
            ViewData["RestaurantId"] = restaurant_id;
            return View();
        }
    }
}