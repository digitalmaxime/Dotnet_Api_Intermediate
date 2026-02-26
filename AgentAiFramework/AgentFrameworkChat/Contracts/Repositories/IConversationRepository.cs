using System.Text.Json;
using AgentFrameworkChat.Models;

namespace AgentFrameworkChat.Contracts.Repositories;

public interface IConversationRepository
{
    Task<List<ConversationModel>> GetUserConversations(string username, CancellationToken cancellationToken = default);

    Task<ConversationModel?> GetConversation(Guid conversationId, CancellationToken cancellationToken = default);
    
    Task<Guid> AddNewConversationAsync(Guid conversationId, string username, string title, JsonElement state, CancellationToken cancellationToken = default);

    Task<JsonElement> GetConversationSessionAsync(Guid conversationId, CancellationToken cancellationToken = default);

    Task RenameConversation(Guid conversationId, string newConversationName,
        CancellationToken cancellationToken = default);

    Task PinConversation(Guid conversationId, bool isPinned, CancellationToken cancellationToken = default);
    
    Task ArchiveUserConversation(Guid conversationId, CancellationToken cancellationToken = default);
    
    Task PurgeConversationsOlderThanAsync(Guid conversationId, int numberOfDays, CancellationToken cancellationToken = default);
}