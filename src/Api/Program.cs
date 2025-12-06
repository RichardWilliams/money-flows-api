using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Infrastructure.Configuration;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;
using PropertyManagement.Api.Shared.Models;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog (IMPORTANT: Use for k8s log scraping)
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "PropertyManagement")
    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
    .WriteTo.Console(new CompactJsonFormatter())  // JSON for log aggregation
    .WriteTo.File(
        "logs/app-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"));

// Configuration binding with validation
builder.Services.AddOptions<DatabaseSettings>()
    .BindConfiguration(nameof(DatabaseSettings))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<AttachmentSettings>()
    .BindConfiguration(nameof(AttachmentSettings))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<CorsSettings>()
    .BindConfiguration(nameof(CorsSettings))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// CORS
var corsSettings = builder.Configuration
    .GetSection(nameof(CorsSettings))
    .Get<CorsSettings>()!;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy => policy
        .WithOrigins(corsSettings.AllowedOrigins)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("X-Deprecated-Warning"));  // For sunset warnings
});

// Infrastructure
builder.Services.ConfigureInfrastructure(builder.Configuration);

// Health checks
var connectionString = builder.Configuration
    .GetSection(nameof(DatabaseSettings))
    .Get<DatabaseSettings>()!
    .ConnectionString;

builder.Services
    .AddHealthChecks()
    .AddNpgSql(
        connectionString,
        name: "database",
        tags: new[] { "db", "postgresql", "ready" })
    .AddCheck(
        "api",
        () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API is running"),
        tags: new[] { "api", "live" });

var app = builder.Build();

// Swagger (always on for now, will restrict later)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Property Management API");
    options.DisplayRequestDuration();
    options.EnableTryItOutByDefault();
});

// Request logging
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
    };
});

// Database migrations on startup
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<PropertyManagementDbContext>();
        logger.LogInformation("Applying database migrations...");
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("Database migrations completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error applying database migrations");
        throw;
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// Health check endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Global exception handling
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var exceptionHandlerPathFeature =
        context.Features.Get<IExceptionHandlerPathFeature>();
    var exception = exceptionHandlerPathFeature?.Error;

    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogError(exception, "Unhandled exception occurred");

    var problemDetails = exception switch
    {
        ValidationException validationEx => new ValidationProblemDetails(
            validationEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()))
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        },

        NotFoundException notFoundEx => new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Resource not found",
            Detail = notFoundEx.Message,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
        },

        _ => new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred",
            Detail = app.Environment.IsDevelopment() ? exception?.Message : "Internal server error",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        }
    };

    context.Response.StatusCode = problemDetails.Status ?? 500;
    await context.Response.WriteAsJsonAsync(problemDetails);
}));

// Feature endpoints will be registered here
// PropertyManagement.Features.Properties.Configure.Endpoints(app);
// PropertyManagement.Features.Participants.Configure.Endpoints(app);
// PropertyManagement.Features.MoneyFlows.Configure.Endpoints(app);
// PropertyManagement.Features.Reports.Configure.Endpoints(app);

app.Run();

// Make Program accessible to tests
public partial class Program { }
