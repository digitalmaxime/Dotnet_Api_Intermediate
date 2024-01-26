using System.Runtime.CompilerServices;

namespace Singleton;

public class Logger
{
    // Lazy<T>
    private static readonly Lazy<Logger> LazyLogger = new(() => new Logger());
    public static Logger Instance => LazyLogger.Value;
    
    // private static Logger? _instance;
    // public static Logger Instance => _instance ?? (_instance = new Logger());

    protected Logger()
    {
    }

    public void Log(string msg)
    {
        Console.WriteLine($"Message to log: {msg}");
    }
}