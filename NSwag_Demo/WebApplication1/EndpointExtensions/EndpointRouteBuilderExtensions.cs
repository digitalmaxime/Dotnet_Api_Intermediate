using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace WebApplication1.EndpointExtensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterFoodEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var foodEndpoints = endpointRouteBuilder
            .MapGroup("/food");

        foodEndpoints.MapGet("/api/foods", async (IGroceryService service) =>
            {
                var foods = await service.GetAllFoods();
                return TypedResults.Ok(foods);
            })
            .WithName("GetAllFoods");

        foodEndpoints.MapGet("/api/food/{name}",
                async Task<Results<Ok<Food>, NotFound>>([FromRoute] string name, IGroceryService service) =>
                {
                    var food = await service.GetFoodByName(name);
                    return food == null ? TypedResults.NotFound() : TypedResults.Ok(food);
                })
            .WithName("GetFood");

        foodEndpoints.MapPost("/api/food/",
                async Task<Results<Created<ICollection<Food>>, BadRequest>>([FromBody] Food food,
                    IGroceryService service) =>
                {
                    var foods = await service.AddFood(food);
                    return TypedResults.Created("/uri", foods);
                })
            .WithName("AddFood")
            .WithOpenApi(operation =>
            {
                operation.Security = new List<OpenApiSecurityRequirement>()
                {
                    new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme()
                            {
                                Reference = new OpenApiReference()
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = JwtBearerDefaults.AuthenticationScheme,
                                },
                                Scheme = SecuritySchemeType.Http.ToString(),
                                Name = JwtBearerDefaults.AuthenticationScheme,
                            },
                            new List<string>()
                        }
                    }
                };
            
                return operation;
            })
            .RequireAuthorization()
            ;
    }
}