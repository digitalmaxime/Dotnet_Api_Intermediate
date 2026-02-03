using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentFrameworkChat.Features;

public class ChatServiceHelper
{
    [Experimental("MEAI001")]
    private async Task<string> ToChatMessageResponse(
        List<FunctionApprovalRequestContent> functionApprovalRequests, AgentRunResponse result, Guid conversationId,
        ChatClientAgentRunOptions executionSettings, ChatMessage message)
    {
        var response = new 
        {
            Messages = functionApprovalRequests.Count != 0
                ? functionApprovalRequests.Select(x => new ChatMessage(ChatRole.Assistant, [x.FunctionCall]))
                    .ToArray()
                : result.Messages.ToArray(),
            ConversationId = conversationId.ToString(),
            Metadata = new Dictionary<string, object>
            {
                ["temperature"] = 7,
                ["maxTokens"] = 100,
                ["processingTime"] = message.CreatedAt.HasValue
                    ? (DateTime.UtcNow - message.CreatedAt.Value.DateTime).TotalMilliseconds
                    : 0
            }
        };
        return JsonSerializer.Serialize(response);
    }
}