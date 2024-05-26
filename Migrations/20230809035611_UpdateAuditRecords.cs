using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShinyEggs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewValues",
                table: "AuditRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginalValues",
                table: "AuditRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewValues",
                table: "AuditRecords");

            migrationBuilder.DropColumn(
                name: "OriginalValues",
                table: "AuditRecords");
        }
    }
}
