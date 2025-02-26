using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StateMachine.Persistence.Domain;
using StateMachine.VehicleStateMachines;

namespace StateMachine.Persistence;

public class VehicleDbContext : DbContext
{
    public VehicleDbContext(DbContextOptions<VehicleDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("VehicleState");
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Name }, LogLevel.Information);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CarEntity>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<PlaneEntity>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<PlaneEntity>()
            .HasData(new PlaneEntity()
                {
                    Id = "Id1",
                    Speed = 0,
                    State = PlaneStateMachine.PlaneState.Stopped
                },
                new PlaneEntity()
                {
                    Id = "Id2",
                    Speed = 0,
                    State = PlaneStateMachine.PlaneState.Stopped
                });

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<CarEntity> CarEntity { get; set; } = default!;
    public DbSet<PlaneEntity> PlaneEntity { get; set; } = default!;
}