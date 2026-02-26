using AgentFrameworkChat.Enums;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentFrameworkChat.AI.Middleware;

public class AgentRunMiddleware
{
    public static async Task<AgentResponse> LanguageAdapter(IEnumerable<ChatMessage> messages, AgentSession session,
        AgentRunOptions options, AIAgent innerAgent, CancellationToken cancellationToken)
    {
        if (options?.AdditionalProperties is null)
        {
            return await innerAgent.RunAsync(messages, session, options, cancellationToken);
        }

        var language = options.AdditionalProperties["language"]?.ToString();

        
        if (string.IsNullOrWhiteSpace(language) || !Enum.TryParse<Language>(language, out var languageEnum))
        {
            return await innerAgent.RunAsync(messages, session, options, cancellationToken);
        }

        var fullLanguage = languageEnum == Language.En ? "English" : "French";

        var localizedMessages =
            messages.Prepend(new ChatMessage(ChatRole.System, $"You must respond in {fullLanguage} language"));
        
        return await innerAgent.RunAsync(localizedMessages, session, options, cancellationToken);
    }
}