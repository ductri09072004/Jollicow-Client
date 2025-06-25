using System.ComponentModel.DataAnnotations;

namespace Jollicow.Models
{
    public class CartModel
    {
        public string id_dishes { get; set; } = String.Empty;
        public string id_restaurant { get; set; } = String.Empty;
        public string id_table { get; set; } = String.Empty;
        public List<string> toppings { get; set; } = new List<string>();
        public string name { get; set; } = String.Empty;
        public string image_url { get; set; } = String.Empty;
        public string note { get; set; } = String.Empty;
        public long price { get; set; }
        public int quantity { get; set; }
    }
}
