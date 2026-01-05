using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Unı_Proje.Web.Client.Services
{
    public class SepetServis
    {
        private readonly HttpClient _http;
        private readonly ApiConfigService _apiConfig;

        public SepetServis(HttpClient http, ApiConfigService apiConfig)
        {
            _http = http;
            _apiConfig = apiConfig;
        }

        public event Action? SepetGuncellendi;

        public async Task<SepetSonuc> SepeteEkle(int urunId, string token, int? teklifId = null)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var url = _apiConfig.GetWebApiUrl($"/api/sepet/ekle/{urunId}");
                if (teklifId.HasValue)
                {
                    url += $"?teklifId={teklifId.Value}";
                }
                
                var response = await _http.PostAsync(url, null);
                
                if (response.IsSuccessStatusCode)
                {
                    SepetGuncellendi?.Invoke();
                    return SepetSonuc.Basarili("Ürün sepete başarıyla eklendi!");
                }
                
                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => SepetSonuc.Hata("Oturumunuz sona erdi. Lütfen tekrar giriş yapın."),
                    HttpStatusCode.BadRequest => SepetSonuc.Hata(await response.Content.ReadAsStringAsync()),
                    HttpStatusCode.NotFound => SepetSonuc.Hata("Ürün bulunamadı."),
                    _ => SepetSonuc.Hata($"Beklenmeyen hata oluştu. Kod: {response.StatusCode}")
                };
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"SepeteEkle - Network Error: {ex.Message}");
                return SepetSonuc.Hata("Bağlantı hatası. İnternet bağlantınızı kontrol edin.");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"SepeteEkle - Timeout: {ex.Message}");
                return SepetSonuc.Hata("İstek zaman aşımına uğradı. Lütfen tekrar deneyin.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SepeteEkle - Unexpected Error: {ex}");
                return SepetSonuc.Hata("Bilinmeyen bir hata oluştu.");
            }
        }

        public async Task<(List<SepetItem> Items, string? Hata)> SepetGetir(string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.GetAsync(_apiConfig.GetWebApiUrl("/api/sepet"));
                
                if (response.IsSuccessStatusCode)
                {
                    var sepet = await response.Content.ReadFromJsonAsync<List<SepetItem>>();
                    return (sepet ?? new List<SepetItem>(), null);
                }
                
                var hataMesaji = response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Oturumunuz sona erdi. Lütfen tekrar giriş yapın.",
                    _ => $"Sepet yüklenemedi. Hata kodu: {response.StatusCode}"
                };
                
                Console.WriteLine($"SepetGetir - HTTP Error: {response.StatusCode}");
                return (new List<SepetItem>(), hataMesaji);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"SepetGetir - Network Error: {ex.Message}");
                return (new List<SepetItem>(), "Bağlantı hatası oluştu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SepetGetir - Error: {ex}");
                return (new List<SepetItem>(), "Sepet yüklenirken bir hata oluştu.");
            }
        }

        public async Task<SepetSonuc> SepettenCikar(int id, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.DeleteAsync(_apiConfig.GetWebApiUrl($"/api/sepet/sil/{id}"));
                
                if (response.IsSuccessStatusCode)
                {
                    SepetGuncellendi?.Invoke();
                    return SepetSonuc.Basarili("Ürün sepetten çıkarıldı.");
                }
                
                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => SepetSonuc.Hata("Oturumunuz sona erdi."),
                    HttpStatusCode.NotFound => SepetSonuc.Hata("Ürün sepette bulunamadı."),
                    _ => SepetSonuc.Hata("Ürün çıkarılamadı.")
                };
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"SepettenCikar - Network Error: {ex.Message}");
                return SepetSonuc.Hata("Bağlantı hatası oluştu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SepettenCikar - Error: {ex}");
                return SepetSonuc.Hata("Bir hata oluştu.");
            }
        }

        public async Task<SepetSonuc> SatinAl(string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.PostAsync(_apiConfig.GetWebApiUrl("/api/sepet/satinal"), null);
                
                if (response.IsSuccessStatusCode)
                {
                    SepetGuncellendi?.Invoke();
                    return SepetSonuc.Basarili("Siparişiniz başarıyla alındı!");
                }
                
                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => SepetSonuc.Hata("Oturumunuz sona erdi."),
                    HttpStatusCode.BadRequest => SepetSonuc.Hata(await response.Content.ReadAsStringAsync()),
                    _ => SepetSonuc.Hata("Sipariş alınamadı.")
                };
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"SatinAl - Network Error: {ex.Message}");
                return SepetSonuc.Hata("Bağlantı hatası oluştu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SatinAl - Error: {ex}");
                return SepetSonuc.Hata("Bir hata oluştu.");
            }
        }
    }

    // Sonuç sınıfı
    public class SepetSonuc
    {
        public bool BasariliMi { get; set; }
        public string Mesaj { get; set; } = "";

        public static SepetSonuc Basarili(string mesaj) => new() { BasariliMi = true, Mesaj = mesaj };
        public static SepetSonuc Hata(string mesaj) => new() { BasariliMi = false, Mesaj = mesaj };
    }

    public class SepetItem
    {
        public int Id { get; set; }
        public int UrunId { get; set; }
        public string UrunAd { get; set; } = "";
        public decimal Fiyat { get; set; }
        public string? ResimUrl { get; set; }
        public string KategoriAd { get; set; } = "";
        // 📦 Stok bilgisi
        public int StokMiktari { get; set; } = 0;
        public bool StokVarMi { get; set; } = true;
    }
}
