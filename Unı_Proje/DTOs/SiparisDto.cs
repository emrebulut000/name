namespace Uný_Proje.DTOs
{
    // Sipariþlerim sayfasýnda gösterilecek özet bilgi
    public class SiparisListeDto
    {
        public int Id { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public decimal ToplamTutar { get; set; }
        public string Durum { get; set; } = string.Empty;
        public int UrunSayisi { get; set; }
    }

    // Sipariþ detay sayfasý için
    public class SiparisDetayDto
    {
        public int Id { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public decimal ToplamTutar { get; set; }
        public string Durum { get; set; } = string.Empty;
        public string? TeslimatAdresi { get; set; }
        public string? TeslimatTelefonu { get; set; }
        public List<SiparisUrunDto> Urunler { get; set; } = new();
    }

    // Sipariþteki ürün bilgileri
    public class SiparisUrunDto
    {
        public int Id { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public decimal BirimFiyat { get; set; }
        public int Adet { get; set; }
        public decimal AraToplam { get; set; }
        public string? ResimUrl { get; set; }
    }

    // Sipariþ oluþturma için
    public class SiparisOlusturDto
    {
        public string? TeslimatAdresi { get; set; }
        public string? TeslimatTelefonu { get; set; }
    }
}
