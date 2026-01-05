using System.ComponentModel.DataAnnotations;

namespace Uný_Proje.DTOs
{
    // Deðerlendirme oluþturma için
    public class DegerlendirmeOlusturDto
    {
        [Required]
        public int SiparisId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasýnda olmalýdýr.")]
        public int Puan { get; set; }

        [MaxLength(500, ErrorMessage = "Yorum en fazla 500 karakter olabilir.")]
        public string? Yorum { get; set; }
    }

    // Deðerlendirme listesi için
    public class DegerlendirmeDto
    {
        public int Id { get; set; }
        public int SaticiId { get; set; }
        public string SaticiAdi { get; set; } = string.Empty;
        public int DegerlendirenId { get; set; }
        public string DegerlendirenAdi { get; set; } = string.Empty;
        public int Puan { get; set; }
        public string? Yorum { get; set; }
        public DateTime DegerlendirmeTarihi { get; set; }
    }

    // Satýcý profili için özet
    public class SaticiPuanOzetiDto
    {
        public int SaticiId { get; set; }
        public string SaticiAdi { get; set; } = string.Empty;
        public decimal? OrtalamaPuan { get; set; }
        public int ToplamDegerlendirme { get; set; }
        public int Puan5Yildiz { get; set; }
        public int Puan4Yildiz { get; set; }
        public int Puan3Yildiz { get; set; }
        public int Puan2Yildiz { get; set; }
        public int Puan1Yildiz { get; set; }
    }

    // Satýcý profil sayfasý için tam DTO
    public class SaticiProfilDto
    {
        public int SaticiId { get; set; }
        public string SaticiAdi { get; set; } = string.Empty;
        public string? ProfilResmi { get; set; }
        public string? Bio { get; set; }
        public string? Telefon { get; set; }
        public DateTime UyelikTarihi { get; set; }
        public decimal? OrtalamaPuan { get; set; }
        public int ToplamDegerlendirme { get; set; }
        public int Puan5Yildiz { get; set; }
        public int Puan4Yildiz { get; set; }
        public int Puan3Yildiz { get; set; }
        public int Puan2Yildiz { get; set; }
        public int Puan1Yildiz { get; set; }
        public int ToplamUrunSayisi { get; set; }
        public List<SaticiUrunDto> Urunler { get; set; } = new();
        public List<SaticiDegerlendirmeDto> Degerlendirmeler { get; set; } = new();
    }

    // Satýcý ürün kartý için
    public class SaticiUrunDto
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public string? ResimUrl { get; set; }
        public string KategoriAdi { get; set; } = string.Empty;
        public DateTime EklemeTarihi { get; set; }
        public int StokMiktari { get; set; }
    }

    // Satýcý deðerlendirme kartý için
    public class SaticiDegerlendirmeDto
    {
        public string DegerlendirenAdi { get; set; } = string.Empty;
        public int Puan { get; set; }
        public string? Yorum { get; set; }
        public DateTime Tarih { get; set; }
    }
}
