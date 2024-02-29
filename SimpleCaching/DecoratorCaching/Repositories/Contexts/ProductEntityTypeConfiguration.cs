using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleCaching.Entities;

namespace SimpleCaching.Repositories.Contexts;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasData(new List<Product>()
        {
            new Product() { Id = 1, Name = "P1" },
            new Product() { Id = 2, Name = "P2" }
        });
    }
}