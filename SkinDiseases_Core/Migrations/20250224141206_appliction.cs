using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkinScan_Core.Migrations
{
    /// <inheritdoc />
    public partial class appliction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "PationDiagnosis",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PationDiagnosis_ApplicationUserId",
                table: "PationDiagnosis",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PationDiagnosis_AspNetUsers_ApplicationUserId",
                table: "PationDiagnosis",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PationDiagnosis_AspNetUsers_ApplicationUserId",
                table: "PationDiagnosis");

            migrationBuilder.DropIndex(
                name: "IX_PationDiagnosis_ApplicationUserId",
                table: "PationDiagnosis");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "PationDiagnosis");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");
        }
    }
}
