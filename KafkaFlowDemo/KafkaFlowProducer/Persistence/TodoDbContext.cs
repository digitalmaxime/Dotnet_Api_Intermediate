using KafkaFlowDemo.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace KafkaFlowDemo.Persistence;

public class TodoDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
        });
    }
    
    public DbSet<Todo> Todos { get; set; }
}