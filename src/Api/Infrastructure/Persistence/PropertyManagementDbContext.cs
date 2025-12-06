using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Features.Expenses.Domain;
using PropertyManagement.Api.Features.Properties.Domain;
using PropertyManagement.Api.Features.Tenants.Domain;

namespace PropertyManagement.Api.Infrastructure.Persistence;

public sealed class PropertyManagementDbContext : DbContext
{
    public PropertyManagementDbContext(DbContextOptions<PropertyManagementDbContext> options)
        : base(options)
    {
    }

    // DbSets for Phase 2 features
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<ExpenseAttachment> ExpenseAttachments => Set<ExpenseAttachment>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Lease> Leases => Set<Lease>();

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
