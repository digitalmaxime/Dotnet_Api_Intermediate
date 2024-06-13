using System.Text.Json;
using CarStateMachine;
using CarStateMachine.CarStateManagerFactory;
using CarStateMachine.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<CarStateDbContext>();

builder.Services.AddScoped<ICarStateRepository, CarStateRepository>();

builder.Services.AddScoped<ICarStateManagerFactory, CarStateManagerFactory>();

builder.Services.AddSingleton<Game>();

var app = builder.Build();

var dbContext = app.Services.GetService<CarStateDbContext>();

if (dbContext != null)
{
    await dbContext.Database.EnsureCreatedAsync();
}

var game = app.Services.GetService<Game>();

var availableCarTypes = JsonSerializer.Serialize(Enum.GetNames<CarType>());

Console.Write($"Choose a car type : {availableCarTypes} : ");

var success = Enum.TryParse<CarType>(Console.ReadLine(), out var type);

game!.Start(success ? type : CarType.Basic);
