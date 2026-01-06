using AgentFrameworkChat.AI.Agents;
using AgentFrameworkChat.Endpoints;
using AgentFrameworkChat.Extensions.OpenApi;
using AgentFrameworkChat.Options;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.private.json").Build();
builder.Services.AddOptions<AzureOpenAiOptions>().Bind(configuration.GetSection(AzureOpenAiOptions.SectionName));
builder.Services.AddOptions<PostgresOptions>().Bind(configuration.GetSection(PostgresOptions.SectionName));
builder.Services.AddScoped<IBasicAgent, BasicAgent>();

builder.Services.ConfigureOpenApi();

var app = builder.Build();

app.UseOpenApiDocumentation();

app.UseHttpsRedirection();

app.MapConversationEndpoints();

app.Run();