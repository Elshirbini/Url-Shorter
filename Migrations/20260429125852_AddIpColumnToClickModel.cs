using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShorter.Migrations
{
    /// <inheritdoc />
    public partial class AddIpColumnToClickModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ip",
                table: "clicks",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ip",
                table: "clicks");
        }
    }
}
