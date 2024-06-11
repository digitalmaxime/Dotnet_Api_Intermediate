using System.Text.Json;
using CarStateMachine;
using CarStateMachine.CarStateManagerFactory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<ICarStateManagerFactory, CarStateManagerFactory>();

builder.Services.AddSingleton<Game>();

var app = builder.Build();

var game = app.Services.GetService<Game>();

var availableCarTypes = JsonSerializer.Serialize(Enum.GetNames<CarType>());

Console.Write($"Choose a car type : {availableCarTypes} : ");

var success = Enum.TryParse<CarType>(Console.ReadLine(), out var type);

game!.Start(success ? type : CarType.Basic);