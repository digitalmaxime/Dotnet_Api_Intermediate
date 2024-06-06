using Car_StateMachine;
using CarStateMachine;
using CarStateMachine.CarStateManagerFactory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<ICarStateManagerFactory, CarStateManagerFactory>();

builder.Services.AddSingleton<Game>();

var app = builder.Build();

var game = app.Services.GetService<Game>();

// Console.WriteLine("Basic or Premium?");
//
// var type = Console.ReadLine();

game!.Start("Basic");
