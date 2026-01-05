using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unı_Proje.Models
{
    public class SiparisDetay
    {
        [Key]
        public int Id { get; set; }

        // Hangi siparişin detayı?
        public int SiparisId { get; set; }
        [ForeignKey("SiparisId")]
        public Siparis Siparis { get; set; } = null!;

        // Hangi ürün?
        public int UrunId { get; set; }
        [ForeignKey("UrunId")]
        public Urun Urun { get; set; } = null!;

        // Ürün adı (sipariş anındaki ad - ürün silinse bile görünsün)
        public string UrunAdi { get; set; } = string.Empty;

        // Birim fiyat (sipariş anındaki fiyat)
        public decimal BirimFiyat { get; set; }

        // Adet
        public int Adet { get; set; } = 1;

        // Ara toplam
        public decimal AraToplam => BirimFiyat * Adet;

        // Ürün resmi (sipariş anındaki resim)
        public string? ResimUrl { get; set; }
    }
}
