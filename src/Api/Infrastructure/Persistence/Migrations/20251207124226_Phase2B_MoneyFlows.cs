using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyManagement.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase2B_MoneyFlows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "money_flows",
                schema: "property_management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ExpenseCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    IncomeSource = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    LeaseId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_money_flows", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_money_flows_date",
                schema: "property_management",
                table: "money_flows",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "idx_money_flows_expense_category",
                schema: "property_management",
                table: "money_flows",
                column: "ExpenseCategoryId");

            migrationBuilder.CreateIndex(
                name: "idx_money_flows_property_date",
                schema: "property_management",
                table: "money_flows",
                columns: new[] { "PropertyId", "Date" });

            migrationBuilder.CreateIndex(
                name: "idx_money_flows_tenant",
                schema: "property_management",
                table: "money_flows",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "idx_money_flows_type",
                schema: "property_management",
                table: "money_flows",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "money_flows",
                schema: "property_management");
        }
    }
}
