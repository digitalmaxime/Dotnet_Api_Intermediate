using System.Diagnostics.CodeAnalysis;
using Application.AI.Agents;
using Application.AI.Tools;
using Application.Options;
using Application.Services;
using Application.Services.HumanInTheLoop;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions_Application;

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
            mediator.RegisterServicesFromAssembly(typeof(ServiceCollectionExtension).Assembly));

        var openAiOptions = configuration.GetRequiredSection(AzureOpenAiOptions.SectionName).Get<AzureOpenAiOptions>();
        services.AddAIAgent(AgentFactory.AgentName,
            (sp, _) => AgentFactory.CreateAgent(sp, openAiOptions, configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}