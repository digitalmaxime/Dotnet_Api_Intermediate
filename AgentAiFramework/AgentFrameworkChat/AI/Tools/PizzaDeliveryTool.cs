using System.ComponentModel;
using AgentFrameworkChat.Features;

namespace AgentFrameworkChat.AI.Tools;

public static class PizzaDeliveryTool
{

    [Description("Order a pizza")]
    public static async Task<string> OrderPizza(IServiceProvider serviceProvider)
    {
        
        using var scope = serviceProvider.CreateScope();
        var testService = scope.ServiceProvider.GetRequiredService<IPizzaService>();
        var response = await testService.OrderPizza();
        return response;
    }
}