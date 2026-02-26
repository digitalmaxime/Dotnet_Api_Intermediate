using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using AgentFrameworkChat.Contracts.Repositories;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentFrameworkChat.AI.History;

[Experimental("MEAI001")]
public class MyChatMessageStore(
    JsonElement serializedStoreState,
    string connectionString,
    IMessageRepository messageRepository)
    : ChatHistoryProvider
{
    private Guid? ConversationKey { get; set; }
    private const int MaxHistorySize = 100;

    public Guid GetConversationKey()
    {
        if (ConversationKey is not null)
        {
            return ConversationKey.Value;
        }

        if (serializedStoreState.ValueKind == JsonValueKind.String)
        {
            var conversationIdString = serializedStoreState.GetString();
            if (!Guid.TryParse(conversationIdString, out var conversationId) || conversationId == Guid.Empty)
            {
                throw new ArgumentException($"Invalid conversation key '{conversationIdString}");
            }

            ConversationKey = conversationId;
        }
        else
        {
            ConversationKey = Guid.CreateVersion7();
        }

        return ConversationKey.Value;
    }

    public override async ValueTask<IEnumerable<ChatMessage>> InvokingAsync(InvokingContext context,
        CancellationToken cancellationToken = new())
    {
        return await messageRepository.GetContextMessagesAsync(GetConversationKey(), MaxHistorySize, cancellationToken);
    }

    public override async ValueTask InvokedAsync(InvokedContext context,
        CancellationToken cancellationToken = new())
    {
        var requestMessages = context.RequestMessages.Where(m => m.Role != ChatRole.System);
        await messageRepository.AddMessagesAsync(requestMessages, GetConversationKey(), cancellationToken);
        if (context.ResponseMessages != null)
        {
            await messageRepository.AddMessagesAsync(context.ResponseMessages, GetConversationKey(), cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("No response messages returned from AI");
        }
    }

    public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null) =>
        JsonSerializer.SerializeToElement(GetConversationKey(), AgentAbstractionsJsonUtilities.DefaultOptions);
}