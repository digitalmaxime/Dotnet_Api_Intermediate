namespace AgentFrameworkChat.Options;

public class AgentConfigurationOptions
{
    public const string SectionName = "AgentConfiguration";
    
    public required string Description { get; init; }
    
    public required float Temperature { get; set; }
    
    public required string[] LlmInstructions { get; init; }
}