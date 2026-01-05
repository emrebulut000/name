using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uný_Proje.Models
{
    public class Siparis
    {
        [Key]
        public int Id { get; set; }

        // Hangi kullanýcýnýn sipariþi?
        public int KullaniciId { get; set; }
        [ForeignKey("KullaniciId")]
        public Kullanici Kullanici { get; set; } = null!;

        // Sipariþ tarihi
        public DateTime SiparisTarihi { get; set; } = DateTime.Now;

        // Toplam tutar
        public decimal ToplamTutar { get; set; }

        // Sipariþ durumu
        public SiparisDurumu Durum { get; set; } = SiparisDurumu.Onaylandi;

        // Teslimat bilgileri (opsiyonel)
        public string? TeslimatAdresi { get; set; }
        public string? TeslimatTelefonu { get; set; }

        // Sipariþ detaylarý (1-N iliþki)
        public List<SiparisDetay> SiparisDetaylari { get; set; } = new List<SiparisDetay>();
    }

    // Sipariþ durumlarý
    public enum SiparisDurumu
    {
        Onaylandi = 0,      // Sipariþ alýndý
        Hazirlaniyor = 1,   // Hazýrlanýyor
        Kargoya_Verildi = 2,// Kargoya verildi
        Teslim_Edildi = 3,  // Teslim edildi
        Iptal_Edildi = 4    // Ýptal edildi
    }
}
