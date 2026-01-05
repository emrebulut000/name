using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Uný_Proje.Web.Client.Services
{
    public class TeklifServis
    {
        private readonly HttpClient _http;
        private readonly ApiConfigService _apiConfig;

        public TeklifServis(HttpClient http, ApiConfigService apiConfig)
        {
            _http = http;
            _apiConfig = apiConfig;
        }

        // 1. Teklif Gönder
        public async Task<TeklifSonuc> TeklifGonder(int urunId, decimal teklifTutari, string? aciklama, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var dto = new { UrunId = urunId, TeklifTutari = teklifTutari, Aciklama = aciklama };
                var response = await _http.PostAsJsonAsync(_apiConfig.GetWebApiUrl("/api/teklifler"), dto);

                if (response.IsSuccessStatusCode)
                {
                    return TeklifSonuc.Basarili("Teklifiniz baþarýyla gönderildi.");
                }

                var hata = await response.Content.ReadAsStringAsync();
                return TeklifSonuc.Hata(hata);
            }
            catch (Exception ex)
            {
                return TeklifSonuc.Hata(ex.Message);
            }
        }

        // 2. Gönderdiðim Teklifler
        public async Task<(List<TeklifDto>? teklifler, string? hata)> GonderdigimTeklifleriGetir(string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var teklifler = await _http.GetFromJsonAsync<List<TeklifDto>>(_apiConfig.GetWebApiUrl("/api/teklifler/gonderdigim"));
                return (teklifler, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        // 3. Gelen Teklifler
        public async Task<(List<TeklifDto>? teklifler, string? hata)> GelenTeklifleriGetir(string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var teklifler = await _http.GetFromJsonAsync<List<TeklifDto>>(_apiConfig.GetWebApiUrl("/api/teklifler/gelen"));
                return (teklifler, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        // 4. Teklifi Kabul Et
        public async Task<TeklifSonuc> TeklifiKabulEt(int teklifId, string? yanit, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var dto = new { Yanit = yanit };
                var response = await _http.PutAsJsonAsync(_apiConfig.GetWebApiUrl($"/api/teklifler/{teklifId}/kabul"), dto);

                if (response.IsSuccessStatusCode)
                {
                    return TeklifSonuc.Basarili("Teklif kabul edildi.");
                }

                var hata = await response.Content.ReadAsStringAsync();
                return TeklifSonuc.Hata(hata);
            }
            catch (Exception ex)
            {
                return TeklifSonuc.Hata(ex.Message);
            }
        }

        // 5. Teklifi Reddet
        public async Task<TeklifSonuc> TeklifiReddet(int teklifId, string? yanit, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var dto = new { Yanit = yanit };
                var response = await _http.PutAsJsonAsync(_apiConfig.GetWebApiUrl($"/api/teklifler/{teklifId}/reddet"), dto);

                if (response.IsSuccessStatusCode)
                {
                    return TeklifSonuc.Basarili("Teklif reddedildi.");
                }

                var hata = await response.Content.ReadAsStringAsync();
                return TeklifSonuc.Hata(hata);
            }
            catch (Exception ex)
            {
                return TeklifSonuc.Hata(ex.Message);
            }
        }

        // 6. Teklifi Ýptal Et
        public async Task<TeklifSonuc> TeklifiIptalEt(int teklifId, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.PutAsync(_apiConfig.GetWebApiUrl($"/api/teklifler/{teklifId}/iptal"), null);

                if (response.IsSuccessStatusCode)
                {
                    return TeklifSonuc.Basarili("Teklif iptal edildi.");
                }

                var hata = await response.Content.ReadAsStringAsync();
                return TeklifSonuc.Hata(hata);
            }
            catch (Exception ex)
            {
                return TeklifSonuc.Hata(ex.Message);
            }
        }
    }

    // Sonuç Sýnýfý
    public class TeklifSonuc
    {
        public bool BasariliMi { get; set; }
        public string Mesaj { get; set; } = "";

        public static TeklifSonuc Basarili(string mesaj) => new() { BasariliMi = true, Mesaj = mesaj };
        public static TeklifSonuc Hata(string mesaj) => new() { BasariliMi = false, Mesaj = mesaj };
    }

    // DTO
    public class TeklifDto
    {
        public int Id { get; set; }
        public int UrunId { get; set; }
        public string UrunAd { get; set; } = "";
        public string? UrunResim { get; set; }
        public decimal UrunFiyat { get; set; }
        public string? KategoriAd { get; set; }
        public string? SaticiAd { get; set; }
        public string? AliciAd { get; set; }
        public string? AliciEmail { get; set; }
        public decimal TeklifTutari { get; set; }
        public string? Aciklama { get; set; }
        public string Durum { get; set; } = "";
        public string DurumText { get; set; } = "";
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime? YanitTarihi { get; set; }
        public string? SaticiYaniti { get; set; }
    }
}
