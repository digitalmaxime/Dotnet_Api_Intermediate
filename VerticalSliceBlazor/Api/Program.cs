using Api.Endpoints;
using Api.Middleware;
using Application.Extensions;
using Microsoft.EntityFrameworkCore;
using Persistence.Database;
using Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.RegisterPersistence(builder.Configuration);
builder.Services.RegisterApplication();
builder.Services.AddTransient<ExceptionMiddleware>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // http://localhost:<PORT>/openapi/v1.json

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    // dbContext.Database.EnsureDeleted();
    dbContext.Database.Migrate();
    Seed.SeedDataWithAddRange(dbContext);
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.MapUserEndpoints();


app.Run();