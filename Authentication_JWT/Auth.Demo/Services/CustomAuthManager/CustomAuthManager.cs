using System.Collections;
using Auth.Demo.Config;
using Microsoft.Extensions.Options;

namespace Auth.Demo.Services.CustomAuthManager;

public class CustomAuthManager: ICustomAuthManager
{
    private readonly JwtOptions _jwtOptions;

    private readonly IDictionary<string, string> _users = new Dictionary<string, string>
    {
        {"test1", "password1"}, {"test2", "password2"}
    };

    private readonly IDictionary<string, string> tokens = new Dictionary<string, string>();

    public IDictionary<string, string> Tokens => tokens;
    
    public CustomAuthManager(IOptions<JwtOptions> jwtOptions )
    {
        _jwtOptions = jwtOptions.Value; // private key provided in Startup, used for encrypting the token
    }
    
    public string Authenticate(string username, string password)
    {
        if (!_users.Any(u => u.Key == username && u.Value == password))
        {
            return null;
        }
        
        var token = Guid.NewGuid().ToString();
        
        tokens.Add(token, username); //instead of an in-memory dictionary, it should ideally be stored in a Redis Cache
        
        return token;
    }
}