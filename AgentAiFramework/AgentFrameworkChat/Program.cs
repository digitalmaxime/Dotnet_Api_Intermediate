using AgentFrameworkChat.AI.Agents;
using AgentFrameworkChat.AI.History;
using AgentFrameworkChat.Endpoints;
using AgentFrameworkChat.Extensions.OpenApi;
using AgentFrameworkChat.Features;
using AgentFrameworkChat.Options;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.private.json").Build(); // TODO: Move to user secrets
builder.Services.AddScoped<IThreadStore, ThreadStore>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IPizzaService, PizzaService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.ConfigureOpenApi();

/* Dev UI OpenAi dependencies */
builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();
var azureOpenAiOptions = configuration.GetRequiredSection(AzureOpenAiOptions.SectionName).Get<AzureOpenAiOptions>();
var postgresOptions = configuration.GetRequiredSection(PostgresOptions.SectionName).Get<PostgresOptions>();
builder.Services.AddAIAgent("BasicChatAgent", (sp, name) => AgentFactory.CreateAgent(sp, azureOpenAiOptions, postgresOptions));

var app = builder.Build();

app.UseOpenApiDocumentation();

app.UseHttpsRedirection();

app.MapConversationEndpoints();

/* Dev UI OpenAi dependencies */
if (app.Environment.IsDevelopment())
{
    app.MapOpenAIResponses();
    app.MapOpenAIConversations();
    app.MapDevUI();
}

app.Run();