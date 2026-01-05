using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Uný_Proje.Web.Client.Services
{
    public class AdminServis
    {
        private readonly HttpClient _http;
        private readonly ApiConfigService _apiConfig;

        public AdminServis(HttpClient http, ApiConfigService apiConfig)
        {
            _http = http;
            _apiConfig = apiConfig;
        }

        public async Task<(List<KullaniciItem> Kullanicilar, string? Hata)> KullanicilarGetir(string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.GetAsync(_apiConfig.GetWebApiUrl("/api/admin/kullanicilar"));
                
                if (response.IsSuccessStatusCode)
                {
                    var kullanicilar = await response.Content.ReadFromJsonAsync<List<KullaniciItem>>();
                    return (kullanicilar ?? new List<KullaniciItem>(), null);
                }
                
                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return (new List<KullaniciItem>(), "Bu iþlem için admin yetkisi gereklidir.");
                }
                
                return (new List<KullaniciItem>(), "Kullanýcýlar yüklenemedi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KullanicilarGetir - Error: {ex}");
                return (new List<KullaniciItem>(), "Bir hata oluþtu.");
            }
        }

        public async Task<(IstatistiklerDto? Istatistikler, string? Hata)> IstatistikleriGetir(string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.GetAsync(_apiConfig.GetWebApiUrl("/api/admin/istatistikler"));
                
                if (response.IsSuccessStatusCode)
                {
                    var istatistikler = await response.Content.ReadFromJsonAsync<IstatistiklerDto>();
                    return (istatistikler, null);
                }
                
                return (null, "Ýstatistikler yüklenemedi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IstatistikleriGetir - Error: {ex}");
                return (null, "Bir hata oluþtu.");
            }
        }

        public async Task<(KullaniciDetayDto? Detay, string? Hata)> KullaniciDetayGetir(int id, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.GetAsync(_apiConfig.GetWebApiUrl($"/api/admin/kullanici/{id}"));
                
                if (response.IsSuccessStatusCode)
                {
                    var detay = await response.Content.ReadFromJsonAsync<KullaniciDetayDto>();
                    return (detay, null);
                }
                
                return (null, "Kullanýcý detayý yüklenemedi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KullaniciDetayGetir - Error: {ex}");
                return (null, "Bir hata oluþtu.");
            }
        }

        public async Task<AdminSonuc> KullaniciSil(int id, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.DeleteAsync(_apiConfig.GetWebApiUrl($"/api/admin/kullanici/{id}"));
                
                if (response.IsSuccessStatusCode)
                {
                    return AdminSonuc.Basarili("Kullanýcý baþarýyla silindi.");
                }
                
                var hata = await response.Content.ReadAsStringAsync();
                return AdminSonuc.Hata(hata);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KullaniciSil - Error: {ex}");
                return AdminSonuc.Hata("Bir hata oluþtu.");
            }
        }

        public async Task<AdminSonuc> AdminYetkisiDegistir(int id, bool isAdmin, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var dto = new { IsAdmin = isAdmin };
                var response = await _http.PutAsJsonAsync(_apiConfig.GetWebApiUrl($"/api/admin/kullanici/{id}/admin"), dto);
                
                if (response.IsSuccessStatusCode)
                {
                    return AdminSonuc.Basarili($"Admin yetkisi {(isAdmin ? "verildi" : "kaldýrýldý")}.");
                }
                
                return AdminSonuc.Hata("Ýþlem baþarýsýz.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AdminYetkisiDegistir - Error: {ex}");
                return AdminSonuc.Hata("Bir hata oluþtu.");
            }
        }

        public async Task<bool> AdminMi(string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _http.GetAsync(_apiConfig.GetWebApiUrl("/api/admin/admin-mi"));
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<bool>();
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AdminMi - Error: {ex}");
                return false;
            }
        }
    }

    public class AdminSonuc
    {
        public bool BasariliMi { get; set; }
        public string Mesaj { get; set; } = "";

        public static AdminSonuc Basarili(string mesaj) => new() { BasariliMi = true, Mesaj = mesaj };
        public static AdminSonuc Hata(string mesaj) => new() { BasariliMi = false, Mesaj = mesaj };
    }

    public class KullaniciItem
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; } = "";
        public string Email { get; set; } = "";
        public string? ProfilResmiUrl { get; set; }
        public string? Telefon { get; set; }
        public string? Bio { get; set; }
        public DateTime KayitTarihi { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class IstatistiklerDto
    {
        public int ToplamKullanici { get; set; }
        public int ToplamUrun { get; set; }
        public int ToplamMesaj { get; set; }
        public int ToplamFavori { get; set; }
        public int YeniKullanicilar { get; set; }
        public int BugunEklenenUrunler { get; set; }
    }

    public class KullaniciDetayDto
    {
        public KullaniciItem Kullanici { get; set; } = new();
        public KullaniciIstatistiklerDto Istatistikler { get; set; } = new();
    }

    public class KullaniciIstatistiklerDto
    {
        public int UrunSayisi { get; set; }
        public int MesajSayisi { get; set; }
        public int FavoriSayisi { get; set; }
    }
}
