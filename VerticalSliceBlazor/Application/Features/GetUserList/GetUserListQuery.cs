using MediatR;

namespace Application.Features.GetUserList;

public record GetUserListQuery(int PageSize = 10, int Page = 0) : IRequest<GetUserList>;