using Application.Features.GetUserList;
using MediatR;

namespace Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("/api/users").WithTags("users");
        
        group.MapGet("", async (IMediator mediator) =>
            {
                var userListResponse = await mediator.Send(new GetUserListQuery());
                return TypedResults.Ok(userListResponse);
            })
            .WithName("GetUsers")
            .WithDescription("Get all users");
    }
}