using Microsoft.EntityFrameworkCore.Migrations;
using UrlShorter.src.Modules.Users.Infrastructure.Enums;

#nullable disable

namespace UrlShorter.src.Migrations
{
    /// <inheritdoc />
    public partial class MakeRoleColumnRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<UserRole>(
                name: "role",
                table: "users",
                type: "user_role",
                nullable: false,
                defaultValue: UserRole.Admin,
                oldClrType: typeof(UserRole),
                oldType: "user_role",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<UserRole>(
                name: "role",
                table: "users",
                type: "user_role",
                nullable: true,
                oldClrType: typeof(UserRole),
                oldType: "user_role");
        }
    }
}
