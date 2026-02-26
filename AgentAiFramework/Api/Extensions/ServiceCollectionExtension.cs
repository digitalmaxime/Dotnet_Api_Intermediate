using System.Diagnostics.CodeAnalysis;
using AgentFrameworkChat.AI.Agents;
using AgentFrameworkChat.AI.Tools;
using AgentFrameworkChat.Features;
using AgentFrameworkChat.Options;
using AgentFrameworkChat.Services.HumanInTheLoop;
using Microsoft.Agents.AI.Hosting;

namespace AgentFrameworkChat.Extensions;

[Experimental("MEAI001")]
public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<AgentConfigurationOptions>(AgentFactory.AgentName).Bind(
            configuration.GetRequiredSection($"{AgentConfigurationOptions.SectionName}:{AgentFactory.AgentName}"));

        services.AddScoped<IHumanInTheLoopService, HumanInTheLoopService>();

        services.AddScoped<IPizzaService, PizzaService>();

        services.AddScoped<IApprovalRequiredAiTool, PizzaDeliveryTool>();

        services.AddMediatR(mediator =>
            mediator.RegisterServicesFromAssembly(typeof(ServiceCollectionContainerBuilderExtensions).Assembly));

        var openAiOptions = configuration.GetRequiredSection(AzureOpenAiOptions.SectionName).Get<AzureOpenAiOptions>();
        var postgresOptions = configuration.GetRequiredSection(PostgresOptions.SectionName).Get<PostgresOptions>();
        services.AddAIAgent(AgentFactory.AgentName,
            (sp, _) => AgentFactory.CreateAgent(sp, openAiOptions, postgresOptions));

        return services;
    }
}