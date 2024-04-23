using System.Net.Http.Headers;
using AutoGenApiClient;
using ConsoleApp1;
using Microsoft.Extensions.DependencyInjection;
using MyNamespace;

Console.WriteLine("Enter food name");
var foodName = Console.ReadLine();

var serviceCollection = new ServiceCollection();
// serviceCollection.AddSingleton<IMyClient, MyClient>();
serviceCollection.AddHttpClient<IMyClient, MyClient>(client =>
    client.BaseAddress = new Uri("https://localhost:7067"));
// .AddHttpClient()
serviceCollection.AddSingleton<MyApp>();
var serviceProvider = serviceCollection.BuildServiceProvider();
// var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
//
// var client = httpClientFactory.CreateClient("MyClient");
// client.BaseAddress = new Uri("https://localhost:7067");

// var toto = await client.GetAsync($"food/api/food/{foodName}");
var client = serviceProvider.GetRequiredService<IMyClient>();
var res = await client.GetFoodAsync(foodName);
// var content = await res.Content.ReadAsStringAsync();

Console.WriteLine(res);

// var serviceProvider = Services.ServiceProvider;
// var foodClient = serviceProvider.GetRequiredService<IMyApp>();
// foodClient.DoStuff();