using Jollicow.Models;

public class CartAPIHelper
{
    private static readonly HttpClient _httpClient = new HttpClient();

    // Call API to add to cart
    public static async Task<bool> AddCartAsync(CartModel cart)
    {
        var response = await _httpClient.PostAsJsonAsync("https://jollicowfe-production.up.railway.app/api/carts/create", cart);
        Console.WriteLine(response.Content.ReadAsStringAsync());
        return response.IsSuccessStatusCode;
    }
}