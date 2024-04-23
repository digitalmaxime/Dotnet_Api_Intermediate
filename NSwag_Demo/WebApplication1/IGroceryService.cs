namespace WebApplication1;

public interface IGroceryService
{
    Task<ICollection<Food>> GetAllFoods();
    Task<Food?> GetFoodByName(string name);
    Task<ICollection<Food>> AddFood(Food food);
}