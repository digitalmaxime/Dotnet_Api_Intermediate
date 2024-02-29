using Microsoft.Extensions.Caching.Memory;
using SimpleCaching.Entities;
using SimpleCaching.Services;

namespace SimpleCaching.Decorators;

public class ProductServiceCacheDecorator: IProductService
{
    private readonly IProductService _productService;
    private readonly IMemoryCache _memoryCache;

    public ProductServiceCacheDecorator(IProductService productService, IMemoryCache memoryCache)
    {
        _productService = productService;
        _memoryCache = memoryCache;
    }
    public async Task<List<Product>> GetAllProductsAsync()
    {
        var options = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(30))
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

        var cacheKey = nameof(GetAllProductsAsync);

        if (_memoryCache.TryGetValue(cacheKey, out List<Product>? result))
        {
            if (result != null) return result;
        }

        result = await _productService.GetAllProductsAsync();

        _memoryCache.Set(cacheKey, result, options);

        return result;
    }
}