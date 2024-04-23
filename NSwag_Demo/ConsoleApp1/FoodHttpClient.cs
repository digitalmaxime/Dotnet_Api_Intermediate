using WebApplication1;

namespace ConsoleApp1;

public class FoodHttpHttpClient : IFoodHttpService
{
    private HttpClient _httpClient;

    public FoodHttpHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> GetFoodAsync(string name)
    {
        try
        {
            var res = await _httpClient.GetAsync($"food/api/food/{name}");
            Console.WriteLine("Response : ");
            var food = (await res.Content.ReadAsStringAsync());
            Console.WriteLine(food);
            return food;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}

public interface IFoodHttpService
{
    Task<string?> GetFoodAsync(string name);
}