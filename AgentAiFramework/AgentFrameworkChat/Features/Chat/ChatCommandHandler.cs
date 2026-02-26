using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using AgentFrameworkChat.AI.Agents;
using AgentFrameworkChat.AI.History;
using AgentFrameworkChat.Contracts.Repositories;
using AgentFrameworkChat.Extensions;
using AgentFrameworkChat.Services.HumanInTheLoop;
using MediatR;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentFrameworkChat.Features.Chat;

public class ChatCommandHandler(
    [FromKeyedServices(AgentFactory.AgentName)]
    AIAgent agent,
    IConversationRepository conversationRepository,
    IHumanInTheLoopService humanInTheLoopService,
    ILogger<ChatCommandHandler> logger
 
) : IRequestHandler<ChatCommandDto, ChatCommandResponseDto>
{
    public async Task<ChatCommandResponseDto> Handle(ChatCommandDto request, CancellationToken cancellationToken)
    {
        AgentRunOptions options = new()
        {
            AdditionalProperties = new AdditionalPropertiesDictionary()
            {
                ["language"] = request.Language
            }
        };

        var (session, conversationId) = request.ConversationId == null || request.ConversationId == Guid.Empty
            ? await CreateNewSessionAsync(request.Message, request.Username, cancellationToken)
            : await GetExistingSessionAsync((Guid)request.ConversationId, cancellationToken);

        AgentResponse agentResponse;

        try
        {
            agentResponse = await agent.RunAsync(request.Message, session, options, cancellationToken);
        }
        catch(Exception e)
        {
            logger.LogError(e, "Failed during AI run. Message : {Message}, CorrelationId : {CorrelationId}", request.Message, request.CorrelationId);
            throw;
        }

        string responseText;
        var isInError = false;

        try
        {
            responseText = humanInTheLoopService.IsFunctionApprovalRequest(agentResponse)
                ? await humanInTheLoopService.ApprovalFunctionRequest(agentResponse, conversationId, request.Language,
                    cancellationToken)
                : agentResponse.Text;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            responseText = e.Message;
            isInError = true;
        }

        return new ChatCommandResponseDto()
        {
            Role = agentResponse.Messages.Last().Role.ToString(),
            Type = agentResponse.Messages.Last().GetChatMessageType().ToString(),
            Content = responseText,
            Timestamp = agentResponse.CreatedAt ?? DateTimeOffset.Now,
            IsRequestInError = isInError
        };

    }
    
    [Experimental("MEAI001")]
    private async Task<(AgentSession, Guid)> CreateNewSessionAsync(string message, string username,
        CancellationToken cancellationToken)
    {
        var session = await agent.GetNewSessionAsync(cancellationToken);

        var serializedSession = session.Serialize();

        var messageStore = session.GetService<MyChatMessageStore>();

        if (messageStore == null)
        {
            throw new InvalidOperationException("Failed to get message store from new session");
        }

        var conversationId = messageStore.GetConversationKey();

        var title = message[..Math.Min(50, message.Length)];

        await conversationRepository.AddNewConversationAsync(conversationId, username, title, serializedSession,
            cancellationToken);

        return (session, conversationId);
    }

    private async Task<(AgentSession, Guid)> GetExistingSessionAsync(Guid conversationId, CancellationToken cancellationToken)
    {
        var savedState = await conversationRepository.GetConversationSessionAsync(conversationId, cancellationToken);
        
        var session = await agent.DeserializeSessionAsync(savedState, AgentAbstractionsJsonUtilities.DefaultOptions, cancellationToken);

        return (session, conversationId);
    }
}