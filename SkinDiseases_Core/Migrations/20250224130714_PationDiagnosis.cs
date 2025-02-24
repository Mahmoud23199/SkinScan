using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkinScan_Core.Migrations
{
    /// <inheritdoc />
    public partial class PationDiagnosis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Pations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PationDiagnosis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Diagnosis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PationDiagnosis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PationDiagnosis_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PationDiagnosis_UserId",
                table: "PationDiagnosis",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PationDiagnosis");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "Pations");
        }
    }
}
