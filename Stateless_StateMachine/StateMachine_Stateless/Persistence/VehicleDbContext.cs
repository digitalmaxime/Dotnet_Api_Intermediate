using Microsoft.EntityFrameworkCore;
using StateMachine.VehicleStateMachines;

namespace StateMachine.Persistence;

public class VehicleDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("VehicleState");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CarEntity>()
            .HasKey(x => x.Id);
        
        modelBuilder.Entity<PlaneEntity>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<CarEntity>()
            .HasData(new CarEntity()
                {
                    Id = "Id1",
                    Speed = 0,
                    State = CarStateMachine.CarState.Stopped
                },
                new CarEntity()
                {
                    Id = "Id2",
                    Speed = 0,
                    State = CarStateMachine.CarState.Stopped
                });

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<CarEntity> CarEntity { get; set; }
    public DbSet<PlaneEntity> PlaneEntity { get; set; }
}