namespace AgentFrameworkChat.Options;

public class AzureOpenAiOptions
{
    public const string SectionName = "AzureOpenAI"; 
    public required string Endpoint { get; set; }
    public required string DeploymentName { get; set; }
    public required string ApiKey { get; set; }
}