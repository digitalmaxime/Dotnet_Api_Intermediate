using ConsoleApp1;
using Microsoft.Extensions.DependencyInjection;
using MyNamespace;

Console.WriteLine("Enter food name");
var foodName = Console.ReadLine();

var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<IFoodService, FoodService>();
serviceCollection.AddHttpClient<IMyClient, MyClient>(client =>
    client.BaseAddress = new Uri("https://localhost:7067"));
var serviceProvider = serviceCollection.BuildServiceProvider();

var foodService = serviceProvider.GetRequiredService<IFoodService>();
var res = await foodService.GetFoodAsync(foodName);

Console.WriteLine(res);
