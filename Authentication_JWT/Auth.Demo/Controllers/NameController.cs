using Auth.Demo.Entities;
using Auth.Demo.Services.AuthManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Demo.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class NameController: ControllerBase
{
    private readonly IJwtAuthenticationManager _jwtAuthenticationManager;

    public NameController(IJwtAuthenticationManager jwtAuthenticationManager)
    {
        _jwtAuthenticationManager = jwtAuthenticationManager;
    }

    [HttpGet]
    [Route("GetValueByQueryParam")] 
    public string GetValueByQueryParam(int id) // api/Name/GetValue?id=4
    {
        return "value " + id.ToString();
    }
    
    [HttpGet]
    [Route("GetValueByPathParam/{id}")]
    public string GetValueByPathParam(int id) // api/Name/GetOtherValue/5
    {
        return "value " + id.ToString();
    }
    
    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] UserCred userCred)
    {
        var token = _jwtAuthenticationManager.Authenticate(userCred.Username, userCred.Password);
        
        if (token == null)
        {
            return Unauthorized();
        }
        
        return Ok(token);
    }
}