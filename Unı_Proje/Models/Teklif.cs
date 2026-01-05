using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uný_Proje.Models
{
    public class Teklif
    {
        [Key]
        public int Id { get; set; }

        public int UrunId { get; set; }
        
        [ForeignKey("UrunId")]
        public Urun Urun { get; set; } = null!;

        public int AliciId { get; set; }
        
        [ForeignKey("AliciId")]
        public Kullanici Alici { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TeklifTutari { get; set; }

        public string? Aciklama { get; set; } // "1000 TL olur mu?" gibi mesaj

        public TeklifDurumu Durum { get; set; } = TeklifDurumu.Bekliyor;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        public DateTime? YanitTarihi { get; set; }

        public string? SaticiYaniti { get; set; } // Satýcýnýn yanýt mesajý
    }

    // Teklif Durumlarý
    public enum TeklifDurumu
    {
        Bekliyor = 0,      // Satýcý henüz yanýt vermedi
        KabulEdildi = 1,   // Satýcý teklifi kabul etti
        Reddedildi = 2,    // Satýcý teklifi reddetti
        IptalEdildi = 3    // Alýcý teklifi iptal etti
    }

    // Teklif Durumu Türkçe Çevirileri
    public static class TeklifDurumuExtensions
    {
        public static string ToTurkce(this TeklifDurumu durum)
        {
            return durum switch
            {
                TeklifDurumu.Bekliyor => "Yanýt Bekleniyor",
                TeklifDurumu.KabulEdildi => "Kabul Edildi",
                TeklifDurumu.Reddedildi => "Reddedildi",
                TeklifDurumu.IptalEdildi => "Ýptal Edildi",
                _ => "Bilinmiyor"
            };
        }

        public static string ToCssClass(this TeklifDurumu durum)
        {
            return durum switch
            {
                TeklifDurumu.Bekliyor => "warning",
                TeklifDurumu.KabulEdildi => "success",
                TeklifDurumu.Reddedildi => "danger",
                TeklifDurumu.IptalEdildi => "secondary",
                _ => "light"
            };
        }
    }
}
