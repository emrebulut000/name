namespace Uný_Proje.DTOs
{
    // Ürün resmi ekleme DTO'su
    public class UrunResimEkleDto
    {
        public int UrunId { get; set; }
        public string ResimUrl { get; set; } = string.Empty;
        public int Sira { get; set; } = 0;
        public bool AnaResimMi { get; set; } = false;
    }

    // Ürün resmi görüntüleme DTO'su
    public class UrunResimDto
    {
        public int Id { get; set; }
        public int UrunId { get; set; }
        public string ResimUrl { get; set; } = string.Empty;
        public int Sira { get; set; }
        public bool AnaResimMi { get; set; }
    }

    // Çoklu resim yükleme için
    public class CokluResimYukleDto
    {
        public int UrunId { get; set; }
        public List<string> ResimUrlListesi { get; set; } = new();
        public int? AnaResimIndex { get; set; } // Hangi resim ana resim olacak (0-based)
    }
}
