using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PingBoard.Migrations
{
    /// <inheritdoc />
    public partial class ProbeTypeUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProbeType",
                table: "ProbeResults",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProbeType",
                table: "ProbeResults");
        }
    }
}
