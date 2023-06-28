using Auth.Demo.Services.AuthManager;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Demo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NameController: ControllerBase
{
    private readonly IJwtAuthenticationManager _jwtAuthenticationManager;

    public NameController(IJwtAuthenticationManager jwtAuthenticationManager)
    {
        _jwtAuthenticationManager = jwtAuthenticationManager;
    }
    
    [HttpGet("GetListOfString")]
    public IEnumerable<string> GetListOfString()
    {
        return new string[] { "a", "b", "c" };
    }

    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] UserCred userCred)
    {
        var token = _jwtAuthenticationManager.Authenticate(userCred.Username, userCred.Password);
        
        if (token == null)
        {
            return Unauthorized();
        }
        
        return Ok();
    }
}