using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uný_Proje.Models
{
    /// <summary>
    /// ChatBot geçmiþi için veritabaný modeli (opsiyonel)
    /// </summary>
    public class ChatMesaj
    {
        [Key]
        public int Id { get; set; }

        // Kullanýcý ID (anonim kullanýcýlar için null olabilir)
        public int? KullaniciId { get; set; }

        // Session ID (her konuþma oturumu için benzersiz)
        [Required]
        [StringLength(100)]
        public string SessionId { get; set; } = string.Empty;

        // Mesaj metni
        [Required]
        [StringLength(1000)]
        public string Mesaj { get; set; } = string.Empty;

        // Mesajý kim gönderdi? (user/bot)
        [Required]
        [StringLength(10)]
        public string Gonderen { get; set; } = string.Empty; // "user" veya "bot"

        // Mesaj zamaný
        public DateTime GondermeTarihi { get; set; } = DateTime.Now;

        // Navigation Property
        [ForeignKey("KullaniciId")]
        public Kullanici? Kullanici { get; set; }
    }
}
