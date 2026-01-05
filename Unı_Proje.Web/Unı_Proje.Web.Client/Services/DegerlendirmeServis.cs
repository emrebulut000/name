using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Uný_Proje.Web.Client.Services
{
    public class DegerlendirmeServis
    {
        private readonly HttpClient _http;
        private readonly string _apiUrl;

        public DegerlendirmeServis(HttpClient http, ApiConfigService apiConfig)
        {
            _http = http;
            _apiUrl = apiConfig.GetWebApiUrl("/api/degerlendirme");
        }

        // 1. Deðerlendirme Yap
        public async Task<(bool basarili, string mesaj)> DegerlendirmeYap(DegerlendirmeOlusturDto dto, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.PostAsJsonAsync(_apiUrl, dto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<MessageResponse>();
                    return (true, result?.Message ?? "Deðerlendirme kaydedildi.");
                }

                var error = await response.Content.ReadAsStringAsync();
                return (false, error);
            }
            catch (Exception ex)
            {
                return (false, $"Hata: {ex.Message}");
            }
        }

        // 2. Satýcý Deðerlendirmelerini Getir
        public async Task<(List<DegerlendirmeDto>? liste, string? hata)> SaticiDegerlendirmeleri(int saticiId)
        {
            try
            {
                var degerlendirmeler = await _http.GetFromJsonAsync<List<DegerlendirmeDto>>($"{_apiUrl}/satici/{saticiId}");
                return (degerlendirmeler, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        // 3. Satýcý Puan Özeti
        public async Task<(SaticiPuanOzetiDto? ozet, string? hata)> SaticiPuanOzeti(int saticiId)
        {
            try
            {
                var ozet = await _http.GetFromJsonAsync<SaticiPuanOzetiDto>($"{_apiUrl}/satici/{saticiId}/ozet");
                return (ozet, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        // 4. Deðerlendirme Yapýldý Mý?
        public async Task<(bool yapildi, string? hata)> DegerlendirmeYapildiMi(int siparisId, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var yapildi = await _http.GetFromJsonAsync<bool>($"{_apiUrl}/kontrol/{siparisId}");
                return (yapildi, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // 5. Benim Deðerlendirmelerim
        public async Task<(List<DegerlendirmeDto>? liste, string? hata)> BenimDegerlendirmelerim(string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var degerlendirmeler = await _http.GetFromJsonAsync<List<DegerlendirmeDto>>($"{_apiUrl}/benimlerim");
                return (degerlendirmeler, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }
    }

    // DTO Sýnýflarý
    public class DegerlendirmeOlusturDto
    {
        public int SiparisId { get; set; }
        public int Puan { get; set; }
        public string? Yorum { get; set; }
    }

    public class DegerlendirmeDto
    {
        public int Id { get; set; }
        public int SaticiId { get; set; }
        public string SaticiAdi { get; set; } = string.Empty;
        public int DegerlendirenId { get; set; }
        public string DegerlendirenAdi { get; set; } = string.Empty;
        public int Puan { get; set; }
        public string? Yorum { get; set; }
        public DateTime DegerlendirmeTarihi { get; set; }
    }

    public class SaticiPuanOzetiDto
    {
        public int SaticiId { get; set; }
        public string SaticiAdi { get; set; } = string.Empty;
        public decimal? OrtalamaPuan { get; set; }
        public int ToplamDegerlendirme { get; set; }
        public int Puan5Yildiz { get; set; }
        public int Puan4Yildiz { get; set; }
        public int Puan3Yildiz { get; set; }
        public int Puan2Yildiz { get; set; }
        public int Puan1Yildiz { get; set; }
    }

    public class MessageResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}
