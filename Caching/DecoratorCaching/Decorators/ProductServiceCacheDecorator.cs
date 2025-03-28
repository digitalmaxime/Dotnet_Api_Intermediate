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

        var cacheKey = nameof(GetAllProductsAsync);
        
        return await _memoryCache.GetOrCreateAsync<List<Product>>(
            cacheKey,
            cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(30);
                return _productService.GetAllProductsAsync();
            }) ?? new List<Product>();
        
        
        var options = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(30))
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
        
        if (_memoryCache.TryGetValue(cacheKey, out List<Product>? result))
        {
            if (result != null) return result;
        }

        result = await _productService.GetAllProductsAsync();

        _memoryCache.Set(cacheKey, result, options);

        return result;
    }
}