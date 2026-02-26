using AgentFrameworkChat.Enums;
using Microsoft.Extensions.AI;

namespace AgentFrameworkChat.AI.Tools;

public interface IApprovalRequiredAiTool
{
    public string Name { get;  }

    public Task<string> BuildApprovalRequestAsync(Guid conversationId, FunctionCallContent functionCallContent,
        Language language, CancellationToken cancellationToken);
}