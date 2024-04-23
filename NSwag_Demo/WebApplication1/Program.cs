using WebApplication1.ConfigurationExtensions;

using WebApplication1.EndpointExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

var app = builder.Build();

app.ConfigurePipeline();

app.RegisterFoodEndpoints();

app.Run();