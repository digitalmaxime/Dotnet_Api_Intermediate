using GraphQL.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphQL.Persistence.EntityConfigurations;

public class CommandEntityConfiguration : IEntityTypeConfiguration<Command>
{
    public void Configure(EntityTypeBuilder<Command> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(100);
        builder.Property(c => c.CommandLine).HasMaxLength(100);
        // builder.HasOne<Platform>().WithMany(p => p.Commands).HasForeignKey(c => c.PlatformId);
    }
}