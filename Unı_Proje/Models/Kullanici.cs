using System;
using System.ComponentModel.DataAnnotations; // Key attribute için gerekli

namespace Unı_Proje.Models
{
    public class Kullanici
    {
        [Key] // Birincil anahtar olduğunu belirtir
        public int Id { get; set; }

        public string KullaniciAdi { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string SifreHash { get; set; } = string.Empty;

        public string? ProfilResmiUrl { get; set; }

        // 👇 YENİ EKLENEN ALANLAR (Profil Sayfası İçin Şart)
        public string? Telefon { get; set; }

        public string? Bio { get; set; } // Hakkımda yazısı

        // Varsayılan değer atadık: Yeni üye olunca otomatik tarih atar.
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        // Admin yetkisi (varsayılan: false)
        public bool IsAdmin { get; set; } = false;

        // 🌟 SATICI PUANLAMA SİSTEMİ
        // Ortalama satıcı puanı (hesaplanmış değer)
        public decimal? OrtalamaPuan { get; set; }

        // Toplam değerlendirme sayısı
        public int ToplamDegerlendirme { get; set; } = 0;
    }
}