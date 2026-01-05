using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uný_Proje.Models
{
    public class Favori
    {
        [Key]
        public int Id { get; set; }

        // Hangi kullanýcýnýn favorisi
        public int KullaniciId { get; set; }
        [ForeignKey("KullaniciId")]
        public Kullanici Kullanici { get; set; } = null!;

        // Hangi ürün favorilere eklendi
        public int UrunId { get; set; }
        [ForeignKey("UrunId")]
        public Urun Urun { get; set; } = null!;

        // Ne zaman eklendi
        public DateTime EklemeTarihi { get; set; } = DateTime.Now;
    }
}
