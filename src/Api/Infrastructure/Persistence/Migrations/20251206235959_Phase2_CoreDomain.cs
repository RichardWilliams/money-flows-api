using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PropertyManagement.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase2_CoreDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "property_management");

            migrationBuilder.EnsureSchema(
                name: "home_accounts");

            migrationBuilder.CreateSequence(
                name: "money_flow_id_seq",
                schema: "home_accounts");

            migrationBuilder.CreateTable(
                name: "expense_attachments",
                schema: "property_management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpenseId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    StoragePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_attachments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "expense_categories",
                schema: "property_management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "expenses",
                schema: "property_management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Vendor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "leases",
                schema: "property_management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    MonthlyRent = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    DepositAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    RentDayOfMonth = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "properties",
                schema: "property_management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AddressLine1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AddressLine2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    County = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Postcode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Bedrooms = table.Column<int>(type: "integer", nullable: false),
                    Bathrooms = table.Column<int>(type: "integer", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    PurchaseDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_properties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                schema: "property_management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmergencyContact = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EmergencyPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "property_management",
                table: "expense_categories",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "MAINTENANCE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Property maintenance and repair costs", true, "Maintenance & Repairs", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-111111111112"), "UTILITIES", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gas, electricity, water, and other utilities", true, "Utilities", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-111111111113"), "INSURANCE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Property and landlord insurance", true, "Insurance", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-111111111114"), "COUNCIL_TAX", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Council tax payments", true, "Council Tax", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-111111111115"), "MORTGAGE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mortgage interest payments", true, "Mortgage Interest", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-111111111116"), "SERVICE_CHARGE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Building or management service charges", true, "Service Charges", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-111111111117"), "GROUND_RENT", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ground rent payments", true, "Ground Rent", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-111111111118"), "AGENT_FEES", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Letting agent fees and commissions", true, "Letting Agent Fees", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-111111111119"), "LEGAL", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Legal and professional fees", true, "Legal & Professional", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-11111111111a"), "FURNISHINGS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Furniture and white goods", true, "Furnishings", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-11111111111b"), "GARDENING", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Garden maintenance and landscaping", true, "Gardening", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-11111111111c"), "ADVERTISING", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Property advertising costs", true, "Advertising", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-1111-1111-1111-11111111111d"), "OTHER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Miscellaneous expenses", true, "Other", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_expense_attachments_ExpenseId",
                schema: "property_management",
                table: "expense_attachments",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_expense_categories_Code",
                schema: "property_management",
                table: "expense_categories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_expense_categories_IsActive",
                schema: "property_management",
                table: "expense_categories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_CategoryId",
                schema: "property_management",
                table: "expenses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_Date",
                schema: "property_management",
                table: "expenses",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_PropertyId",
                schema: "property_management",
                table: "expenses",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_leases_PropertyId",
                schema: "property_management",
                table: "leases",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_leases_PropertyId_Status",
                schema: "property_management",
                table: "leases",
                columns: new[] { "PropertyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_leases_Status",
                schema: "property_management",
                table: "leases",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_leases_TenantId",
                schema: "property_management",
                table: "leases",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_properties_Postcode",
                schema: "property_management",
                table: "properties",
                column: "Postcode");

            migrationBuilder.CreateIndex(
                name: "IX_properties_Status",
                schema: "property_management",
                table: "properties",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_Email",
                schema: "property_management",
                table: "tenants",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_LastName",
                schema: "property_management",
                table: "tenants",
                column: "LastName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "expense_attachments",
                schema: "property_management");

            migrationBuilder.DropTable(
                name: "expense_categories",
                schema: "property_management");

            migrationBuilder.DropTable(
                name: "expenses",
                schema: "property_management");

            migrationBuilder.DropTable(
                name: "leases",
                schema: "property_management");

            migrationBuilder.DropTable(
                name: "properties",
                schema: "property_management");

            migrationBuilder.DropTable(
                name: "tenants",
                schema: "property_management");

            migrationBuilder.DropSequence(
                name: "money_flow_id_seq",
                schema: "home_accounts");
        }
    }
}
