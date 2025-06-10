using Jollicow.Models;

public class MenuViewModel
{
    public List<DishesModels> Dishes { get; set; }
    public List<CategoriesModel> Categories { get; set; }
    public string SelectedCategoryID { get; set; }

    public string IdTable { get; set; }
    public string RestaurantId { get; set; }
}