using System.ComponentModel;

namespace AgentFrameworkChat.AI.Tools;

public static class PizzaDeliveryTool
{
    
    [Description("Order a pizza")]
    public static string OrderPizza() => "A pizza is on its way!";
}