using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace pcms.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Model_Updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Members",
                keyColumn: "MemberId",
                keyValue: "8aeb75b8-f6b3-4785-8fe4-0d3f7de195a0");

            migrationBuilder.DeleteData(
                table: "Members",
                keyColumn: "MemberId",
                keyValue: "fce9aa65-d18f-4532-852a-4e7b4dce0051");

            migrationBuilder.AddColumn<string>(
                name: "Employer",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsEligibleForBenefit",
                table: "Members",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RSAPin",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "Members",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Contributions",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeContribution",
                table: "Contributions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeIntrestAmount",
                table: "Contributions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "Contributions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "Contributions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MonthForContribution",
                table: "Contributions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Contributions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCumulative",
                table: "Contributions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearForContribution",
                table: "Contributions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "Contributions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Members",
                columns: new[] { "MemberId", "DateOfBirth", "Email", "Employer", "IsActive", "IsDeleted", "IsEligibleForBenefit", "Name", "Phone", "RSAPin", "RegistrationDate" },
                values: new object[,]
                {
                    { "45a99bb0-4d5c-4bba-b221-a15b453e2a11", new DateTime(1990, 8, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", "NNPC", true, false, false, "Jane Smith", "2349876543210", "PIN9993243989", new DateTime(2025, 3, 24, 22, 22, 5, 807, DateTimeKind.Local).AddTicks(1527) },
                    { "eae1e136-b9fa-4684-a01f-25dcdc68f25a", new DateTime(1985, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@example.com", "NDPR", true, false, false, "John Doe", "23480102468635", "PIN1234567890", new DateTime(2025, 3, 24, 22, 22, 5, 807, DateTimeKind.Local).AddTicks(1517) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Members",
                keyColumn: "MemberId",
                keyValue: "45a99bb0-4d5c-4bba-b221-a15b453e2a11");

            migrationBuilder.DeleteData(
                table: "Members",
                keyColumn: "MemberId",
                keyValue: "eae1e136-b9fa-4684-a01f-25dcdc68f25a");

            migrationBuilder.DropColumn(
                name: "Employer",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "IsEligibleForBenefit",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "RSAPin",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CumulativeContribution",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "CumulativeIntrestAmount",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "MonthForContribution",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "TotalCumulative",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "YearForContribution",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Contributions");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Contributions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.InsertData(
                table: "Members",
                columns: new[] { "MemberId", "DateOfBirth", "Email", "IsActive", "IsDeleted", "Name", "Phone" },
                values: new object[,]
                {
                    { "8aeb75b8-f6b3-4785-8fe4-0d3f7de195a0", new DateTime(1985, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@example.com", true, false, "John Doe", null },
                    { "fce9aa65-d18f-4532-852a-4e7b4dce0051", new DateTime(1990, 8, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", true, false, "Jane Smith", null }
                });
        }
    }
}
