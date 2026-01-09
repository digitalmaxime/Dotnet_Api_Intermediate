using AgentFrameworkChat.AI.Agents;
using AgentFrameworkChat.AI.History;
using AgentFrameworkChat.Endpoints;
using AgentFrameworkChat.Extensions.OpenApi;
using AgentFrameworkChat.Features;
using AgentFrameworkChat.Options;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.private.json").Build();
builder.Services.AddOptions<AzureOpenAiOptions>().Bind(configuration.GetSection(AzureOpenAiOptions.SectionName));
builder.Services.AddOptions<PostgresOptions>().Bind(configuration.GetSection(PostgresOptions.SectionName));
builder.Services.AddScoped<IBasicAgent, BasicAgent>();
builder.Services.AddScoped<IThreadStore, ThreadStore>();
builder.Services.AddScoped<IAgentFactory, AgentFactory>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddHttpContextAccessor();


builder.Services.ConfigureOpenApi();

/* Dev UI OpenAi dependencies */
builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();
builder.Services.AddAIAgent("BasicChatAgent", (sp, name) =>
{
    var factory = sp.GetRequiredService<IAgentFactory>();
    return factory.CreateAgent();
});

var app = builder.Build();

app.UseOpenApiDocumentation();

app.UseHttpsRedirection();

app.MapConversationEndpoints();

/* Dev UI OpenAi dependencies */
app.MapOpenAIResponses();
app.MapOpenAIConversations();
if (app.Environment.IsDevelopment())
{
    app.MapDevUI();
}

app.Run();