using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unı_Proje.Migrations
{
    /// <inheritdoc />
    public partial class SepetEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SepetUrunleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SepetUrunleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SepetUrunleri_kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SepetUrunleri_urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SepetUrunleri_KullaniciId",
                table: "SepetUrunleri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_SepetUrunleri_UrunId",
                table: "SepetUrunleri",
                column: "UrunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SepetUrunleri");
        }
    }
}
