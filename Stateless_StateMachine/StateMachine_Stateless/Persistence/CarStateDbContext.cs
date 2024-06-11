using Microsoft.EntityFrameworkCore;

namespace CarStateMachine.Persistence;

public class CarStateDbContext: DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("CarState");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CarStateMachine>().HasData(new CarStateMachine
        {
            Name = "default",
            Current
        })
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<CarStateMachine> Car { get; set; }
}