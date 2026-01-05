using System.ComponentModel.DataAnnotations;

namespace Unı_Proje.DTOs
{
    public class KullaniciProfilDto
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ad Soyad 2-100 karakter arası olmalıdır")]
        public string AdSoyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        public string? ProfilResmiUrl { get; set; }

        public string? Telefon { get; set; }
        
        public string? Bio { get; set; }
        
        public DateTime? KayitTarihi { get; set; }

        // Şifre değiştirme alanları
        [MinLength(6, ErrorMessage = "Mevcut şifre en az 6 karakter olmalıdır")]
        public string? MevcutSifre { get; set; }

        [MinLength(6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", 
            ErrorMessage = "Şifre en az 1 büyük harf, 1 küçük harf ve 1 rakam içermelidir")]
        public string? YeniSifre { get; set; }
    }
}