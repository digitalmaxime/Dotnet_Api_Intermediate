namespace AgentFrameworkChat.AI.Agents;

public interface IBasicAgent
{
    Task<string> SendMessage(string message, string username);
}