using Domain;

namespace Application.Features.GetUserList;

public record GetUserList(IEnumerable<User> Users, int TotalNumberOfUsers, int PageSize, int Page)
{
    public int TotalPages => (int)Math.Ceiling(TotalNumberOfUsers / (double)PageSize);
}