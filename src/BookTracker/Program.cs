using System.Text;
using BookTracker.Common;
using BookTracker.MongoDb.Extensions;
using BookTracker.Services;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApplicationSetting>(builder.Configuration);
var options = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationSetting>>().Value;

builder
    .ConfigureMongoDb(options)
    .ConfigureMongoDbEntities();

builder.Services.AddSingleton<JwtTokenService>();

builder.Services.AddCarter();

builder.Services.AddAuthentication(authenticationOptions =>
{
    authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwtOptions =>
{
    var jwtConfiguration = options.JwtConfiguration;

    jwtOptions.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfiguration.Issuer,
        ValidAudience = jwtConfiguration.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtConfiguration.Key))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseAuthentication();
app.UseAuthorization();

await app.UseMongoDbEntitiesAsync();

app.MapCarter();

app.Run();