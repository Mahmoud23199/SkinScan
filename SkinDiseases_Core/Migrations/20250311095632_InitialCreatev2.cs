using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkinScan_Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreatev2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatbotQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    NextQuestionId = table.Column<int>(type: "int", nullable: true),
                    BotResponse = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatbotQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatbotQuestion_ChatbotQuestion_NextQuestionId",
                        column: x => x.NextQuestionId,
                        principalTable: "ChatbotQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatbotQuestion_ChatbotQuestion_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ChatbotQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Diseases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Symptoms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Causes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Treatments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    References1 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diseases", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatbotQuestion_NextQuestionId",
                table: "ChatbotQuestion",
                column: "NextQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatbotQuestion_ParentId",
                table: "ChatbotQuestion",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatbotQuestion");

            migrationBuilder.DropTable(
                name: "Diseases");
        }
    }
}
