using System.Text;
using AppJwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ***************** Reading Configuration AppSettings

var validIssuerConfig = builder.Configuration.GetValue<string>("Security:ValidIssuer");
Console.WriteLine("---------------");
Console.WriteLine(validIssuerConfig);
Console.WriteLine("---------------");


var myKeyValue = builder.Configuration["MyKey"];
var title = builder.Configuration["Position:Title"];
var name = builder.Configuration["Position:Name"];
var defaultLogLevel = builder.Configuration["Logging:LogLevel:Default"];
Console.WriteLine($"MyKey value: {myKeyValue} \n" +
                  $"Title: {title} \n" +
                  $"Name: {name} \n" +
                  $"Default Log Level: {defaultLogLevel}");


// ************ Reading Configuration AppSettings Options Pattern
/*
var positionOptions = new PositionOptions();
builder.Configuration.GetSection(PositionOptions.Position).Bind(positionOptions);

Console.WriteLine($"Title: {positionOptions.Title} \n" +
                  $"Name: {positionOptions.Name}");

positionOptions = builder.Configuration.GetSection(PositionOptions.Position)
    .Get<PositionOptions>();

Console.WriteLine($"Title: {positionOptions.Title} \n" +
                  $"Name: {positionOptions.Name}");
*/
builder.Services.Configure<PositionOptions>(
    builder.Configuration.GetSection(PositionOptions.Position));


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetValue<string>("Security:ValidIssuer"),
            // ValidIssuer = "apiWithAuthBackend",
            ValidAudience = "apiWithAuthBackend",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("!SomethingSecret!")
            )
        };
    });
    

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

app.Run();