namespace Jollicow.Models
{
    public class ToppingOptions
    {
        public string id_option { get; set; } = String.Empty;
        public string name { get; set; } = String.Empty;
        public string price { get; set; } = String.Empty;
    }

    public class ToppingModels
    {
        public string id_topping { get; set; } = String.Empty;
        public string id_dishes { get; set; } = String.Empty;
        public string name_details { get; set; } = String.Empty;
        public List<ToppingOptions> options { get; set; } = new List<ToppingOptions>();

        public ToppingType Type => GetCateFromName(name_details);

        private ToppingType GetCateFromName(string name_details)
        {
            return name_details.ToLowerInvariant() switch
            {
                "kích thước" => ToppingType.Size,
                "độ cay" => ToppingType.Spice,
                _ => ToppingType.Other,
            };
        }
    }

    // Chuẩn hóa data
    public enum ToppingType
    {
        Size,
        Spice,
        Other,
    }
}