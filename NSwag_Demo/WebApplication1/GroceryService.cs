using Microsoft.EntityFrameworkCore;

namespace WebApplication1;

public class GroceryService : IGroceryService
{
    private readonly FoodContext _foodContext;

    public GroceryService(FoodContext foodContext)
    {
        _foodContext = foodContext;
    }
    // private readonly List<Food> _foods = new()
    // {
    //     new Food()
    //     {
    //         Name = "Carrot", Quantity = 2
    //     },
    //     new Food()
    //     {
    //         Name = "Tofu", Quantity = 1
    //     }
    // };
    public async Task<ICollection<Food>> GetAllFoods()
    {
        return await _foodContext.Foods.ToArrayAsync();
    }

    public async Task<Food?> GetFoodByName(string name)
    {
        return await _foodContext.Foods.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<ICollection<Food>> AddFood(Food food)
    {
        var too = _foodContext.Add(food);
        await _foodContext.SaveChangesAsync();
        return await _foodContext.Foods.ToArrayAsync();
    }
}