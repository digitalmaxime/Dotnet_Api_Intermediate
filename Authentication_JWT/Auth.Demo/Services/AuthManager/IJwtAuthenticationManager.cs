namespace Auth.Demo.Services.AuthManager;

public interface IJwtAuthenticationManager
{
    string? Authenticate(string username, string password);
}