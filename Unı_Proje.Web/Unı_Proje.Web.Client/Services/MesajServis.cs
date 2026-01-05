using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Uný_Proje.Web.Client.Services
{
    public class MesajServis
    {
        private readonly HttpClient _http;
        private readonly ApiConfigService _apiConfig;

        public MesajServis(HttpClient http, ApiConfigService apiConfig)
        {
            _http = http;
            _apiConfig = apiConfig;
        }

        public event Action? MesajlarGuncellendi;

        public async Task<(List<KonusmaItem> Konusmalar, string? Hata)> KonusmalarGetir(string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.GetAsync(_apiConfig.GetWebApiUrl("/api/mesajlar/konusmalar"));
                
                if (response.IsSuccessStatusCode)
                {
                    var konusmalar = await response.Content.ReadFromJsonAsync<List<KonusmaItem>>();
                    return (konusmalar ?? new List<KonusmaItem>(), null);
                }
                
                return (new List<KonusmaItem>(), "Konuþmalar yüklenemedi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KonusmalarGetir - Error: {ex}");
                return (new List<KonusmaItem>(), "Bir hata oluþtu.");
            }
        }

        public async Task<(List<MesajItem> Mesajlar, string? Hata)> KonusmaGetir(int karsiTarafId, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.GetAsync(_apiConfig.GetWebApiUrl($"/api/mesajlar/konusma/{karsiTarafId}"));
                
                if (response.IsSuccessStatusCode)
                {
                    var mesajlar = await response.Content.ReadFromJsonAsync<List<MesajItem>>();
                    return (mesajlar ?? new List<MesajItem>(), null);
                }
                
                return (new List<MesajItem>(), "Mesajlar yüklenemedi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KonusmaGetir - Error: {ex}");
                return (new List<MesajItem>(), "Bir hata oluþtu.");
            }
        }

        public async Task<MesajSonuc> MesajGonder(MesajGonderDto dto, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.PostAsJsonAsync(_apiConfig.GetWebApiUrl("/api/mesajlar/gonder"), dto);
                
                if (response.IsSuccessStatusCode)
                {
                    MesajlarGuncellendi?.Invoke();
                    return MesajSonuc.Basarili("Mesaj gönderildi.");
                }
                
                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => MesajSonuc.Hata("Oturumunuz sona erdi."),
                    HttpStatusCode.BadRequest => MesajSonuc.Hata(await response.Content.ReadAsStringAsync()),
                    _ => MesajSonuc.Hata("Mesaj gönderilemedi.")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MesajGonder - Error: {ex}");
                return MesajSonuc.Hata("Bir hata oluþtu.");
            }
        }

        public async Task<int> OkunmamisSayisi(string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.GetAsync(_apiConfig.GetWebApiUrl("/api/mesajlar/okunmamis-sayisi"));
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<int>();
                }
                
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OkunmamisSayisi - Error: {ex}");
                return 0;
            }
        }
    }

    public class MesajSonuc
    {
        public bool BasariliMi { get; set; }
        public string Mesaj { get; set; } = "";

        public static MesajSonuc Basarili(string mesaj) => new() { BasariliMi = true, Mesaj = mesaj };
        public static MesajSonuc Hata(string mesaj) => new() { BasariliMi = false, Mesaj = mesaj };
    }

    public class KonusmaItem
    {
        public int KarsiTarafId { get; set; }
        public string KarsiTarafAdi { get; set; } = "";
        public string? KarsiTarafResim { get; set; }
        public string SonMesaj { get; set; } = "";
        public DateTime SonMesajTarihi { get; set; }
        public int? UrunId { get; set; }
        public string? UrunAd { get; set; }
        public int OkunmamisSayisi { get; set; }
        public bool BenGonderdim { get; set; }
    }

    public class MesajItem
    {
        public int Id { get; set; }
        public int GonderenId { get; set; }
        public string GonderenAdi { get; set; } = "";
        public int AliciId { get; set; }
        public string AliciAdi { get; set; } = "";
        public string Icerik { get; set; } = "";
        public DateTime GonderimTarihi { get; set; }
        public bool Okundu { get; set; }
        public int? UrunId { get; set; }
        public string? UrunAd { get; set; }
        public string? UrunResim { get; set; }
    }

    public class MesajGonderDto
    {
        public int AliciId { get; set; }
        public int? UrunId { get; set; }
        public string Icerik { get; set; } = string.Empty;
    }
}
