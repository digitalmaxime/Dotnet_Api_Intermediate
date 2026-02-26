using System.Diagnostics.CodeAnalysis;
using AgentFrameworkChat.AI.Tools;
using AgentFrameworkChat.Enums;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentFrameworkChat.Services.HumanInTheLoop;

[Experimental("MEAI001")]
public class HumanInTheLoopService(IEnumerable<IApprovalRequiredAiTool> approvalRequiredAiTools)
    : IHumanInTheLoopService
{
    public async Task<string> ApprovalFunctionRequest(AgentResponse agentResponse, Guid conversationId,
        Language language,
        CancellationToken cancellationToken = default)
    {
        var functionApprovalRequestContent = agentResponse.Messages
                                                 .SelectMany(x => x.Contents)
                                                 .OfType<FunctionApprovalRequestContent>()
                                                 .LastOrDefault()
                                             ?? throw new ArgumentException(
                                                 "Function approval request detected but no FunctionApprovalRequestContent found");

        var functionCallContent = functionApprovalRequestContent.FunctionCall ??
                                  throw new ArgumentException(
                                      "Function approval request detected but no FUnctionCallContent was found");

        var aiTool = approvalRequiredAiTools.First(x => x.Name == functionCallContent.Name);
        
        return await aiTool.BuildApprovalRequestAsync(conversationId, functionCallContent, language, cancellationToken);
    }

    public bool IsFunctionApprovalRequest(AgentResponse agentResponse)
    {
        return agentResponse.Messages.Last().Contents.OfType<FunctionApprovalRequestContent>().Any();
    }
}