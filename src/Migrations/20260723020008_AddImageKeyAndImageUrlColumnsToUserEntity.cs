using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShorter.src.Migrations
{
    /// <inheritdoc />
    public partial class AddImageKeyAndImageUrlColumnsToUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image_key",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_key",
                table: "users");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "users");
        }
    }
}
