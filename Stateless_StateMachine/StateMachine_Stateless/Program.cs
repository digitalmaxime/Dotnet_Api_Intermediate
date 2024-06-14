using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StateMachine;
using StateMachine.Persistence;
using StateMachine.VehicleStateMachineFactory;
using StateMachine.VehicleStateMachines;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<CarStateDbContext>();

builder.Services.AddScoped<IVehicleStateRepository, VehicleStateRepository>();

builder.Services.AddScoped<IVehicleStateMachineBase, CarStateMachine>();

builder.Services.AddScoped<IVehicleFactory, VehicleFactory>();

builder.Services.AddSingleton<Game>();

var app = builder.Build();

var dbContext = app.Services.GetService<CarStateDbContext>();

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

    Console.Write("Choose a vehicle name : ");
    var vehicleName = Console.ReadLine();

    game!.Start(success ? type : VehicleType.Car, string.IsNullOrEmpty(vehicleName) ? "Name1" : vehicleName);

    Console.Write("Type 'yes' to start a new state machine game : ");
    wannaStartGame = Console.ReadLine() == "yes";
} while (wannaStartGame);

Console.WriteLine("Bye bye");