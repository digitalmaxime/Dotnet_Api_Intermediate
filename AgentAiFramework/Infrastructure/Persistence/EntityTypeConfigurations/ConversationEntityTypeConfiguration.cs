using System.Runtime.CompilerServices;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityTypeConfigurations;

public class ConversationEntityTypeConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(x => x.ConversationId);

        builder.Property(x => x.ConversationId).HasColumnName("id");
        builder.Property(x => x.SerializedState).HasColumnName("serialized_state").HasColumnType("jsonb");
        builder.Property(x => x.Title).HasColumnName("title").HasColumnType("text");
        builder.Property(x => x.Username).HasColumnName("username").HasColumnType("text");
        builder.Property(x => x.IsPinned).HasColumnName("is_pinned");
        builder.Property(x => x.IsArchived).HasColumnName("is_archived");
        builder.Property(x => x.CreationDate).HasColumnName("creation_date");
        builder.Property(x => x.ModificationDate).HasColumnName("modification_date");

        builder.HasIndex(m => new { m.Username, m.IsArchived, m.CreationDate });

        builder.ToTable("conversations",
            tableBuilder => tableBuilder.HasCheckConstraint("CK_Conversation_Username_Lowercase", "username = LOWER(username)"));
    }
}