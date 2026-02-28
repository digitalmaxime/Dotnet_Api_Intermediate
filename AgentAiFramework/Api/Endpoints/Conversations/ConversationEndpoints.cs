using Application.Features.Chat;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Mvc;

namespace AgentFrameworkChat.Endpoints.Conversations;

public static class ConversationEndpoints
{
    public static IEndpointRouteBuilder MapConversationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("conversations")
            .WithTags("Conversations");
        // TODO: require auth
        // TODO: add endpoint filter
        // TODO: validator

        group.MapPost("", async Task<Results<Ok<ChatCommandResponseDto>, BadRequest<List<ValidationFailure>>>>
        (HttpContext httpContext, IMediator mediator, /*, IValidator<ChatRequestDto> validator, */
            [FromBody] ChatRequestDto requestDto) =>
        {
            // var validationResult = await validator.ValidateAsync(requestDto);
            // if (!validationResult.IsValid)
            // {
            //     return TypedResults.BadRequest(validationResult.Errors);
            // }

            var username0 = httpContext.User.Identity?.Name;

            var chatCommandDto = new ChatCommandDto()
            {
                Username = username0,
                ConversationId = requestDto.ConversationId,
                Message = requestDto.Message,
                CorrelationId = requestDto.CorrelationId,
                Language = requestDto.Language
            };

            var aiChatResponse = await mediator.Send(chatCommandDto);

            return TypedResults.Ok(aiChatResponse);

            var header = httpContext.Request.Headers["Authorization"];
            if (header.Count == 0)
            {
                return TypedResults.BadRequest(new List<ValidationFailure>()
                    { new() { ErrorMessage = "No Authorization header foun" } });
            }

            // read the token from the header
            var accessToken = header[0]?.Replace("Bearer ", "");
            var handler = new JsonWebTokenHandler();
            if (!handler.CanReadToken(accessToken)) return TypedResults.BadRequest(new List<ValidationFailure>()
                { new() { ErrorMessage = "invalid token" } });
            
            var token = handler.ReadJsonWebToken(accessToken);
            var username = token.Claims.First(c => c.Type == "name").Value;
        });

        return endpoints;
    }
}