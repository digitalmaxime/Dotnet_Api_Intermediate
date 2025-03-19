using Microsoft.EntityFrameworkCore;

namespace VerticalSliceBlazor.Persistence.Database;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersistenceLibrary).Assembly);
        modelBuilder.Entity<User>().HasData(
        new List<User>()
        {
            new()
            {
                Id = new Guid("939df3fd-de57-4caf-96dc-c5e110322a96"),
                Name = "Max",
                Email = "max@email.com",
                Password = "password"
            },
            new()
            {
                Id = new Guid("d70f656d-75a7-45fc-b385-e4daa834e6a8"),
                Name = "maude",
                Email = "maude@email.com",
                Password = "password"
            }
        });
        
        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<User> Users { get; set; }
}

public class User
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
