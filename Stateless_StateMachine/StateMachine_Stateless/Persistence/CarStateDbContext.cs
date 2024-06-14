using Microsoft.EntityFrameworkCore;
using StateMachine.VehicleStateMachines;

namespace StateMachine.Persistence;

public class CarStateDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("CarState");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VehicleEntity>()
            .HasKey(x => x.Name);

        modelBuilder.Entity<VehicleEntity>()
            .HasData(new VehicleEntity()
                {
                    Name = "Name1",
                    Speed = 0,
                    State = State.Stopped
                },
                new VehicleEntity()
                {
                    Name = "Name2",
                    Speed = 0,
                    State = State.Stopped
                });

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<VehicleEntity> CarEntity { get; set; }
}