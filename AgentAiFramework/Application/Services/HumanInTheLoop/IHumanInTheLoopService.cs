using Application.Enums;
using Microsoft.Agents.AI;

namespace Application.Services.HumanInTheLoop;

public interface IHumanInTheLoopService
{
    Task<string> ApprovalFunctionRequest(AgentResponse agentResponse, Guid conversationId, Language language, CancellationToken cancellationToken = default);

    bool IsFunctionApprovalRequest(AgentResponse agentResponse);
}