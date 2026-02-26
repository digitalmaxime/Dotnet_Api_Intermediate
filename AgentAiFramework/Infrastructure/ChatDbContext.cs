using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ChatDbContext(TimeProvider timeprovider, DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChatDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var now = timeprovider.GetUtcNow();
        foreach (var entity in ChangeTracker.Entries<Conversation>())
        {
            switch (entity.State)
            {
                case EntityState.Added:
                    entity.Property(c => c.CreationDate).CurrentValue = now;
                    entity.Property(c => c.ModificationDate).CurrentValue = now;
                    break;
                case EntityState.Modified:
                    entity.Property(c => c.ModificationDate).CurrentValue = now;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(entity.State));
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
}