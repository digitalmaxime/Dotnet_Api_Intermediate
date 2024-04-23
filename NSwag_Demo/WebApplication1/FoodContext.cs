using Microsoft.EntityFrameworkCore;

namespace WebApplication1;

public class FoodContext : DbContext
{
    public FoodContext(DbContextOptions<FoodContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Food>()
            .HasKey(x => x.Name);

        modelBuilder.Entity<Food>().HasData(
            new Food()
            {
                Name = "Carrot", Quantity = 2
            },
            new Food()
            {
                Name = "Tofu", Quantity = 1
            }
        );

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Food> Foods { get; set; }
}