using System.ComponentModel.DataAnnotations;

namespace Jollicow.Models
{
    public class CategoriesModel
    {
        public string id_category { get; set; } = String.Empty;
        public string id_restaurant { get; set; } = String.Empty;
        public string name { get; set; } = String.Empty;
    }
}
