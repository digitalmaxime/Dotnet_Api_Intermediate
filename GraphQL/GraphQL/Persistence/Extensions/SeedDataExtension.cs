using GraphQL.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.Persistence.Extensions;

public static class SeedDataExtension
{
    public static void SeedData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Platform>().HasData(new Platform()
            {
                Id = 1,
                Name = "Dotnet",
                Description = "A platform for building applications using C#",
                LiscenceKey = "asdf"
            },
            new Platform()
            {
                Id = 2,
                Name = "Docker",
                Description = "A platform for building applications using containers",
                LiscenceKey = "asdf"
            },
            new Platform()
            {
                Id = 3,
                Name = "Linux",
                Description = "Open source operating system",
            });

        modelBuilder.Entity<Command>().HasData(
            new Command()
            {
                Id = 1,
                Name = "dotnet new",
                CommandLine = "dotnet new graphql",
                PlatformId = 1
            },
            new Command()
            {
                Id = 2,
                Name = "dotnet add package",
                CommandLine = "dotnet add package <Package Name>",
                PlatformId = 1
            },
            new Command()
            {
                Id = 3,
                Name = "dotnet restore",
                CommandLine = "dotnet restore",
                PlatformId = 1
            },
            new Command()
            {
                Id = 4,
                Name = "list current directory",
                CommandLine = "ls -la",
                PlatformId = 3
            },
            new Command()
            {
                Id = 5,
                Name = "Launch a container",
                CommandLine = "docker run -d -p 8080:80 --name myapp myapp",
                PlatformId = 2
            },
            new Command()
            {
                Id = 6,
                Name = "Stop a container",
                CommandLine = "docker stop <Container Name>",
                PlatformId = 2
            }
        );
    }
}