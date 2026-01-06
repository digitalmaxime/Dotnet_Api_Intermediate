using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AgentFrameworkChat.Extensions.OpenApi;

public class ConfigureScalarOptions : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Api",
            Version = "v1",
            Description = "Chat API",
            Contact = new OpenApiContact
            {
                Name = "John Doe",
                Email = "john.doe@email.com"
            }
        });
        options.CustomSchemaIds(type => (type.FullName ?? type.ToString()).Replace('+', '.'));
    }
}