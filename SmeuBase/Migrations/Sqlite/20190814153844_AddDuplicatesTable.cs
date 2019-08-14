using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmeuBase.Migrations.Sqlite
{
    public partial class AddDuplicatesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DuplicateId",
                table: "Suspensions",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "Revoker",
                table: "Suspensions",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "Suspender",
                table: "Suspensions",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateTable(
                name: "Duplicates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Author = table.Column<ulong>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    OriginalId = table.Column<int>(nullable: false),
                    SuspensionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duplicates", x => x.Id);
                    table.ForeignKey(
                        name: "ForeignKey_Duplicate_Submission",
                        column: x => x.OriginalId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "ForeignKey_Duplicate_Suspension",
                        column: x => x.SuspensionId,
                        principalTable: "Suspensions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Duplicates_OriginalId",
                table: "Duplicates",
                column: "OriginalId");

            migrationBuilder.CreateIndex(
                name: "IX_Duplicates_SuspensionId",
                table: "Duplicates",
                column: "SuspensionId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Duplicates");

            migrationBuilder.DropColumn(
                name: "DuplicateId",
                table: "Suspensions");

            migrationBuilder.DropColumn(
                name: "Revoker",
                table: "Suspensions");

            migrationBuilder.DropColumn(
                name: "Suspender",
                table: "Suspensions");
        }
    }
}
