using Hangfire;
using Hangfire.MySql;
using HangfireDemo.Application.Service;
using HangfireDemo.Infra;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


string? hangfireConnectionString = builder.Configuration.GetConnectionString("Hangfire");
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseStorage(
        new MySqlStorage(
            hangfireConnectionString,
            new MySqlStorageOptions
            {
                QueuePollInterval = TimeSpan.FromSeconds(10),
                JobExpirationCheckInterval = TimeSpan.FromHours(1),
                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                PrepareSchemaIfNecessary = true,
                DashboardJobListLimit = 25000,
                TransactionTimeout = TimeSpan.FromMinutes(1),
                TablesPrefix = "Hangfire",
            }
        )
    )); // Hangfire client (default config)

builder.Services.AddHangfireServer(); // Hangfire server with default config (polling every 15 seconds)

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PersonContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("Hangfire") ?? string.Empty));

builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<ITimeService, TimeService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();


app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<ITimeService>("print-time-unique-name", service => service.PrintTime(), Cron.Minutely);
// RecurringJob.AddOrUpdate("easyjob", () => Console.Write("Easy!"), Cron.Minutely);

app.MapGet("/", (context) =>
{
    context.Response.Redirect("./swagger/index.html", permanent: false);
    return Task.FromResult(0);
});

app.Run();