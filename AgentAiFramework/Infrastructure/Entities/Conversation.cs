using System.Text.Json;

namespace Infrastructure.Entities;

public class Conversation
{
    public required Guid ConversationId { get; init; }
    
    public required JsonElement SerializedState { get; set; }
    
    public required string Title { get; set; }
    
    public required string Username { get; init; }
    
    public bool IsPinned { get; set; }
    
    public bool IsArchived { get; set; }
    
    public required DateTimeOffset CreationDate { get; init; }
    
    public DateTimeOffset? ModificationDate { get; set; }

    public ICollection<Message> Messages { get; init; } = [];

    public static Conversation CreateNew(Guid conversationId, string username, string title, JsonElement state,
        DateTimeOffset createdAt)
    {
        return new Conversation()
        {
            ConversationId = conversationId,
            SerializedState = state,
            Username = username.ToLowerInvariant(),
            Title = title,
            IsPinned = false,
            IsArchived = false,
            CreationDate = createdAt,
            ModificationDate = createdAt
        };
    }

}