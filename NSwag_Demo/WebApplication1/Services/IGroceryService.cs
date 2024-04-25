using WebApplication1.Models;

namespace WebApplication1.Services;

public interface IGroceryService
{
    Task<ICollection<Food>> GetAllFoods();
    Task<Food?> GetFoodByName(string name);
    Task<ICollection<Food>> AddFood(Food food);
}