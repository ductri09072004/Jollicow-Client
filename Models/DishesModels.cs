using System.ComponentModel.DataAnnotations;

namespace Jollicow.Models
{
    public class DishesModels
    {
        public string id_dishes { get; set; } = String.Empty;
        public string id_cate { get; set; } = String.Empty;
        public string restaurant_id { get; set; } = String.Empty;
        public string image { get; set; } = String.Empty;
        public string name { get; set; } = String.Empty;
        public string status { get; set; } = String.Empty;


    }
}
