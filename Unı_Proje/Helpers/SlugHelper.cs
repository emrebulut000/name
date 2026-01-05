using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Uný_Proje.Helpers
{
    /// <summary>
    /// SEO-friendly URL (slug) oluþturmak için yardýmcý sýnýf
    /// </summary>
    public static class SlugHelper
    {
        /// <summary>
        /// Türkçe karakterli metni SEO-friendly slug'a dönüþtürür
        /// Örnek: "iPhone 11 - Temiz" ? "iphone-11-temiz"
        /// </summary>
        public static string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Küçük harfe çevir
            text = text.ToLowerInvariant();

            // Türkçe karakterleri dönüþtür
            text = TurkceKarakterDonustur(text);

            // Özel karakterleri ve boþluklarý tire ile deðiþtir
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

            // Birden fazla boþluðu tek tire yap
            text = Regex.Replace(text, @"\s+", "-");

            // Birden fazla tireyi tek tire yap
            text = Regex.Replace(text, @"-+", "-");

            // Baþtaki ve sondaki tireleri temizle
            text = text.Trim('-');

            // Maksimum 100 karakter
            if (text.Length > 100)
                text = text.Substring(0, 100).Trim('-');

            return text;
        }

        /// <summary>
        /// Benzersiz slug oluþturur (ID ekleyerek)
        /// Örnek: "iphone-11-temiz-123"
        /// </summary>
        public static string GenerateUniqueSlug(string text, int id)
        {
            var baseSlug = GenerateSlug(text);
            return $"{baseSlug}-{id}";
        }

        /// <summary>
        /// Türkçe karakterleri Ýngilizce karþýlýklarýna dönüþtürür
        /// </summary>
        private static string TurkceKarakterDonustur(string text)
        {
            text = text.Replace("ç", "c");
            text = text.Replace("ð", "g");
            text = text.Replace("ý", "i");
            text = text.Replace("ö", "o");
            text = text.Replace("þ", "s");
            text = text.Replace("ü", "u");
            text = text.Replace("Ý", "i");
            
            return text;
        }

        /// <summary>
        /// Slug'dan ID'yi çýkarýr
        /// Örnek: "iphone-11-temiz-123" ? 123
        /// </summary>
        public static int? ExtractIdFromSlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return null;

            // Son tire iþaretinden sonraki sayýyý al
            var parts = slug.Split('-');
            if (parts.Length > 0)
            {
                var lastPart = parts[^1]; // Son eleman
                if (int.TryParse(lastPart, out int id))
                {
                    return id;
                }
            }

            return null;
        }
    }
}
