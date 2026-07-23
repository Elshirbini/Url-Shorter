using Microsoft.EntityFrameworkCore.Migrations;
using UrlShorter.Modules.Users.Infrastructure.Enums;

#nullable disable

namespace UrlShorter.Migrations
{
    /// <inheritdoc />
    public partial class AddNullabeRoleColumnToUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:user_role", "admin,user");

            migrationBuilder.AddColumn<UserRole>(
                name: "role",
                table: "users",
                type: "user_role",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "role",
                table: "users");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:user_role", "admin,user");
        }
    }
}
