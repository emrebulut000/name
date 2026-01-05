using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unı_Proje.Migrations
{
    /// <inheritdoc />
    public partial class UrunVeKategoriModelleriGuncellendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Aciklama",
                table: "urunler",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "EklemeTarihi",
                table: "urunler",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Fiyat",
                table: "urunler",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "KategoriId",
                table: "urunler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KullaniciId",
                table: "urunler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_urunler_KategoriId",
                table: "urunler",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_urunler_KullaniciId",
                table: "urunler",
                column: "KullaniciId");

            migrationBuilder.AddForeignKey(
                name: "FK_urunler_kategoriler_KategoriId",
                table: "urunler",
                column: "KategoriId",
                principalTable: "kategoriler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_urunler_kullanicilar_KullaniciId",
                table: "urunler",
                column: "KullaniciId",
                principalTable: "kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_urunler_kategoriler_KategoriId",
                table: "urunler");

            migrationBuilder.DropForeignKey(
                name: "FK_urunler_kullanicilar_KullaniciId",
                table: "urunler");

            migrationBuilder.DropIndex(
                name: "IX_urunler_KategoriId",
                table: "urunler");

            migrationBuilder.DropIndex(
                name: "IX_urunler_KullaniciId",
                table: "urunler");

            migrationBuilder.DropColumn(
                name: "Aciklama",
                table: "urunler");

            migrationBuilder.DropColumn(
                name: "EklemeTarihi",
                table: "urunler");

            migrationBuilder.DropColumn(
                name: "Fiyat",
                table: "urunler");

            migrationBuilder.DropColumn(
                name: "KategoriId",
                table: "urunler");

            migrationBuilder.DropColumn(
                name: "KullaniciId",
                table: "urunler");
        }
    }
}
