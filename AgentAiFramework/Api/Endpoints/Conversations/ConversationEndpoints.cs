using AgentFrameworkChat.Endpoints.Conversations.Filters;
using Application.Enums;
using Application.Features.Chat;
using Application.Features.GetUserConversations;
using Application.Models;
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
            .WithTags("Conversations")
            .RequireAuthorization()
            .AddEndpointFilter<ConversationEndpointFilter>();
        // TODO: require auth
        // TODO: add endpoint filter
        // TODO: validator

        group.MapPost("", async Task<Results<Ok<ChatCommandResponseDto>, BadRequest<List<ValidationFailure>>>>
        (HttpContext httpContext, IMediator mediator, /*, IValidator<ChatRequestDto> validator, */
            [FromBody] ChatRequestDto requestDto, [FromQuery] Language language) =>
        {
            // var validationResult = await validator.ValidateAsync(requestDto);
            // if (!validationResult.IsValid)
            // {
            //     return TypedResults.BadRequest(validationResult.Errors);
            // }

            var username0 = httpContext.User.Identity?.Name;

            var chatCommandDto = new ChatCommandDto()
            {
                Username = "maxou",
                ConversationId = requestDto.ConversationId,
                Message = requestDto.Message,
                CorrelationId = requestDto.CorrelationId,
                Language = language
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
            if (!handler.CanReadToken(accessToken))
                return TypedResults.BadRequest(new List<ValidationFailure>()
                    { new() { ErrorMessage = "invalid token" } });

            var token = handler.ReadJsonWebToken(accessToken);
            var username = token.Claims.First(c => c.Type == "name").Value;
        });

        group.MapGet("", async Task<Results<Ok<GetUserConversationsResponseDto>, ValidationProblem>> (HttpContext context,
            IMediator mediator) =>
        {
            var dto = new GetUserConversationsRequestDto
            {
                Username = context.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value!
            };

            var response = await mediator.Send(dto);

            return TypedResults.Ok(response);
        });

        return endpoints;
    }
}