using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using AgentFrameworkChat.Enums;
using AgentFrameworkChat.Features;
using Microsoft.Extensions.AI;

namespace AgentFrameworkChat.AI.Tools;

    [Experimental("MEAI001")] 
public class PizzaDeliveryTool : IApprovalRequiredAiTool
{
    public string Name => ApprovalRequiredReservationTool.Name;

    public static readonly ApprovalRequiredAIFunction ApprovalRequiredReservationTool = new(AIFunctionFactory.Create(OrderPizza));

    
    [Description("Order a pizza")]
    private static async Task<string> OrderPizza(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var testService = scope.ServiceProvider.GetRequiredService<IPizzaService>();
        var response = await testService.OrderPizza();
        return response;
    }


    public Task<string> BuildApprovalRequestAsync(Guid conversationId, FunctionCallContent functionCallContent, Language language,
        CancellationToken cancellationToken)
    {
        return Task.FromResult("Are you sure you wanna order this pizza?");
    }
}