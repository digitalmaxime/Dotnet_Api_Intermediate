using AgentFrameworkChat.Endpoints;
using AgentFrameworkChat.Endpoints.Conversations;
using AgentFrameworkChat.Extensions.EndpointsExtension;
using AgentFrameworkChat.Extensions.OpenApi;
using Application.Extensions_Application;
using FluentValidation;
using Infrastructure.Extensions;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor()
    .AddAuthorization()
    .ConfigureOpenApiDocumentation()
    .AddValidatorsFromAssembly(typeof(Program).Assembly)
// services.AddTransient<ExceptionMiddleware>() // TODO:
    // services.ConfigureSecurity() // TODO:
    .AddSingleton(TimeProvider.System)
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

/* Dev UI OpenAi dependencies */
builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();

var application = builder.Build();

application
    .UseOpenApiDocumentation()
    .MapApiEndpoints()
    .UseHttpsRedirection();


/* Dev UI OpenAi dependencies */
if (application.Environment.IsDevelopment())
{
    application.MapOpenAIResponses();
    application.MapOpenAIConversations();
    application.MapDevUI();
}

application.Run();