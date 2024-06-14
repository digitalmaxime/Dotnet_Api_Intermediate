using System.Text.Json;
using CarStateMachine;
using CarStateMachine.CarStateManagerFactory;
using CarStateMachine.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<CarStateDbContext>();

builder.Services.AddScoped<IVehicleStateRepository, VehicleStateRepository>();

builder.Services.AddScoped<IVehicleStateMachineBase, CarStateMachine.CarStateMachine>();

builder.Services.AddScoped<IVehicleStateManagerFactory, VehicleStateManagerFactory>();

builder.Services.AddSingleton<Game>();

var app = builder.Build();

var dbContext = app.Services.GetService<CarStateDbContext>();

if (dbContext != null)
{
    await dbContext.Database.EnsureCreatedAsync();
}

var game = app.Services.GetService<Game>();

var availableCarTypes = JsonSerializer.Serialize(Enum.GetNames<VehicleType>());

Console.Write($"Choose a car type : {availableCarTypes} : ");

var success = Enum.TryParse<VehicleType>(Console.ReadLine(), out var type);

game!.Start(success ? type : VehicleType.Basic);
