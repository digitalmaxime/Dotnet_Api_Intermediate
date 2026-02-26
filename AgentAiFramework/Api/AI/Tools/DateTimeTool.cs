using System.ComponentModel;

namespace AgentFrameworkChat.AI.Tools;

public class DateTimeTool
{
    [Description("Get the current utc date and time.")]
    public static string GetDateTime()
        => $"The current utc date time is {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss}";
}