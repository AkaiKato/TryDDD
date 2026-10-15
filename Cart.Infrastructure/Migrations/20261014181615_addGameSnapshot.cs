using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addGameSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameSnapshots",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    ForSale = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSnapshots", x => x.GameId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameSnapshots");
        }
    }
}
