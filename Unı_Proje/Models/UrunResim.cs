using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uný_Proje.Models
{
    [Table("urun_resimleri")]
    public class UrunResim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UrunId { get; set; }

        [Required]
        [MaxLength(500)]
        public string ResimUrl { get; set; } = string.Empty;

        // Sýralama için (1, 2, 3, ...)
        public int Sira { get; set; } = 0;

        // Ana resim mi?
        public bool AnaResimMi { get; set; } = false;

        public DateTime EklemeTarihi { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("UrunId")]
        public virtual Urun? Urun { get; set; }
    }
}
