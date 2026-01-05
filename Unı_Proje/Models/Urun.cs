using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unı_Proje.Models
{
    public class Urun
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Ad { get; set; } = string.Empty;

        public string Aciklama { get; set; } = string.Empty;

        public decimal Fiyat { get; set; }

        public DateTime EklemeTarihi { get; set; }

        // Resim URL
        public string? ResimUrl { get; set; }

        // Ürün konumu (şehir/ilçe veya serbest metin)
        public string? Konum { get; set; }

        // 🌍 KONUM KOORDİNATLARI (Mesafe hesaplama için)
        public double? Latitude { get; set; }  // Enlem
        public double? Longitude { get; set; } // Boylam

        // 🔗 SEO-FRIENDLY URL (Slug)
        [Required]
        [StringLength(200)]
        public string Slug { get; set; } = string.Empty;

        // 📦 STOK YÖNETİMİ
        [Required]
        public int StokMiktari { get; set; } = 0;

        // Stokta var mı kontrolü (Computed Property)
        [NotMapped]
        public bool StokVarMi => StokMiktari > 0;

        // ----- İLİŞKİLER (FOREIGN KEYS) -----

        // Kategori İlişkisi
        public int KategoriId { get; set; }

        [ForeignKey("KategoriId")]
        public Kategori Kategori { get; set; } = null!;

        // Kullanıcı (Satıcı) İlişkisi
        public int KullaniciId { get; set; }

        [ForeignKey("KullaniciId")]
        public Kullanici Kullanici { get; set; } = null!;

        // 📸 ÇOKLU RESİM İLİŞKİSİ
        public virtual ICollection<UrunResim>? Resimler { get; set; }
    }
}