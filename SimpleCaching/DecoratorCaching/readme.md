# Caching Decorator

Project demonstrating simple caching with decorator pattern 

Working with In-Memory database

## Implementation steps
1) [Add required pacakges](#packages)
1) [Create the decorator class](#decorator)
1) [DI the decorator class](#di)
1) [Enable caching](#caching)

## Packages

Add scrutor to enable decorator pattern `dotnet add package Scrutor`

## Decorator

The decorator class **needs** to take in the common interface as ctor parameter.

Aka the interface of the class it decorates

```
public class ProductServiceCacheDecorator: IProductService
{
    public ProductServiceCacheDecorator(IProductService productService)
    { }
}
```

## DI

```
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.Decorate<IProductService, ProductServiceCacheDecorator>();
```


## Caching
```
builder.Services.AddMemoryCache();
```

Use IMemoryCache something like this :

```
if (_memoryCache.TryGetValue(cacheKey, out List<Product>? result)) 
{ ... } 

```

