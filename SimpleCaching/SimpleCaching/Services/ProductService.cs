using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SimpleCaching.Entities;
using SimpleCaching.Repositories.Contexts;

namespace SimpleCaching.Services;

public interface IProductService
{
    Task<List<Product>> GetAllProductsAsync();
}

public class ProductService : IProductService
{
    private readonly IMemoryCache _cache;
    private readonly ProductContext _productContext;

    public ProductService(IMemoryCache cache, ProductContext productContext)
    {
        _cache = cache;
        _productContext = productContext;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _productContext.Products.OrderBy(p => p.Name).ToListAsync();
    }
}