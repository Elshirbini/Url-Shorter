using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShorter.Migrations
{
    /// <inheritdoc />
    public partial class AddLinksAndClicks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "links",
                columns: table => new
                {
                    link_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    redirect_url = table.Column<string>(type: "text", nullable: false),
                    clicks = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_links", x => x.link_id);
                    table.ForeignKey(
                        name: "FK_links_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "clicks",
                columns: table => new
                {
                    click_id = table.Column<Guid>(type: "uuid", nullable: false),
                    link_id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    referer = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clicks", x => x.click_id);
                    table.ForeignKey(
                        name: "FK_clicks_links_link_id",
                        column: x => x.link_id,
                        principalTable: "links",
                        principalColumn: "link_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_clicks_link_id",
                table: "clicks",
                column: "link_id");

            migrationBuilder.CreateIndex(
                name: "IX_links_category_id",
                table: "links",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_links_code",
                table: "links",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clicks");

            migrationBuilder.DropTable(
                name: "links");
        }
    }
}
