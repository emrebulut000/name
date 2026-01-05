using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uný_Proje.Models
{
    public class SaticiDegerlendirme
    {
        [Key]
        public int Id { get; set; }

        // Hangi satýcý için?
        public int SaticiId { get; set; }
        [ForeignKey("SaticiId")]
        public Kullanici Satici { get; set; } = null!;

        // Kim deðerlendirdi?
        public int DegerlendirenId { get; set; }
        [ForeignKey("DegerlendirenId")]
        public Kullanici Degerlendiren { get; set; } = null!;

        // Hangi sipariþ için? (Bir sipariþ baþýna 1 deðerlendirme)
        public int SiparisId { get; set; }
        [ForeignKey("SiparisId")]
        public Siparis Siparis { get; set; } = null!;

        // Puan (1-5 yýldýz)
        [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasýnda olmalýdýr.")]
        public int Puan { get; set; }

        // Yorum (opsiyonel)
        [MaxLength(500)]
        public string? Yorum { get; set; }

        // Ne zaman yapýldý?
        public DateTime DegerlendirmeTarihi { get; set; } = DateTime.Now;
    }
}
