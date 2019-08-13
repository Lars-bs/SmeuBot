using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmeuBase.Migrations.MySQL
{
    public partial class SuspensionsAddMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Suspensions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Suspensions",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Suspensions");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Suspensions");
        }
    }
}
