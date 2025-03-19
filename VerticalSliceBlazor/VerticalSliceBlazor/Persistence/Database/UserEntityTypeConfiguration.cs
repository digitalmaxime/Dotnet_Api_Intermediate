using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VerticalSliceBlazor.Persistence.Database;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Password)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(50);
    }
}