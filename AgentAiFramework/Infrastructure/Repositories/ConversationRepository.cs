using System.Text.Json;
using AgentFrameworkChat.Contracts.Repositories;
using AgentFrameworkChat.Models;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ConversationRepository(ChatDbContext context, TimeProvider timeProvider) : IConversationRepository
{
    public async Task<List<ConversationModel>> GetUserConversations(string username,
        CancellationToken cancellationToken = default)
    {
        username = username.ToLowerInvariant();
        var filteredConverations = await context.Conversations
            .AsNoTracking()
            .Where(c => c.Username == username && !c.IsArchived)
            .OrderBy(c => c.CreationDate)
            .Select(c => new ConversationModel()
            {
                Id = c.ConversationId,
                Title = c.Title,
                Username = c.Username,
                IsPinned = c.IsPinned,
                CreationDate = c.CreationDate,
                ModificationDate = c.ModificationDate,
            }).ToListAsync(cancellationToken);

        return filteredConverations;
    }

    public async Task<ConversationModel?> GetConversation(Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var conversation = await context.Conversations
            .AsNoTracking()
            .Select(c => new ConversationModel()
            {
                Id = c.ConversationId,
                Title = c.Title,
                Username = c.Username,
                IsPinned = c.IsPinned,
                CreationDate = c.CreationDate,
                ModificationDate = c.ModificationDate,
            })
            .FirstOrDefaultAsync(c => c.Id == conversationId, cancellationToken);

        return conversation;
    }

    public async Task<Guid> AddNewConversation(Guid conversationId, string username, string title, JsonElement state,
        CancellationToken cancellationToken = default)
    {
        var conversation = Conversation.CreateNew(conversationId, username, title, state, timeProvider.GetUtcNow());
        await context.Conversations.AddAsync(conversation, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return conversation.ConversationId;
    }

    public async Task<JsonElement> GetConversationSession(Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var existingConversation = await context.Conversations
            .AsNoTracking()
            .Select(c => new { c.ConversationId, c.SerializedState })
            .FirstOrDefaultAsync(c => c.ConversationId == conversationId, cancellationToken);

        return existingConversation is null
            ? throw new KeyNotFoundException($"Conversation with id {conversationId} not found")
            : existingConversation.SerializedState.Clone();
    }

    public async Task RenameConversation(Guid conversationId, string newConversationName,
        CancellationToken cancellationToken = default)
    {
        var existingConversation = await context.Conversations
            .FirstAsync(c => c.ConversationId == conversationId, cancellationToken);

        existingConversation.Title = newConversationName;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task PinConversation(Guid conversationId, bool isPinned, CancellationToken cancellationToken = default)
    {
        var existingConversation = await context.Conversations
            .FirstAsync(c => c.ConversationId == conversationId, cancellationToken);

        existingConversation.IsPinned = isPinned;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task ArchiveUserConversation(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var existingConversation = await context.Conversations
            .FirstAsync(c => c.ConversationId == conversationId, cancellationToken);

        existingConversation.IsArchived = true;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> PurgeConversationsOlderThan(Guid conversationId, int numberOfDays,
        CancellationToken cancellationToken = default)
    {
        var cutoffDate = timeProvider.GetUtcNow().AddDays(-1);
        var conversationsToDelete = context.Conversations
            .Where(c => !c.Messages.Any(m => m.Timestamp > cutoffDate) ||
                        c.CreationDate < timeProvider.GetUtcNow().AddDays(-7));
        
        context.Conversations.RemoveRange(conversationsToDelete);

        return await context.SaveChangesAsync(cancellationToken);
    }
}