using MyNamespace;
using WebApplication1;

namespace ConsoleApp1;

public class FoodService : IFoodService
{
    private readonly IMyClient _httpClient;

    public FoodService(IMyClient httpClient)
    {
        _httpClient = httpClient;
    }
// private readonly HttpClient _httpClient;
//
//     public FoodService(HttpClient httpClient)
//     {
//         _httpClient = httpClient;
//     }

    public async Task<string?> GetFoodAsync(string name)
    {
        try
        {
            // var res = await _httpClient.GetAsync($"food/api/food/{name}");
            // Console.WriteLine("Response : ");
            // var food = (await res.Content.ReadAsStringAsync());
            // Console.WriteLine(food);
            // return food;
            var res = await _httpClient.GetFoodAsync(name);
            return res.Name;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}

public interface IFoodService
{
    Task<string?> GetFoodAsync(string name);
}