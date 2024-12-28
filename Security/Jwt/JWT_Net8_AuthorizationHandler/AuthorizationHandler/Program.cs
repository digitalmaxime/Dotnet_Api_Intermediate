using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder();

// Auth
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

// Services
builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeAuthorizationHandler>();
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

[ApiController]
[Route("api/[controller]")]
public class GreetingsController : Controller
{
    [MinimumAgeAuthorize(16)]
    [HttpGet("hello")]
    public string Hello() => $"Hello {(HttpContext.User.Identity?.Name ?? "world")}!";
}

class MinimumAgeAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthorizationRequirementData
{
    public MinimumAgeAuthorizeAttribute(int age) => Age =age;
    public int Age { get; }

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}

class MinimumAgeAuthorizationHandler : AuthorizationHandler<MinimumAgeAuthorizeAttribute>
{
    private readonly ILogger<MinimumAgeAuthorizationHandler> _logger;

    public MinimumAgeAuthorizationHandler(ILogger<MinimumAgeAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    // Check whether a given MinimumAgeRequirement is satisfied or not for a particular context
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeAuthorizeAttribute requirement)
    {
        // Log as a warning so that it's very clear in sample output which authorization policies
        // (and requirements/handlers) are in use
        _logger.LogWarning("Evaluating authorization requirement for age >= {age}", requirement.Age);

        // Check the user's age
        var dateOfBirthClaim = context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth);
        if (dateOfBirthClaim != null)
        {
            // If the user has a date of birth claim, check their age
            var dateOfBirth = Convert.ToDateTime(dateOfBirthClaim.Value, CultureInfo.InvariantCulture);
            var age = DateTime.Now.Year - dateOfBirth.Year;
            if (dateOfBirth > DateTime.Now.AddYears(-age))
            {
                // Adjust age if the user hasn't had a birthday yet this year
                age--;
            }

            // If the user meets the age criterion, mark the authorization requirement succeeded
            if (age >= requirement.Age)
            {
                _logger.LogInformation("Minimum age authorization requirement {age} satisfied", requirement.Age);
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogInformation("Current user's DateOfBirth claim ({dateOfBirth}) does not satisfy the minimum age authorization requirement {age}",
                    dateOfBirthClaim.Value,
                    requirement.Age);
            }
        }
        else
        {
            _logger.LogInformation("No DateOfBirth claim present");
        }

        return Task.CompletedTask;
    }
}