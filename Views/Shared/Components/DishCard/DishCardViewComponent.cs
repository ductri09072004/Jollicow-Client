using Jollicow.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jollicow.Views.Shared.Components.DishCard
{
    public class DishCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string id_categorygory, string id_dishes, string restaurant_id, string name, long price, string image, string status)
        {
            var model = new DishesModels
            {
                id_category = id_categorygory,
                id_dishes = id_dishes,
                restaurant_id = restaurant_id,
                name = name,
                price = price,
                image = image,
                status = status
            };
            return View(model);
        }
    }
}