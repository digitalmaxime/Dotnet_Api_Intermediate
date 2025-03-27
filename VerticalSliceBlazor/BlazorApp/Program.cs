using Application.Extensions;
using Application.Features.GetUserList;
using MediatR;
using Persistence.Extensions;
using VerticalSliceBlazor.Components;
using VerticalSliceBlazor.Components.Pages.GetUserList;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.RegisterPersistence(builder.Configuration);
builder.Services.RegisterApplication();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/blazorapi/users", async (IMediator mediator) =>
{
    var users = await mediator.Send(new GetUserListQuery());
    var usersViewModel = users.Users.Select(u => new UserViewModel { Name = u.Name }).ToArray();
    var userListViewModel = new GetUserListViewModel(usersViewModel, users.TotalNumberOfUsers,  users.PageSize, users.Page);
    return TypedResults.Ok(userListViewModel);
});

app.Run();