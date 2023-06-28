using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Demo.Services.AuthManager;
public class JwtAuthenticationManager: IJwtAuthenticationManager
{
    private readonly string _key;

    private readonly IDictionary<string, string> _users = new Dictionary<string, string>
    {
        {"test1", "password1"}, {"test2", "password2"}
    };

    public JwtAuthenticationManager(string key)
    {
        _key = key; // private key provided in Startup, used for encrypting the token
    }
    
    public string? Authenticate(string username, string password)
    {
        if (!_users.Any(u => u.Key == username && u.Value == password))
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(_key); // get the byte array of the private key
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}