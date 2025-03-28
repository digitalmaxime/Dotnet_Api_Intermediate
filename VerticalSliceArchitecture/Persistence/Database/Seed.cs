using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Database;

public static class Seed
{
    public static void SeedDataWithHasData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new List<User>()
            {
                new()
                {
                    Id = new Guid("939df3fd-de57-4caf-96dc-c5e110322a96"),
                    Name = "Max",
                    Email = "max@email.com",
                    Password = "password"
                },
                new()
                {
                    Id = new Guid("d70f656d-75a7-45fc-b385-e4daa834e6a8"),
                    Name = "maude",
                    Email = "maude@email.com",
                    Password = "password"
                }
            });
    }

    public static void SeedDataWithAddRange(UserDbContext context)
    {
        if (context.Users.Any()) return;
        
        var toto = new Bogus.Faker<User>()
            .StrictMode(true)
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, f => f.Internet.Password())
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .Generate(100);
        
        context.Users.AddRange(toto);
        context.SaveChanges();
    }
}