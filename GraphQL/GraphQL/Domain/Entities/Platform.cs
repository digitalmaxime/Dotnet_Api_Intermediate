namespace GraphQL.Domain.Entities;

public class Platform
{
    public int Id { get; init; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public string LiscenceKey { get; set; } = string.Empty;

    public ICollection<Command>? Commands { get; set; } = new List<Command>();
}