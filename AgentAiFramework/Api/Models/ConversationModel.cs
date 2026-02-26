namespace AgentFrameworkChat.Models;

public class ConversationModel
{
    public Guid Id { get; init; }
    public required string Title { get; set; }
    public required string Username { get; set;  }
    public bool IsPinned { get; set; }
    public DateTimeOffset? CreationDate { get; set; }
    public DateTimeOffset? ModificationDate { get; set; }
}