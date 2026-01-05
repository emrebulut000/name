using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unı_Proje.Migrations
{
    /// <inheritdoc />
    public partial class ProfilDetaylariEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "kullanicilar",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Telefon",
                table: "kullanicilar",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "kullanicilar");

            migrationBuilder.DropColumn(
                name: "Telefon",
                table: "kullanicilar");
        }
    }
}
