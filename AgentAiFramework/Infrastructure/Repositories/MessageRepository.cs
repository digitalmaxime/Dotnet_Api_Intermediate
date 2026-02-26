using AgentFrameworkChat.Contracts.Repositories;
using AgentFrameworkChat.Models;
using Microsoft.Extensions.AI;

namespace Infrastructure.Repositories;

public class MessageRepository: IMessageRepository
{
    public ValueTask AddMessagesAsync(IEnumerable<ChatMessage> messages, Guid conversationId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ChatMessage>> GetContextMessagesAsync(Guid conversationId, int take, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<MessageModel>> GetFilteredChatMessagesAsync(Guid conversationId, int take, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateMessageAsync(Guid messageId, string updateContent, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Guid?> GetLatestMessageIdAsync(Guid conversationId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ChatMessage> GetLatestMessageAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}