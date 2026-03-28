﻿using Application.AI.History;
using Application.AI.Tools;
using Application.Options;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Application.AI.Agents;

#pragma warning disable MEAI001
public static class AgentFactory
{
    public const string AgentName = "MainAgent";

    public static AIAgent CreateAgent(IServiceProvider serviceProvider, AzureOpenAiOptions azureOpenAiOptions)
    {
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var aiOptions = serviceProvider.GetRequiredService<IOptionsMonitor<AgentConfigurationOptions>>().Get(AgentName);
        
        var chatClient = new AzureOpenAIClient(
                new Uri(azureOpenAiOptions.Endpoint),
                new AzureKeyCredential(azureOpenAiOptions.ApiKey)
            ).GetChatClient(azureOpenAiOptions.DeploymentName)
            .AsIChatClient();

        var chatClientOptions = new ChatClientAgentOptions()
        {
            Name = AgentName,
            Description = "A chat agent that uses AI tools to assist users.",
            ChatOptions = new ChatOptions()
            {
                Instructions = string.Join(" ", aiOptions.LlmInstructions),
                Tools =
                [
                    AIFunctionFactory.Create(DateTimeTool.GetDateTime),
                    // AIFunctionFactory.Create(PizzaDeliveryTool.OrderPizza)
                    PizzaDeliveryTool.ApprovalRequiredReservationTool
                ]
            },
            ChatHistoryProviderFactory = (ctx, _) =>
            {
                var requestService = httpContextAccessor.HttpContext?.RequestServices
                                     ?? throw new InvalidOperationException("No request services found");

                ChatHistoryProvider historyProvider =
                    ActivatorUtilities.CreateInstance<MyChatMessageStore>(requestService, ctx.SerializedState);

                return ValueTask.FromResult(historyProvider);
            }
        };

        var agent = new ChatClientAgent(chatClient, chatClientOptions, services: serviceProvider)
            .AsBuilder()
            // .Use() // TODO: middleware
            .Build();

        return agent;
    }
}