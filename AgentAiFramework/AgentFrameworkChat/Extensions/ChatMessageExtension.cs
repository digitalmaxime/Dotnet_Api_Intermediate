using AgentFrameworkChat.Enums;
using Microsoft.Extensions.AI;

namespace AgentFrameworkChat.Extensions;

#pragma warning disable MEAI001
public static class ChatMessageExtension
{
    public static MessageType GetChatMessageType(this ChatMessage message)
    {
        return message.Contents[0] switch
        {
            TextContent => MessageType.Text,
            TextReasoningContent => MessageType.Reasoning,
            FunctionCallContent => MessageType.FunctionCall,
            FunctionResultContent => MessageType.FunctionResult,
            FunctionApprovalRequestContent => MessageType.FunctionApprovalRequest,
            FunctionApprovalResponseContent => MessageType.FunctionApprovalResponse,
            ErrorContent => MessageType.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(message), message.Contents[0],
                "Unsupported chat message type")
        };
    }

    public static MessageRole ToRoleEnum(this ChatRole role)
    {
        var value = role.Value;
        
        return role.Value switch
        {
            "user" => MessageRole.User,
            "assistant" => MessageRole.Assistant,
            "tool" => MessageRole.Tool,
            "system" => MessageRole.System,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role.Value, $"Unsupported chat role: {role.Value}")
        };
    }
}