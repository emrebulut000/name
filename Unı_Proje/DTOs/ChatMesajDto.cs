using System.ComponentModel.DataAnnotations;

namespace Uný_Proje.DTOs
{
    /// <summary>
    /// ChatBot mesaj gönderme DTO'su
    /// </summary>
    public class ChatMesajDto
    {
        [Required(ErrorMessage = "Mesaj boþ olamaz")]
        [StringLength(1000, ErrorMessage = "Mesaj en fazla 1000 karakter olabilir")]
        public string Mesaj { get; set; } = string.Empty;

        // Session ID (frontend tarafýndan gönderilir)
        public string? SessionId { get; set; }
    }

    /// <summary>
    /// ChatBot cevap DTO'su
    /// </summary>
    public class ChatCevapDto
    {
        public string Cevap { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public DateTime ZamanDamgasi { get; set; } = DateTime.Now;
        
        // Önerilen hýzlý cevaplar (opsiyonel)
        public List<string>? HizliCevaplar { get; set; }
    }
}
