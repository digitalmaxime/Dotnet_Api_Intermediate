using System.Text.Json;
using AgentFrameworkChat.Contracts.Repositories;
using AgentFrameworkChat.Models;

namespace Infrastructure.Repositories;

public class ConversationRepository: IConversationRepository
{
    public Task<List<ConversationModel>> GetUserConversations(string username, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ConversationModel?> GetConversation(Guid conversationId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> AddNewConversation(Guid conversationId, string username, string title, JsonElement state,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<JsonElement> GetConversationSession(Guid conversationId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RenameConversation(Guid conversationId, string newConversationName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task PinConversation(Guid conversationId, bool isPinned, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task ArchiveUserConversation(Guid conversationId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task PurgeConversationsOlderThan(Guid conversationId, int numberOfDays, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}