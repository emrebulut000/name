// DTOs/UrunEkleDto.cs
using System.ComponentModel.DataAnnotations;

namespace Unı_Proje.DTOs
{

    public class UrunEkleDto
    {
        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(100, MinimumLength = 3)]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        public string Aciklama { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal Fiyat { get; set; }

        // Kategori ID'si. Bu ürünü eklerken hangi kategoriye ait olduğunu belirteceğiz.
        [Required]
        public int KategoriId { get; set; }

        public string ResimUrl { get; set; } = "";

        // Ürün konumu
        public string? Konum { get; set; } = string.Empty;

        // 📦 Stok Miktarı
        [Required(ErrorMessage = "Stok miktarı zorunludur.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı 0 veya daha büyük olmalıdır.")]
        public int StokMiktari { get; set; } = 0;
    }
}
