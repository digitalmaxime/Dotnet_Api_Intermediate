using System.Net.Security;

namespace Auth.Demo.Services.CustomAuthManager;

public interface ICustomAuthManager
{
    public IDictionary<string, string> Tokens { get; }
    string Authenticate(string username, string password);
}