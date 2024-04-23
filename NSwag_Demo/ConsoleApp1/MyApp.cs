using System.Net.Http.Headers;
using AutoGenApiClient;

namespace ConsoleApp1;

public interface IMyApp
{
    void DoStuff();
}

public class MyApp : IMyApp
{
    private readonly IFoodHttpService _foodHttpService;

    public MyApp(IFoodHttpService foodHttpService)
    {
        _foodHttpService = foodHttpService;
    }

    public async void DoStuff()
    {
        var res = _foodHttpService.GetFoodAsync("Carrot");
    }
    
    public async void DoStuff2()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6Im1heGltZXRyZW1ibGF5LXJoZWF1bHQiLCJzdWIiOiJtYXhpbWV0cmVtYmxheS1yaGVhdWx0IiwianRpIjoiMjVlYzM0NzUiLCJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdDo0MjY3OSIsImh0dHBzOi8vbG9jYWxob3N0OjQ0Mzg3IiwiaHR0cDovL2xvY2FsaG9zdDo1MDU0IiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NzA2NyJdLCJuYmYiOjE3MTM3OTA0NTIsImV4cCI6MTcyMTY1Mjg1MiwiaWF0IjoxNzEzNzkwNDUyLCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.GlLJd-OX654QbNLa79-hrXUzsm_5t8oDwTeyTo63IIM");

        var httpClient = new MyClient("https://localhost:7067", client);

        Console.WriteLine("Enter an ingredient");
        var ingredient = Console.ReadLine();
        Console.WriteLine();
        Console.WriteLine(ingredient);
        Console.WriteLine();


        var carrot = await httpClient.GetFoodAsync("Carrot");

        Console.WriteLine("Name " + carrot.Name);
        Console.WriteLine("Qty " + carrot.Quantity);

        var postReq = await httpClient.AddFoodAsync(new Food() { Name = ingredient, Quantity = 3 });

        Console.WriteLine(postReq);
        Console.WriteLine();

        var allFoods = await httpClient.GetAllFoodsAsync();

        foreach (var food in allFoods)
        {
            Console.WriteLine(food.Name);
        }

        Console.WriteLine();
    }
}