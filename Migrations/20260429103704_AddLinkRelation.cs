using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShorter.Migrations
{
    /// <inheritdoc />
    public partial class AddLinkRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "links",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_links_user_id",
                table: "links",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_links_users_user_id",
                table: "links",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_links_users_user_id",
                table: "links");

            migrationBuilder.DropIndex(
                name: "IX_links_user_id",
                table: "links");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "links");
        }
    }
}
