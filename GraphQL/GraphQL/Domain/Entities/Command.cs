namespace GraphQL.Domain.Entities;

public class Command
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string CommandLine { get; set; }
    public required int PlatformId { get; set; }
    public Platform? Platform { get; set; }
}