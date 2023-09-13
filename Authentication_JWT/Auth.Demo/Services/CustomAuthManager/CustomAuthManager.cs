using System.Collections;
using Auth.Demo.Config;
using Microsoft.Extensions.Options;

namespace Auth.Demo.Services.CustomAuthManager;

public class CustomAuthManager : ICustomAuthManager
{
    private readonly JwtOptions _jwtOptions;

    private readonly IList<User> _users = new List<User>
    {
        new User() { Username = "test1", Password = "password1", Role = "Administrator" },
        new User() { Username = "test2", Password = "password2", Role = "User" }
    };

    private readonly IDictionary<string, Tuple<string, string>> _tokens =
        new Dictionary<string, Tuple<string, string>>(); // Tokens contain key (guid) and value (username and role)

    public IDictionary<string, Tuple<string, string>> Tokens => _tokens;

    public CustomAuthManager(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value; // private key provided in Startup, used for encrypting the token
    }

    public string? Authenticate(string username, string password)
    {
        if (!_users.Any(u => u.Username == username && u.Password == password))
        {
            return null;
        }

        var token = Guid.NewGuid().ToString();

        _tokens.Add(token,
            new Tuple<string, string>(
                username,
                _users.FirstOrDefault(u => u.Username == username && u.Password == password).Role
            )); //instead of an in-memory dictionary, it should ideally be stored in a Redis Cache

        return token;
    }
}

internal class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}