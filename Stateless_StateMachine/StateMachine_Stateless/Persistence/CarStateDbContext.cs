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
            .HasKey(x => x.Id);

        modelBuilder.Entity<VehicleEntity>()
            .HasData(new VehicleEntity()
                {
                    Id = "Name1",
                    Speed = 0,
                    State = CarStateMachine.CarState.Stopped
                },
                new VehicleEntity()
                {
                    Id = "Name2",
                    Speed = 0,
                    State = CarStateMachine.CarState.Stopped
                });

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<CarEntity> CarEntity { get; set; }
}