namespace HangfireDemo.Application.Service;

public interface ITimeService
{
    void PrintTime();
}

public class TimeService : ITimeService
{
    private readonly ILogger<TimeService> _logger;

    public TimeService(ILogger<TimeService> logger)
    {
        _logger = logger;
    }

    public void PrintTime()
    {
        _logger.LogInformation(DateTime.Now.ToString("dd/MM/yyy hh:mm:ss tt"));
    }
}