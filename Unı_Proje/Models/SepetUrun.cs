using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unı_Proje.Models
{
    public class SepetUrun
    {
        [Key]
        public int Id { get; set; }

        // Hangi Kullanıcının Sepeti?
        public int KullaniciId { get; set; }
        [ForeignKey("KullaniciId")]
        public Kullanici Kullanici { get; set; } = null!;

        // Hangi Ürün?
        public int UrunId { get; set; }
        [ForeignKey("UrunId")]
        public Urun Urun { get; set; } = null!;

        // Ne zaman ekledi?
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;

        // Özel Fiyat (Teklif kabul edilmişse bu fiyat kullanılır, null ise ürün fiyatı kullanılır)
        [Column(TypeName = "decimal(18,2)")]
        public decimal? OzelFiyat { get; set; }

        // Teklif ID (hangi teklif için eklendi?)
        public int? TeklifId { get; set; }
    }
}