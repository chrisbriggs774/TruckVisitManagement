using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
using TruckVisitManagement.Api.Authentication;
using TruckVisitManagement.Api.Infrastructure;
using TruckVisitManagement.Application.Commands.DependencyInjection;
using TruckVisitManagement.Application.Queries.DependencyInjection;
using TruckVisitManagement.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Controllers + JSON: serialize enums as strings so the OpenAPI contract is human-readable.
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Truck Visit Management API",
        Version = "v1",
        Description = "API for creating, retrieving and searching truck visits at a terminal."
    });

    // Include XML comments for richer OpenAPI documentation.
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Advertise bearer-token authentication in the OpenAPI document so the Swagger UI
    // exposes an "Authorize" button and sends the token on each request.
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the bearer token in the format: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme)] = new List<string>()
    });
});

// Application (CQRS) handlers.
builder.Services.AddVisitCommandHandlers();
builder.Services.AddVisitQueryHandlers();

// Authentication & authorization.
// Default provider is the in-memory "Local" scheme. Set Authentication:Provider=Entra
// to switch to Microsoft Entra ID (JWT bearer) without code changes.
builder.Services.AddApiAuthentication(builder.Configuration);

// Infrastructure (database) implementations.
// Default provider is InMemory. Use Infrastructure:Provider=DynamoDbOpenSearch to switch
// to future DynamoDB/OpenSearch implementations.
var infrastructureProvider = builder.Configuration["Infrastructure:Provider"] ?? "InMemory";
builder.Services.AddInfrastructure(infrastructureProvider);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Truck Visit Management API v1");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Exposed so the API can be referenced by integration tests via WebApplicationFactory.
/// </summary>
public partial class Program
{
}
