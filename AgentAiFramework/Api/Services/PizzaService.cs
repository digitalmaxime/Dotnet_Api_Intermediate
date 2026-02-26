namespace AgentFrameworkChat.Features;

public interface IPizzaService
{
    Task<string> OrderPizza();
}

public class PizzaService() : IPizzaService
{
    public async Task<string> OrderPizza()
    {
        return "Pizza ordered!";
    }
}