namespace VerticalSliceBlazor.Components.Pages.GetUserList;

public record GetUserListViewModel(IEnumerable<UserViewModel> Users, int TotalNumberOfUsers, int PageSize, int Page)
{
    public int TotalPages => (int)Math.Ceiling(TotalNumberOfUsers / (double)PageSize);
}