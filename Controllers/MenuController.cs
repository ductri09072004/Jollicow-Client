using Microsoft.AspNetCore.Mvc;

namespace Jollicow.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Menu()
        {
            return View();
        }
    }
}
