using Microsoft.EntityFrameworkCore;

namespace PropertyManagement.Api.Infrastructure.Persistence;

public sealed class PropertyManagementDbContext : DbContext
{
    public PropertyManagementDbContext(DbContextOptions<PropertyManagementDbContext> options)
        : base(options)
    {
    }

    // DbSets will be added as features are implemented
    // public DbSet<PropertyEntity> Properties => Set<PropertyEntity>();
    // public DbSet<ParticipantEntity> Participants => Set<ParticipantEntity>();
    // public DbSet<MoneyFlowEntity> MoneyFlows => Set<MoneyFlowEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set default schema for new application tables
        modelBuilder.HasDefaultSchema("property_management");

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PropertyManagementDbContext).Assembly);

        // Create legacy schema for existing data
        modelBuilder.HasSequence("money_flow_id_seq", "home_accounts");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Enable sensitive data logging in development
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }
    }
}
