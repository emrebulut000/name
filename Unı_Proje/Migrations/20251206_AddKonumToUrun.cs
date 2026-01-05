using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uný_Proje.Migrations
{
    /// <inheritdoc />
    public partial class AddKonumToUrun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Konum",
                table: "urunler",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Konum",
                table: "urunler");
        }
    }
}
