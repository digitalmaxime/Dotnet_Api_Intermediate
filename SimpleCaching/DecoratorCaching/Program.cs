using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SimpleCaching.Contexts;
using SimpleCaching.Decorators;
using SimpleCaching.Entities;
using SimpleCaching.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductContext>(o => o.UseInMemoryDatabase("ProductsDb"));

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.Decorate<IProductService, ProductServiceCacheDecorator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
using (var context = scope.ServiceProvider.GetService<ProductContext>()) context?.Database.EnsureCreated();

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