using Microsoft.EntityFrameworkCore;

namespace CarStateMachine.Persistence;

public class CarStateDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("CarState");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CarEntity>()
            .HasKey(x => x.Name);
        
        modelBuilder.Entity<CarEntity>()
            .HasData(new CarEntity()
            {
                Name = "Name1",
                Speed = 0,
                State = Car.State.Stopped
            });

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<CarEntity> CarEntity { get; set; }
}