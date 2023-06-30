﻿using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Auth.Demo.Services.CustomAuthManager;

public class BasicAuthenticationOptions : AuthenticationSchemeOptions
{
    
}

public class CustomAuthHandler: AuthenticationHandler<BasicAuthenticationOptions>
{
    private readonly ICustomAuthManager _customAuthenticationManager;

    public CustomAuthHandler(
        IOptionsMonitor<BasicAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ICustomAuthManager customAuthenticationManager) : base(options, logger, encoder, clock)
    {
        _customAuthenticationManager = customAuthenticationManager;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
         // 1st, make sure that the authorization header available in the request - check that the request header contains the authorization key
         if (!Request.Headers.ContainsKey("Authorization"))
         {
             return AuthenticateResult.Fail("Unauthorized");
         }

         var authorizationHeader = Request.Headers["Authorization"];
         if (string.IsNullOrEmpty(authorizationHeader[0]))
         {
             return AuthenticateResult.Fail("Unauthorized");
         }

         if (!authorizationHeader[0]!.StartsWith("bearer", StringComparison.OrdinalIgnoreCase))
         {
             return AuthenticateResult.Fail("Unauthorized");
         }
         
         var token = authorizationHeader[0].Substring("bearer".Length).Trim();
         // var token = authorizationHeader[0].Split(" ")[0];
         if (string.IsNullOrEmpty(token))
         {
             return AuthenticateResult.Fail("Unauthorized");
         }

         try
         {
             return ValidateToken(token);
         }
         catch (Exception e)
         {
             Console.WriteLine(e);
             return AuthenticateResult.Fail(e.Message);
         }
        
    }

    private AuthenticateResult ValidateToken(string token)
    {
        var validatedToken = _customAuthenticationManager.Tokens.FirstOrDefault(t => t.Key == token);
        if (validatedToken.Key == null)
        {
            return AuthenticateResult.Fail("Unauthorized");
        }

        // Create an Identity
        // 1st create a claim
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, validatedToken.Value)
        };
        
        // 2nd create the identity
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new GenericPrincipal(identity, null);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}