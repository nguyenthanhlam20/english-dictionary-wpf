using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishDictionary.Migrations
{
    public partial class Initialization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "WordTypes",
                columns: table => new
                {
                    WordTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WordTypeName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordTypes", x => x.WordTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    WordId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WordName = table.Column<string>(type: "TEXT", nullable: false),
                    WordTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    IPA = table.Column<string>(type: "TEXT", nullable: false),
                    IsSelected = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsUserSaved = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAdminSaved = table.Column<bool>(type: "INTEGER", nullable: false),
                    IconName = table.Column<string>(type: "TEXT", nullable: false),
                    UserSavedIconName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.WordId);
                    table.ForeignKey(
                        name: "FK_Words_WordTypes_WordTypeId",
                        column: x => x.WordTypeId,
                        principalTable: "WordTypes",
                        principalColumn: "WordTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WordExamples",
                columns: table => new
                {
                    ExampleId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExampleContent = table.Column<string>(type: "TEXT", nullable: false),
                    WordId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordExamples", x => x.ExampleId);
                    table.ForeignKey(
                        name: "FK_WordExamples_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "WordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WordMeanings",
                columns: table => new
                {
                    MeaningId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MeaningContent = table.Column<string>(type: "TEXT", nullable: false),
                    WordId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordMeanings", x => x.MeaningId);
                    table.ForeignKey(
                        name: "FK_WordMeanings_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "WordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WordExamples_WordId",
                table: "WordExamples",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_WordMeanings_WordId",
                table: "WordMeanings",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_WordTypeId",
                table: "Words",
                column: "WordTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "WordExamples");

            migrationBuilder.DropTable(
                name: "WordMeanings");

            migrationBuilder.DropTable(
                name: "Words");

            migrationBuilder.DropTable(
                name: "WordTypes");
        }
    }
}
