using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace pcms.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    MemberId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.MemberId);
                });

            migrationBuilder.CreateTable(
                name: "Contributions",
                columns: table => new
                {
                    ContributionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MemberId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ContributionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributions", x => x.ContributionId);
                    table.ForeignKey(
                        name: "FK_Contributions_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Members",
                columns: new[] { "MemberId", "DateOfBirth", "Email", "IsActive", "IsDeleted", "Name", "Phone" },
                values: new object[,]
                {
                    { "8aeb75b8-f6b3-4785-8fe4-0d3f7de195a0", new DateTime(1985, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@example.com", true, false, "John Doe", null },
                    { "fce9aa65-d18f-4532-852a-4e7b4dce0051", new DateTime(1990, 8, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", true, false, "Jane Smith", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_MemberId",
                table: "Contributions",
                column: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contributions");

            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
