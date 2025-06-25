namespace Jollicow.Models
{
    public class PaymentMethodModel
    {
        public string SelectedMethod { get; set; } = "";
        public string IdTable { get; set; } = "";
        public string RestaurantId { get; set; } = "";
    }
}