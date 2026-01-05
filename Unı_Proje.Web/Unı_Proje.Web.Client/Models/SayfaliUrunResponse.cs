namespace Uný_Proje.Web.Client.Models
{
    public class SayfaliUrunResponse
    {
        public List<UrunViewModel> Urunler { get; set; } = new();
        public SayfaBilgisi SayfaBilgisi { get; set; } = new();
    }

    public class SayfaBilgisi
    {
        public int MevcutSayfa { get; set; }
        public int SayfaBoyutu { get; set; }
        public int ToplamSayfa { get; set; }
        public int ToplamUrun { get; set; }
        public bool OncekiSayfa { get; set; }
        public bool SonrakiSayfa { get; set; }
    }
}
