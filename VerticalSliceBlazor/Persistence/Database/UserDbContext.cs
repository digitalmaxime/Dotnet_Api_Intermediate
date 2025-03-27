using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Database;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersistenceLibrary).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<User> Users { get; set; }

}

