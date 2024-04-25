using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApplication1.Models;

public class FoodContextConfiguration: IEntityTypeConfiguration<Food>
{
    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.HasData(
            new Food()
            {
                Name = "Carrot", Quantity = 2
            },
            new Food()
            {
                Name = "Tofu", Quantity = 1
            }
        );
    }
}