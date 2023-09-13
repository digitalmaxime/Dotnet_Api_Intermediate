using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AppJwt.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly PositionOptions _options;

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptions<PositionOptions> options)
    {
        _options = options.Value;
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast"), Authorize]
    public IEnumerable<string> Get()
    {
        return new List<string>()
        {
            _options.Name, _options.Title
        };

    }
}