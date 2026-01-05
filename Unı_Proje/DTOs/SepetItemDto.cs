namespace Unı_Proje.DTOs
{
    public class SepetItemDto
    {
        public int Id { get; set; }
        public int UrunId { get; set; }
        public string UrunAd { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public string? ResimUrl { get; set; }
        public string KategoriAd { get; set; } = string.Empty;
        
        // 📦 Stok bilgisi (opsiyonel - sepet görünümü için)
        public int StokMiktari { get; set; } = 0;
        public bool StokVarMi { get; set; } = true;
    }
}