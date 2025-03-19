using Duende.IdentityServer.Models;

namespace IdentityServerProject;

public static class Config
{
    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new Client
            {
                ClientId = "angular-client",
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { "http://localhost:4200/signin-oidc" },
                PostLogoutRedirectUris = { "http://localhost:4200" },
                AllowedScopes = { "openid", "profile", "api1" },
                RequirePkce = true,
                AllowAccessTokensViaBrowser = true
            }
        };

    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("api1", "My API")
        };
}