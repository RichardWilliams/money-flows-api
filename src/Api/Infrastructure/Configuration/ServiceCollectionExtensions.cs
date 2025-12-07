using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Behaviours;
using PropertyManagement.Api.Shared.Models;

namespace PropertyManagement.Api.Infrastructure.Configuration;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<Program>());

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<Program>(
            includeInternalTypes: true);

        // MediatR pipeline behaviors
        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));
        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));
        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(TransactionBehavior<,>));

        // AutoMapper
        services.AddAutoMapper(typeof(Program));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // DbContext
        var dbSettings = configuration
            .GetSection(nameof(DatabaseSettings))
            .Get<DatabaseSettings>()!;

        services.AddDbContext<PropertyManagementDbContext>(options =>
            options.UseNpgsql(
                dbSettings.ConnectionString,
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable(
                        "__EFMigrationsHistory",
                        "property_management");
                }));

        // Swagger/OpenAPI
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "UK Property Management API",
                Version = "1.0",
                Description = "Financial management system for UK rental properties",
                Contact = new OpenApiContact
                {
                    Name = "Support",
                    Email = "support@example.com"
                }
            });

            // XML comments for better documentation
            var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        // JSON serialization
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        return services;
    }
}
