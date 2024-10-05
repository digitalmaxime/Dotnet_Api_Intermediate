using GraphQL.Domain.Entities;
using GraphQL.Persistence.EntityConfigurations;
using GraphQL.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.Persistence.Context;

public class DemoDbContext : DbContext
{
    public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
    {
        Console.WriteLine("Inside DemoDbContext Constructor!");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.ApplyConfiguration(new PlatformEntityConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersistenceLibrary).Assembly);

        modelBuilder.SeedData();
    }

    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Command> Commands { get; set; }
}