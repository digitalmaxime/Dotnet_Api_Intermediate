using AgentFrameworkChat.AI.Agents;
using Microsoft.Agents.AI.Hosting;

namespace AgentFrameworkChat.Extensions.AgentRegistration;

public static class AgentRegistrationExtension
{
    public static IServiceCollection RegisterAgents(this IServiceCollection services)
    {
        services.AddAIAgent("BasicChatAgent", (sp, name) =>
        {
            var factory = sp.GetRequiredService<IAgentFactory>();
            return factory.CreateAgent();
        });
// builder.AddWorkflow("BasicWorkflow", (sp, name) =>
// {
//     return new Workflow();
// });
        return services;
    }
}