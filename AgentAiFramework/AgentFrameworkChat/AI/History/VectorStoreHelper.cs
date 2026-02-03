using System.Security.Cryptography;
using System.Text;

namespace AgentFrameworkChat.AI.History;

public static class VectorStoreHelper
{
    public static int GenerateChatMessageIntKey(string threadId, string messageId)
    {
        var compositeKey = $"{threadId}_{messageId}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(compositeKey));
        return Math.Abs(BitConverter.ToInt32(hash, 0));
    }
    
    public static long GenerateIntKeyFromString(string username)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(username));
        return Math.Abs(BitConverter.ToInt64(hash, 0));
    }
}