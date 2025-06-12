using Firebase.Database;
using Firebase.Database.Query;
using System.Threading.Tasks;
using Jollicow.Models;

public class FirebaseService
{
    private readonly FirebaseClient _client;

    public FirebaseService()
    {
        _client = new FirebaseClient("https://jollicow-e6724-default-rtdb.firebaseio.com/",
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult("jaD69f0MHzQpqulFNwun3xtHeZPbwZhDxasC9VcU")
            });
    }


    // Dishes

    public async Task AddDishAsync(string name, decimal price, string image, string status)
    {
        await _client
            .Child("dishes")
            .PostAsync(new
            {
                Name = name,
                Price = price,
                Image = image,
                Status = status
            });
    }

    public void ListenForDishes(Action<DishesModels> onData)
    {
        _client
            .Child("Menus")
            .AsObservable<DishesModels>()
            .Subscribe(d => onData(d.Object));
    }

    public async Task<List<DishesModels>> GetAllDishesAsync()
    {
        var dishes = await _client
            .Child("Menus")
            .OnceAsync<DishesModels>();

        return dishes.Select(d => d.Object).ToList();
    }

    public async Task<List<DishesModels>> GetDishesByCategoryAsync(string categoryId)
    {
        var dishes = await _client
            .Child("Menus")
            .OrderBy("id_category")
            .EqualTo(categoryId)
            .OnceAsync<DishesModels>();

        return dishes.Select(d => d.Object).ToList();
    }

    public async Task<DishesModels?> GetDishByIdAsync(string id)
    {
        var dishes = await _client
            .Child("Menus")
            .OnceAsync<DishesModels>();

        return dishes
            .Select(item => item.Object)
            .FirstOrDefault(d => d.id_dishes == id);
    }


    // Category

    public async Task<List<CategoriesModel>> GetAllCategoriesAsync()
    {
        var categories = await _client
            .Child("Categories")
            .OnceAsync<CategoriesModel>();

        return categories.Select(c => c.Object).ToList();
    }
}