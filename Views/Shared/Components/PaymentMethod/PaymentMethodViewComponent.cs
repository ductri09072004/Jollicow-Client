using Microsoft.AspNetCore.Mvc;
using Jollicow.Models;

namespace Jollicow.Views.Shared.Components.PaymentMethod
{
    public class PaymentMethodViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string selectedMethod = "", string idTable = "", string restaurantId = "")
        {
            var model = new PaymentMethodModel
            {
                SelectedMethod = selectedMethod,
                IdTable = idTable,
                RestaurantId = restaurantId
            };
            return View(model);
        }
    }
}