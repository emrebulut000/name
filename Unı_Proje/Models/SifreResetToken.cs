using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uný_Proje.Models
{
    public class SifreResetToken
    {
        [Key]
        public int Id { get; set; }

        public int KullaniciId { get; set; }
        
        [ForeignKey("KullaniciId")]
        public Kullanici Kullanici { get; set; } = null!;

        [Required]
        public string Token { get; set; } = string.Empty;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        public DateTime SonKullanmaTarihi { get; set; }

        public bool Kullanildi { get; set; } = false;
    }
}
