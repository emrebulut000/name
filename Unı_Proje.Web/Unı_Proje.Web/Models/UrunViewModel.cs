namespace Unı_Proje.Web.Models 
{
    public class UrunViewModel
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public DateTime EklemeTarihi { get; set; }
        public string KategoriAdi { get; set; } = string.Empty;
        public string SaticiAdi { get; set; } = string.Empty;
    }
}