using AgentFrameworkChat.Endpoints.Conversations.Filters;
using Application.Enums;
using Application.Features.ApprovePizzaOrder;
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
            var chatCommandDto = new ChatCommandDto()
            {
                Username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value!,
                ConversationId = requestDto.ConversationId,
                Message = requestDto.Message,
                CorrelationId = requestDto.CorrelationId,
                Language = language
            };

            var aiChatResponse = await mediator.Send(chatCommandDto);

            return TypedResults.Ok(aiChatResponse);
        });

        group.MapGet("", async Task<Results<Ok<GetUserConversationsResponseDto>, ValidationProblem>> (
            HttpContext context,
            IMediator mediator) =>
        {
            var dto = new GetUserConversationsRequestDto
            {
                Username = context.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value!
            };

            var response = await mediator.Send(dto);

            return TypedResults.Ok(response);
        });

        group.MapPost("{conversationId}/approve",
            async Task<Results<Ok<ApprovePizzaOrderResponseDto>, ValidationProblem>> ([FromRoute] Guid conversationId,
                HttpContext context, IMediator mediator) =>
            {
                var username = context.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value!;

                var response = await mediator.Send(new ApprovePizzaOrderRequestDto()
                {
                    Username = username,
                    ConversationId = conversationId,
                    IsApproved = true,
                    Language = Language.En
                });

                return TypedResults.Ok(response);
            });

        return endpoints;
    }
}