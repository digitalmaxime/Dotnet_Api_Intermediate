namespace AgentFrameworkChat.AI;

public interface IBasicAgent
{
    Task<string> SendMessage(string message);
}