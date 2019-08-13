using Microsoft.EntityFrameworkCore.Migrations;

namespace SmeuBase.Migrations.Sqlite
{
    public partial class SubmissionsAddMessageId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "MessageId",
                table: "Submissions",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "Submissions");
        }
    }
}
