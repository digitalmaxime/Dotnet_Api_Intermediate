using System.Diagnostics.CodeAnalysis;
using AgentFrameworkChat.AI.Agents;
using AgentFrameworkChat.Contracts.Repositories;
using AgentFrameworkChat.Enums;
using AgentFrameworkChat.Extensions;
using AgentFrameworkChat.Services.HumanInTheLoop;
using MediatR;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace AgentFrameworkChat.Features.ApprovePizzaOrder;

[Experimental("MEAI001")]
public class ApprovePizzaOrderHandler(
    [FromKeyedServices(AgentFactory.AgentName)]
    AIAgent agent,
    TimeProvider timeProvider,
    IConversationRepository conversationRepository,
    IMessageRepository messageRepository,
    ILogger logger
) : IRequestHandler<ApprovePizzaOrderRequestDto, ApprovePizzaOrderResponseDto>
{
    public async Task<ApprovePizzaOrderResponseDto> Handle(ApprovePizzaOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        AgentRunOptions options = new AgentRunOptions()
        {
            AdditionalProperties = new AdditionalPropertiesDictionary()
            {
                ["language"] = request.Language
            }
        };

        logger.LogInformation("{Username} {Approval} request for pizza order. ConversationId {ConversatinId}",
            request.Username, request.IsApproved ? "Approved" : "Rejected", request.ConversationId);

        ChatMessage approvalMessage =
            await CreateApprovalMessage(request.ConversationId, request.IsApproved, cancellationToken);

        if (!request.IsApproved)
        {
            return await HandleRejectionAsync(request.ConversationId, approvalMessage, request.Language,
                cancellationToken);
        }

        var conversationSession =
            await conversationRepository.GetConversationSession(request.ConversationId, cancellationToken);

        var session = await agent.DeserializeSessionAsync(conversationSession, cancellationToken: cancellationToken);

        var response = await agent.RunAsync(approvalMessage, session, options, cancellationToken);

        return new ApprovePizzaOrderResponseDto()
        {
            Role = response.Messages.Last().Role.ToString(),
            Type = response.Messages.Last().GetChatMessageType().ToString(),
            Content = response.Text,
            Timestamp = response.CreatedAt ?? timeProvider.GetUtcNow()
        };
    }

    private async Task<ChatMessage> CreateApprovalMessage(Guid conversationId, bool isApproved,
        CancellationToken cancellationToken)
    {
        var message = await messageRepository.GetLatestMessageAsync(conversationId, cancellationToken);

        if (message.Role != ChatRole.Assistant)
        {
            throw new InvalidOperationException(
                $"{nameof(CreateApprovalMessage)} - Last message found for conversation {conversationId} was not an assistant message.");
        }

        var approvalRequestContents = message.Contents.OfType<FunctionApprovalRequestContent>().ToArray();
        if (approvalRequestContents.Length == 0)
        {
            throw new InvalidOperationException(
                $"{nameof(CreateApprovalMessage)} - No function approval request content found for conversation {conversationId}.");
        }

        var approvalResponseContent = approvalRequestContents.Last().CreateResponse(isApproved);

        return new ChatMessage(ChatRole.User, [approvalResponseContent]);
    }

    private async Task<ApprovePizzaOrderResponseDto> HandleRejectionAsync(Guid conversationId,
        ChatMessage rejectionMessage, Language language, CancellationToken cancellationToken)
    {
        /*
         * Do not re-run the agent on rejection.
         * The LLM may ignore the rejection or interpret the rejection as a failure
         * and retry the process by re-emit a new functionApprovalRequest
         * Instead persist the full reject sequence and return a static response
         */

        var approvalResponseContent = rejectionMessage.Contents.OfType<FunctionApprovalResponseContent>().First();

        var toolResultMessage = new ChatMessage(ChatRole.Tool, [
            new FunctionResultContent(approvalResponseContent.FunctionCall.CallId, "Tool call invocation rejected")
        ]);

        var assistantAckMessage = new ChatMessage(ChatRole.Assistant,
            [new TextContent("The approval request was reject. I understand and respect the user's choice")]);

        await messageRepository.AddMessagesAsync([rejectionMessage, toolResultMessage, assistantAckMessage],
            conversationId, cancellationToken);

        var responseContent = language switch
        {
            Language.En => "The approvalrequest was rejected. Noactionwill take place.",
            Language.Fr => "Bien compris, la requete a ete rejetee. ",
            _ => throw new ArgumentOutOfRangeException(nameof(language))
        };

        return new ApprovePizzaOrderResponseDto()
        {
            Role = ChatMessageRole.Assistant.ToString(),
            Type = MessageType.Text.ToString(),
            Content = responseContent,
            Timestamp = DateTimeOffset.UtcNow
        };
    }
}