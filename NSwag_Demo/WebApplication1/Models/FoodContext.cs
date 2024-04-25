using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models;

public class FoodContext : DbContext
{
    public FoodContext(DbContextOptions<FoodContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FoodContext).Assembly);
        modelBuilder.Entity<Food>().HasKey(x => x.Name);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Food> Foods { get; set; }
}