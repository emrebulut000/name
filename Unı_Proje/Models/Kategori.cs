// Models/Kategori.cs
namespace Unı_Proje.Models 
{
    public class Kategori
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;

        // Bir kategorinin birden fazla ürünü olabilir (Navigation property)
        public ICollection<Urun> Urunler { get; set; } = new List<Urun>();
    }
}