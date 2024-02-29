using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SimpleCaching.Decorators;
using SimpleCaching.Entities;
using SimpleCaching.Repositories.Contexts;
using SimpleCaching.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductContext>();

var options = new DbContextOptionsBuilder<ProductContext>()
    .UseInMemoryDatabase("ProductsDb").Options;

using (var context = new ProductContext(options))
{
    context.Database.EnsureCreated();
}

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.Decorate<IProductService, ProductServiceCacheDecorator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/products", async Task<Ok<List<Product>>> (IProductService productService) =>
{
    var res = await productService.GetAllProductsAsync();
    return TypedResults.Ok(res);
});

app.Run();