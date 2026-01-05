using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Uný_Proje.Web.Client.Services
{
    public class BildirimServis
    {
        private readonly HttpClient _http;
        private readonly string _apiUrl;

        public BildirimServis(HttpClient http, ApiConfigService apiConfig)
        {
            _http = http;
            _apiUrl = apiConfig.GetWebApiUrl("/api/bildirimler");
        }

        // 1. Tüm Bildirimleri Getir
        public async Task<(List<BildirimDto>? bildirimler, string? hata)> BildirimleriGetir(string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var bildirimler = await _http.GetFromJsonAsync<List<BildirimDto>>(_apiUrl);
                return (bildirimler, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        // 2. Okunmamýþ Bildirim Sayýsý
        public async Task<(int sayý, string? hata)> OkunmamisSayisi(string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var sayi = await _http.GetFromJsonAsync<int>($"{_apiUrl}/okunmamis-sayisi");
                return (sayi, null);
            }
            catch (Exception ex)
            {
                return (0, ex.Message);
            }
        }

        // 3. Bildirimi Okundu Ýþaretle
        public async Task<bool> OkunduIsaretle(int bildirimId, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.PutAsync($"{_apiUrl}/{bildirimId}/okundu", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // 4. Tüm Bildirimleri Okundu Ýþaretle
        public async Task<bool> HepsiniOkunduIsaretle(string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.PutAsync($"{_apiUrl}/hepsini-okundu", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // 5. Bildirimi Sil
        public async Task<bool> BildirimSil(int bildirimId, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.DeleteAsync($"{_apiUrl}/{bildirimId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // 6. Tüm Bildirimleri Sil
        public async Task<bool> HepsiniSil(string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.DeleteAsync($"{_apiUrl}/hepsini-sil");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    // DTO Sýnýflarý
    public class BildirimDto
    {
        public int Id { get; set; }
        public string Tip { get; set; } = "";
        public string Baslik { get; set; } = "";
        public string Mesaj { get; set; } = "";
        public int? IliskiliId { get; set; }
        public bool Okundu { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
    }
}
