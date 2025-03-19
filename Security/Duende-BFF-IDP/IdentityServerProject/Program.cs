using IdentityServerProject;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServer(options => { options.EmitStaticAudienceClaim = true; })
    .AddInMemoryClients(Config.Clients)
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiScopes(Config.ApiScopes);

var app = builder.Build();

// Middleware setup
app.UseRouting();
app.UseIdentityServer();
app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

app.MapGet("/", () => "Hello World!");

app.Run();