using AgentFrameworkChat.Enums;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityTypeConfigurations;

public class MessageEntityTypeConfiguration: IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(x => x.MessageId);

        builder.Property(x => x.MessageId).HasColumnName("id");
        
        builder.Property(x => x.ConversationId).HasColumnName("conversation_id");
        
        builder.Property(x => x.Timestamp).HasColumnName("timestamp");

        builder.Property(x => x.SerializedMessage)
            .HasColumnName("serialized_message")
            .HasColumnType("json");

        builder.Property(x => x.Role)
            .HasColumnName("role")
            .HasColumnType("text")
            .HasConversion(x => x.ToString(), x => Enum.Parse<MessageRole>(x, true));
        
        builder.Property(x => x.Type)
            .HasColumnName("type")
            .HasColumnType("text")
            .HasConversion(x => x.ToString(), x => Enum.Parse<MessageType>(x, true));

        builder.Property(x => x.MessageText)
            .HasColumnName("message_text")
            .HasColumnType("text")
            .HasMaxLength(250);
        
        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => new { m.ConversationId, m.Timestamp });

        builder.ToTable("messages");
    }
}