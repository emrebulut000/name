using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uný_Proje.Migrations
{
    /// <inheritdoc />
    public partial class AddLatitudeLongitudeToUrun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "urunler",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "urunler",
                type: "double",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "urunler");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "urunler");
        }
    }
}
