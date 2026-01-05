using System;

namespace Unı_Proje.Web.Client.Models
{
    // Ana sayfa ve listeleme için kullanılan ViewModel
    public class UrunViewModel
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public DateTime EklemeTarihi { get; set; }
        public string? ResimUrl { get; set; }
        public int KullaniciId { get; set; }
        public string? Konum { get; set; }
        public string Slug { get; set; } = string.Empty; // 🔗 SEO-friendly URL
        public int StokMiktari { get; set; }
        public bool StokVarMi { get; set; }
        public KategoriViewModel? Kategori { get; set; }
    }

    // Kategori bilgisi için
    public class KategoriViewModel
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
    }

    // Yeni ürün ekleme için
    public class YeniUrunViewModel
    {
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public int KategoriId { get; set; }
        public string ResimUrl { get; set; } = string.Empty;
        public int StokMiktari { get; set; } = 0;
        public string? Konum { get; set; } = string.Empty;
    }

    // Ürün güncelleme için
    public class UrunGuncelleViewModel
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public int KategoriId { get; set; }
        public int StokMiktari { get; set; } = 0;
        public string? Konum { get; set; } = string.Empty;
    }

    // Resim upload sonucu için
    public class ResimUploadSonuc
    {
        public string Url { get; set; } = string.Empty;
    }
}