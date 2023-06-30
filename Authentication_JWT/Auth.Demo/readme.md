In this example, the controller _NameController_ exposes 2 endpoints
```
[HttpGet]
[Route("GetValueByQueryParam")]
[HttpGet]
[Route("GetValueByPathParam/{id}")]
```
which are protected by the decorator `[Authorize]` on the class `NameController`

```
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class NameController: ControllerBase
{
    ...
}
```

In order to be authorized, the client must bear a token, which can be taken from the 
`[AllowAnonymous]
[HttpPost("authenticate")]
`
endpoint. 

---
In the startup, the following middleware
(taken from Microsoft.Extensions.DependencyInjection)
adds jwt authentication to protect the controller.
```
builder.Services.AddAuthentication(authOptions =>
{
    authOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
    };
});
```

---
<h2>Project References</h2>
```
        <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="7.0.8" />
        <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="7.0.5" />
        <PackageReference Include="Swashbuckle.AspNetCore" Version="6.4.0" />
        <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="6.31.0" />
```

---
<h2>Custom Authentication Handler</h2>
Should derive from IAuthenticationHandler.
