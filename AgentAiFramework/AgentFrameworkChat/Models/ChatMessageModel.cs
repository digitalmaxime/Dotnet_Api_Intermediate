using System.ComponentModel;
using AgentFrameworkChat.Enums;

namespace AgentFrameworkChat.Models;

public class ChatMessageModel
{
    [Description("The role of the user in the conversation. User or Assistant")]
    public required MessageRole Role { get; set; }
    
    [Description("The type of message. Text, Reasoning, FunctionCall, FunctionResult, FUnctionApprovalRequest, FunctionApprovalResponse, Error")]
    public required MessageType Type { get; set; }
    
    [Description("The content of the message. This can be a text message, a function call or a function result.")]
    public required string Content { get; set; }
    
    public DateTimeOffset? Timestamp { get; set; }
}