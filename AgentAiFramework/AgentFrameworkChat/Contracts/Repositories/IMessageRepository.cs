using AgentFrameworkChat.Models;
using Microsoft.Extensions.AI;

namespace AgentFrameworkChat.Contracts.Repositories;

public interface IMessageRepository
{
    ValueTask AddMessagesAsync(IEnumerable<ChatMessage> messages, Guid conversationId,
        CancellationToken cancellationToken = default);

    Task<List<ChatMessage>> GetContextMessagesAsync(Guid conversationId, int take,
        CancellationToken cancellationToken = default);

    Task<List<ChatMessageModel>> GetFilteredChatMessagesAsync(Guid conversationId, int take,
        CancellationToken cancellationToken = default);

    Task UpdateMessageAsync(Guid messageId, string updateContent, CancellationToken cancellationToken);

    Task<Guid?> GetLatestMessageIdAsync(Guid conversationId, CancellationToken cancellationToken);

    Task<ChatMessage> GetLatestMessageAsync(Guid conversationId, CancellationToken cancellationToken = default);
}