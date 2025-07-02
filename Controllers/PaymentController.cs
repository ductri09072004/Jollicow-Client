using Microsoft.AspNetCore.Mvc;
using Jollicow.Services;
using Jollicow.Models;

namespace Jollicow.Controllers
{
    public class PaymentController : Controller
    {
        private readonly TokenService _tokenService;
        private readonly OrderService _orderService;
        private readonly ILogger<PaymentController> _logger;



        public PaymentController(TokenService tokenService, OrderService orderService, ILogger<PaymentController> logger)
        {
            _tokenService = tokenService;
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> VietQR(string acsc)
        {
            // Viết hàm get payment info từ db
            // Data tạm
            var paymentInfo = new PaymentInfo
            {
                AccountName = "Trần Hữu Minh Trí",
                AccountNumber = "0389105492",
                Bank = "MB Bank - Ngân Hàng Quân Đội",
            };
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
            ViewData["PaymentInfo"] = paymentInfo;
            ViewData["Description"] = $"Table {idTable} - Thanh toán hóa đơn";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string acsc)
        {

            try
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
                await _orderService.CreateOrder(idTable, restaurantId);
                TempData["Success"] = "Đặt thành công";
                _logger.LogInformation($"Đơn hàng đã được tạo thành công cho Bàn {idTable} tại Nhà hàng {restaurantId}.");

            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi đặt đơn: {ex.Message}";
            }

            return RedirectToAction("Confirmation", "Payment", new { acsc = acsc });
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
