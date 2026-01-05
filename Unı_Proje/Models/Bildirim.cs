namespace Uný_Proje.Models
{
    public class Bildirim
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public string Tip { get; set; } = string.Empty; // siparis, degerlendirme, mesaj, satis
        public string Baslik { get; set; } = string.Empty;
        public string Mesaj { get; set; } = string.Empty;
        public int? IliskiliId { get; set; } // Sipariþ ID, Mesaj ID vb.
        public bool Okundu { get; set; } = false;
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        // Navigation Property
        public Kullanici? Kullanici { get; set; }
    }

    // Bildirim Tipleri
    public static class BildirimTipleri
    {
        public const string Siparis = "siparis";
        public const string Degerlendirme = "degerlendirme";
        public const string Mesaj = "mesaj";
        public const string Satis = "satis";
    }
}
