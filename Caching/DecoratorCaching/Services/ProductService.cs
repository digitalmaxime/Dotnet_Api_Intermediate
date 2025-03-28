using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SimpleCaching.Contexts;
using SimpleCaching.Entities;
using SimpleCaching.Repositories.Contexts;

namespace SimpleCaching.Services;

public interface IProductService
{
    Task<List<Product>> GetAllProductsAsync();
}

public class ProductService : IProductService
{
    private readonly ProductContext _productContext;

    public ProductService(ProductContext productContext)
    {
        _productContext = productContext;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _productContext.Products.OrderBy(p => p.Name).ToListAsync();
    }
}