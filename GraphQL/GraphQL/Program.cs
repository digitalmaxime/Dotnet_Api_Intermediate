using GraphQL.Infra.Extensions;
using GraphQL.Persistence.Context;
using GraphQL.Persistence.Extensions;
using GraphQL.Server.Ui.Voyager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterPersistenceServices(builder.Configuration);
builder.Services.RegisterGraphQlServices();

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    builder.Services.EnsureDatabaseCreated();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapGraphQL();
app.UseGraphQLVoyager("/graphql-voyager", new VoyagerOptions()
{
    GraphQLEndPoint = "/graphql"
});


app.MapGet("/", async (HttpContext context, DemoDbContext dbContext) =>
{
    context.Response.Redirect("./swagger/index.html", permanent: false);

    return Task.FromResult(0);
});


app.Run();