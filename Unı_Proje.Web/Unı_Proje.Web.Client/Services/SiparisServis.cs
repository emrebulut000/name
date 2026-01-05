using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Uný_Proje.Web.Client.Services
{
    public class SiparisServis
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7000/api/siparisler";

        public SiparisServis(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 1. Tüm sipariþleri getir
        public async Task<(List<SiparisListeDto> siparisler, string? hata)> SiparisleriGetir(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync(_apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var siparisler = await response.Content.ReadFromJsonAsync<List<SiparisListeDto>>();
                    return (siparisler ?? new List<SiparisListeDto>(), null);
                }

                var hata = await response.Content.ReadAsStringAsync();
                return (new List<SiparisListeDto>(), hata);
            }
            catch (Exception ex)
            {
                return (new List<SiparisListeDto>(), ex.Message);
            }
        }

        // 2. Sipariþ detayýný getir
        public async Task<(SiparisDetayDto? siparis, string? hata)> SiparisDetayGetir(int siparisId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"{_apiUrl}/{siparisId}");

                if (response.IsSuccessStatusCode)
                {
                    var siparis = await response.Content.ReadFromJsonAsync<SiparisDetayDto>();
                    return (siparis, null);
                }

                var hata = await response.Content.ReadAsStringAsync();
                return (null, hata);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        // 3. Sipariþ iptal et
        public async Task<(bool basarili, string mesaj)> SiparisIptal(int siparisId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PutAsync($"{_apiUrl}/{siparisId}/iptal", null);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Sipariþ iptal edildi.");
                }

                var hata = await response.Content.ReadAsStringAsync();
                return (false, hata);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }

    // DTO sýnýflarý
    public class SiparisListeDto
    {
        public int Id { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public decimal ToplamTutar { get; set; }
        public string Durum { get; set; } = string.Empty;
        public int UrunSayisi { get; set; }
    }

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

    public class SiparisUrunDto
    {
        public int Id { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public decimal BirimFiyat { get; set; }
        public int Adet { get; set; }
        public decimal AraToplam { get; set; }
        public string? ResimUrl { get; set; }
    }
}
