using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StateMachine;
using StateMachine.Persistence;
using StateMachine.Persistence.Constants;
using StateMachine.Persistence.Contracts;
using StateMachine.Persistence.Domain;
using StateMachine.Persistence.Repositories;
using StateMachine.VehicleStateMachineFactory;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<VehicleDbContext>();
builder.Services.AddScoped<ICarStateRepository, CarStateRepository>();
builder.Services.AddScoped<IPlaneStateRepository, PlaneStateRepository>();
builder.Services.AddSingleton<IVehicleFactory, VehicleFactory>();

builder.Services.AddSingleton<Game>();

var app = builder.Build();

var dbContext = app.Services.GetService<VehicleDbContext>();

if (dbContext != null)
{
    await dbContext.Database.EnsureCreatedAsync();
}

var game = app.Services.GetService<Game>();

var availableVehicleTypes = JsonSerializer.Serialize(Enum.GetNames<VehicleType>());

bool wannaStartGame;
do
{
    Console.Write($"Choose a vehicle type : {availableVehicleTypes} : ");
    var success = Enum.TryParse<VehicleType>(Console.ReadLine(), out var type);

    Console.Write("Choose a vehicle id : ");
    var vehicleId = Console.ReadLine();

    game!.Start(success ? type : VehicleType.Car, string.IsNullOrEmpty(vehicleId) ? "Id1" : vehicleId);

    Console.Write("Type 'yes' to start a new state machine game : ");
    wannaStartGame = Console.ReadLine() == "yes";
} while (wannaStartGame);

Console.WriteLine("Bye bye");