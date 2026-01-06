namespace AgentFrameworkChat.Options;

public class AzureOpenAiOptions
{
    public const string SectionName = "AzureOpenAI"; 
    public string endpoint { get; set; }
    public string deploymentName { get; set; }
    public string apiKey { get; set; }
}