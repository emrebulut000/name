using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uný_Proje.Models
{
    /// <summary>
    /// Email doðrulama token'larý için model
    /// </summary>
    [Table("email_dogrulama_tokenleri")]
    public class EmailDogrulamaToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(6)]
        public string Token { get; set; } = string.Empty;

        public DateTime OlusturmaTarihi { get; set; }

        public DateTime SonKullanmaTarihi { get; set; }

        public bool Kullanildi { get; set; }
    }
}
