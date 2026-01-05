using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Uný_Proje.Web.Client.Services
{
    public class FavoriServis
    {
        private readonly HttpClient _http;
        private readonly ApiConfigService _apiConfig;

        public FavoriServis(HttpClient http, ApiConfigService apiConfig)
        {
            _http = http;
            _apiConfig = apiConfig;
        }

        public event Action? FavorilerGuncellendi;

        public async Task<FavoriSonuc> FavorilereEkle(int urunId, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.PostAsync(_apiConfig.GetWebApiUrl($"/api/favoriler/ekle/{urunId}"), null);
                
                if (response.IsSuccessStatusCode)
                {
                    FavorilerGuncellendi?.Invoke();
                    return FavoriSonuc.Basarili("Ürün favorilere baþarýyla eklendi!");
                }
                
                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => FavoriSonuc.Hata("Oturumunuz sona erdi. Lütfen tekrar giriþ yapýn."),
                    HttpStatusCode.BadRequest => FavoriSonuc.Hata(await response.Content.ReadAsStringAsync()),
                    HttpStatusCode.NotFound => FavoriSonuc.Hata("Ürün bulunamadý."),
                    _ => FavoriSonuc.Hata($"Beklenmeyen hata oluþtu. Kod: {response.StatusCode}")
                };
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"FavorilereEkle - Network Error: {ex.Message}");
                return FavoriSonuc.Hata("Baðlantý hatasý. Ýnternet baðlantýnýzý kontrol edin.");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"FavorilereEkle - Timeout: {ex.Message}");
                return FavoriSonuc.Hata("Ýstek zaman aþýmýna uðradý. Lütfen tekrar deneyin.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FavorilereEkle - Unexpected Error: {ex}");
                return FavoriSonuc.Hata("Bilinmeyen bir hata oluþtu.");
            }
        }

        public async Task<(List<FavoriItem> Items, string? Hata)> FavorileriGetir(string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.GetAsync(_apiConfig.GetWebApiUrl("/api/favoriler"));
                
                if (response.IsSuccessStatusCode)
                {
                    var favoriler = await response.Content.ReadFromJsonAsync<List<FavoriItem>>();
                    return (favoriler ?? new List<FavoriItem>(), null);
                }
                
                var hataMesaji = response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Oturumunuz sona erdi. Lütfen tekrar giriþ yapýn.",
                    _ => $"Favoriler yüklenemedi. Hata kodu: {response.StatusCode}"
                };
                
                Console.WriteLine($"FavorileriGetir - HTTP Error: {response.StatusCode}");
                return (new List<FavoriItem>(), hataMesaji);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"FavorileriGetir - Network Error: {ex.Message}");
                return (new List<FavoriItem>(), "Baðlantý hatasý oluþtu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FavorileriGetir - Error: {ex}");
                return (new List<FavoriItem>(), "Favoriler yüklenirken bir hata oluþtu.");
            }
        }

        public async Task<FavoriSonuc> FavorilerdenCikar(int urunId, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.DeleteAsync(_apiConfig.GetWebApiUrl($"/api/favoriler/sil/{urunId}"));
                
                if (response.IsSuccessStatusCode)
                {
                    FavorilerGuncellendi?.Invoke();
                    return FavoriSonuc.Basarili("Ürün favorilerden çýkarýldý.");
                }
                
                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => FavoriSonuc.Hata("Oturumunuz sona erdi."),
                    HttpStatusCode.NotFound => FavoriSonuc.Hata("Ürün favorilerde bulunamadý."),
                    _ => FavoriSonuc.Hata("Ürün çýkarýlamadý.")
                };
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"FavorilerdenCikar - Network Error: {ex.Message}");
                return FavoriSonuc.Hata("Baðlantý hatasý oluþtu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FavorilerdenCikar - Error: {ex}");
                return FavoriSonuc.Hata("Bir hata oluþtu.");
            }
        }

        public async Task<bool> FavoriMi(int urunId, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.GetAsync(_apiConfig.GetWebApiUrl($"/api/favoriler/kontrol/{urunId}"));
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<bool>();
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FavoriMi - Error: {ex}");
                return false;
            }
        }
    }

    // Sonuç sýnýfý
    public class FavoriSonuc
    {
        public bool BasariliMi { get; set; }
        public string Mesaj { get; set; } = "";

        public static FavoriSonuc Basarili(string mesaj) => new() { BasariliMi = true, Mesaj = mesaj };
        public static FavoriSonuc Hata(string mesaj) => new() { BasariliMi = false, Mesaj = mesaj };
    }

    public class FavoriItem
    {
        public int Id { get; set; }
        public int UrunId { get; set; }
        public string Ad { get; set; } = "";
        public decimal Fiyat { get; set; }
        public string? ResimUrl { get; set; }
        public KategoriInfo? Kategori { get; set; }
        public DateTime EklemeTarihi { get; set; }
    }

    public class KategoriInfo
    {
        public int Id { get; set; }
        public string Ad { get; set; } = "";
    }
}
