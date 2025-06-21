using KafkaFlowProducer.Entities;
using Microsoft.EntityFrameworkCore;

namespace KafkaFlowProducer.Persistence;

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