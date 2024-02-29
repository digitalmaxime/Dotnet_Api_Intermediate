using Microsoft.EntityFrameworkCore;
using SimpleCaching.Entities;

namespace SimpleCaching.Repositories.Contexts;

public class ProductContext: DbContext
{
    public ProductContext(DbContextOptions options): base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductContext).Assembly);

        modelBuilder.Entity<Product>().Property(p => p.Id).IsRequired();
    }
    
    public DbSet<Product> Products { get; set; }
}