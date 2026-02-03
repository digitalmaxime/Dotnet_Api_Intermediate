namespace AgentFrameworkChat.Options;

public class PostgresOptions
{
    public const string SectionName = "Postgres";
    public required string ConnectionString { get; init;  }
}