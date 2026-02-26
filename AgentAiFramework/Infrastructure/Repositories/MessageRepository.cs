using System.Text.Json;
using AgentFrameworkChat.Contracts.Repositories;
using AgentFrameworkChat.Enums;
using AgentFrameworkChat.Extensions;
using AgentFrameworkChat.Models;
using Infrastructure.Entities;
using Microsoft.Agents.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;

namespace Infrastructure.Repositories;

public class MessageRepository(ChatDbContext context, TimeProvider timeProvider) : IMessageRepository
{
    public async ValueTask AddMessagesAsync(IEnumerable<ChatMessage> messages, Guid conversationId, CancellationToken cancellationToken = default)
    {
        await context.Messages.AddRangeAsync(messages.Select(x => new Message()
        {
            MessageId = Guid.CreateVersion7(),
            Timestamp = timeProvider.GetUtcNow(),
            ConversationId = conversationId,
            SerializedMessage = JsonSerializer.Serialize(x, AgentAbstractionsJsonUtilities.DefaultOptions),
            MessageText = x.Text,
            Role = x.Role.ToRoleEnum(),
            Type = x.GetChatMessageType()
        }));

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ChatMessage>> GetContextMessagesAsync(Guid conversationId, int take, CancellationToken cancellationToken = default)
    {
        var filteredMessages = await context.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.Timestamp)
            .Take(take)
            .ToListAsync(cancellationToken);

        var jsonSerializerOptions = AgentAbstractionsJsonUtilities.DefaultOptions;
        var messages = filteredMessages.Select(x =>
                JsonSerializer.Deserialize<ChatMessage>(x.SerializedMessage, jsonSerializerOptions)
                ?? throw new InvalidOperationException($"Failed to deserialize chat message {x.SerializedMessage}"))
            .ToList();

        return messages;
    }

    public async Task<List<MessageModel>> GetFilteredChatMessagesAsync(Guid conversationId, int take, CancellationToken cancellationToken = default)
    {
        var filteredMessages = await context.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .Where(m => 
                m.Role == MessageRole.User ||
                (m.Role == MessageRole.Assistant && 
                    (m.Type == MessageType.Text || m.Type == MessageType.FunctionApprovalRequest)))
            .OrderBy(m => m.Timestamp)
            .Take(take)
            .Select(x => new MessageModel()
            {
                Content = x.MessageText,
                Role = x.Role,
                Type = x.Type,
                Timestamp = x.Timestamp,
            }).ToListAsync(cancellationToken);

        return filteredMessages;
    }

    public async Task UpdateMessageAsync(Guid messageId, string updateContent, CancellationToken cancellationToken)
    {
        var message = await context.Messages.FirstOrDefaultAsync(m => m.MessageId == messageId, cancellationToken);
        if (message != null)
        {
            message.MessageText = updateContent;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<Guid?> GetLatestMessageIdAsync(Guid conversationId, CancellationToken cancellationToken)
    {
        var latestMessage = await context.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);

        return latestMessage?.MessageId;
    }

    public async Task<ChatMessage> GetLatestMessageAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var latestMessage = await context.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);

        if (latestMessage == null)
        {
            throw new InvalidOperationException($"No messages found for conversation {conversationId}");
        }
        
        var jsonSerializerOptions = AgentAbstractionsJsonUtilities.DefaultOptions;
        var messages = JsonSerializer.Deserialize<ChatMessage>(latestMessage.SerializedMessage, jsonSerializerOptions)
                       ?? throw new InvalidOperationException($"Failed to deserialize chat message {latestMessage.SerializedMessage}");

        return messages;
    }
}