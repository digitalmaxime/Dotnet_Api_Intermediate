using HangfireDemo.Domain;
using Microsoft.EntityFrameworkCore;

namespace HangfireDemo.Infra;

public class PersonContext : DbContext
{
    public PersonContext(DbContextOptions options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Person>()
            .HasIndex(p => p.Id)
            .IsUnique();
    }

    public DbSet<Person> Persons => Set<Person>(); // Entity set for Entity 'Person' 
}