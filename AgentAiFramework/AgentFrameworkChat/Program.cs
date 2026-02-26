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

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.ConfigureOpenApi();

/* Dev UI OpenAi dependencies */
builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();

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