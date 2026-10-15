using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class dropLicenseKeyOrderRow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicenseKeyRows_Orders_OrderRowId",
                table: "LicenseKeyRows");

            migrationBuilder.DropIndex(
                name: "IX_LicenseKeyRows_OrderRowId",
                table: "LicenseKeyRows");

            migrationBuilder.DropColumn(
                name: "OrderRowId",
                table: "LicenseKeyRows");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderRowId",
                table: "LicenseKeyRows",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LicenseKeyRows_OrderRowId",
                table: "LicenseKeyRows",
                column: "OrderRowId");

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseKeyRows_Orders_OrderRowId",
                table: "LicenseKeyRows",
                column: "OrderRowId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
