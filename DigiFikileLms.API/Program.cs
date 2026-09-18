using System.Text.Json.Serialization;
using DigiFikileLms.API.Extensions;
using DigiFikileLms.Application;
using DigiFikileLms.Infrastructure.Extensions;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);
// PORT CONFIGURATION FOR RENDER
// ================================================================
var port = Environment.GetEnvironmentVariable("PORT") ?? "5001";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var connectionString = Environment.GetEnvironmentVariable(
    "ConnectionStrings__DigiFikileLmsDb");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Configuration["ConnectionStrings:DigiFikileLmsDb"] = connectionString;
}

// Add services
// Enums travel as their names ("Open", "InReview", ...) in both directions, matching how they
// are stored in the database (HasConversion<string>) and shown in the UI.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDigiFikileLmsSwagger();
builder.Services.AddDigiFikileLmsAuthentication(builder.Configuration);

// Add Application Services (MediatR, FluentValidation, AutoMapper)
builder.Services.AddApplicationServices();

// Add Infrastructure Services
builder.Services.AddInfrastructureServices(builder.Configuration);
// CORS FOR PRODUCTION
// ================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DigiFikile LMS API v1");
        c.RoutePrefix = "swagger"; // Swagger at /swagger
    });

}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll"); //Enable CORS


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    // Use DigiFikileLmsDbContext
    var dbContext = scope.ServiceProvider.GetRequiredService<DigiFikileLmsDbContext>();

    // Apply migrations
    await dbContext.Database.MigrateAsync();
    Console.WriteLine("Database migrated successfully!");
}

// HEALTH CHECK ENDPOINT FOR RENDER
// ================================================================
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
}));


app.Run();

