# Caching Decorator

Project demonstrating simple caching with decorator pattern

## Implementation steps
1) [Define the model classes](#model)
2) [Add required pacakges](#packages)
3) [Create the decorator class](#decorator)
4) [DI the decorator class](#di)
5) [Enable caching](#caching)

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

