using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Jollicow.Models;
using Newtonsoft.Json;

// Lớp request
public class DishRequest
{
    [JsonProperty("id_dishes")]
    public string id_dishes { get; set; } = String.Empty;
}
public class ToppingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ToppingService> _logger;

    public ToppingService(HttpClient httpClient, ILogger<ToppingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<ToppingModels>> GetToppings(string id_dishes)
    {
        var request = new DishRequest
        {
            id_dishes = id_dishes
        };
        _logger.LogInformation("Dishes ID: {@DishesId}", id_dishes);
        _logger.LogInformation("Request of GetToppings: {@Request}", request);

        var url = $"https://jollicowbe-admin.up.railway.app/api/admin/toppings/filter";
        var response = await _httpClient.PostAsJsonAsync(url, request);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Không tìm thấy topping cho món {0}", request.id_dishes);
            return new List<ToppingModels>();
        }
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Lấy topping thất bại: {response.StatusCode}");
        }
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<List<ToppingModels>>(content);
        return data ?? new List<ToppingModels>();
    }
}