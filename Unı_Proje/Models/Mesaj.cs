using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uný_Proje.Models
{
    public class Mesaj
    {
        [Key]
        public int Id { get; set; }

        // Gönderen kullanýcý
        public int GonderenId { get; set; }
        [ForeignKey("GonderenId")]
        public Kullanici Gonderen { get; set; } = null!;

        // Alýcý kullanýcý
        public int AliciId { get; set; }
        [ForeignKey("AliciId")]
        public Kullanici Alici { get; set; } = null!;

        // Hangi ürün hakkýnda konuþuluyor (opsiyonel)
        public int? UrunId { get; set; }
        [ForeignKey("UrunId")]
        public Urun? Urun { get; set; }

        // Mesaj içeriði
        [Required]
        public string Icerik { get; set; } = string.Empty;

        // Mesaj gönderim tarihi
        public DateTime GonderimTarihi { get; set; } = DateTime.Now;

        // Mesaj okundu mu?
        public bool Okundu { get; set; } = false;

        // Okunma tarihi
        public DateTime? OkunmaTarihi { get; set; }
    }
}
