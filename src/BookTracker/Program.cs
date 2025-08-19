using BookTracker.MongoDb.Extensions;
using Carter;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder
    .ConfigureMongoDb()
    .ConfigureMongoDbEntities();

builder.Services.AddCarter();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

await app.UseMongoDbEntitiesAsync();

app.MapCarter();

app.Run();