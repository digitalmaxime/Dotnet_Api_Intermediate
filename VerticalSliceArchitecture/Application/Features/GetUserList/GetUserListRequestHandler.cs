using Application.Contracts.Persistence;
using MediatR;

namespace Application.Features.GetUserList;

public class GetUserListRequestHandler(IUserRepository userRepository) : IRequestHandler<GetUserListQuery, GetUserList>
{
    public async Task<GetUserList> Handle(GetUserListQuery request, CancellationToken cancellationToken)
    {
        var totalNumberOfUsers = userRepository.GetQueryableUsers().Count();
        
        var users = userRepository.GetQueryableUsers()
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .ToArray();
        
        return new GetUserList(users, totalNumberOfUsers, request.PageSize, request.Page);
    }
}