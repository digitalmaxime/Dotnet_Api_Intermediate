using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Demo.Config;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace Auth.Demo.Services.AuthManager;
public class JwtAuthenticationManager: IJwtAuthenticationManager
{
    private readonly JwtOptions _jwtOptions;

    private readonly IDictionary<string, string> _users = new Dictionary<string, string>
    {
        {"test1", "password1"}, {"test2", "password2"}
    };

    public JwtAuthenticationManager(IOptions<JwtOptions> jwtOptions )
    {
        _jwtOptions = jwtOptions.Value; // private key provided in Startup, used for encrypting the token
    }
    
    public string? Authenticate(string username, string password)
    {
        if (!_users.Any(u => u.Key == username && u.Value == password))
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(_jwtOptions.PrivateKey); // get the byte array of the private key
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