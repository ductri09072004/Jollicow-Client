using Microsoft.AspNetCore.Mvc;
using Jollicow.Services;

namespace Jollicow.Controllers
{
    public class PaymentController : Controller
    {
        private readonly TokenService _tokenService;

        public PaymentController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        // GET: /Payment/VietQR?acsc=ENCRYPTED_TOKEN
        [HttpGet]
        public async Task<IActionResult> VietQR(string acsc)
        {
            // Lấy thông tin từ mã hóa (giống như CartController và MenuController)
            var decrypted = _tokenService.TryDecrypt(acsc);
            if (decrypted == null)
            {
                ViewBag.Error = "Link không hợp lệ hoặc đã hết hạn.";
                return View("Error");
            }

            var (idTable, restaurantId) = decrypted.Value;

            if (string.IsNullOrEmpty(idTable) || string.IsNullOrEmpty(restaurantId))
            {
                return BadRequest("Thiếu thông tin.");
            }

            // Lấy tổng tiền từ giỏ hàng
            var cartTotal = await CartAPIHelper.GetCartTotalAsync(idTable, restaurantId);

            ViewData["IdTable"] = idTable;
            ViewData["RestaurantId"] = restaurantId;
            ViewData["Amount"] = cartTotal;
            ViewData["AccountName"] = "Jollicow Restaurant";
            ViewData["AccountNumber"] = "0389105492";
            ViewData["Bank"] = "MB";
            ViewData["Description"] = $"Table {idTable} - Restaurant {restaurantId}";
            return View();
        }
    }
}
