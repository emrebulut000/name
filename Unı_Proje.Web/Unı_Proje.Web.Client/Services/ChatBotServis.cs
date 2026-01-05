using System.Net.Http.Json;
using System.Text.Json;

namespace Uný_Proje.Web.Client.Services
{
    /// <summary>
    /// ChatBot ile iletiþim için servis
    /// </summary>
    public class ChatBotServis
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public ChatBotServis(HttpClient http, ApiConfigService apiConfig)
        {
            _http = http;
            _baseUrl = $"{apiConfig.CurrentApiUrl}/api/chatbot";
            Console.WriteLine($"[CHATBOT SERVIS] Baþlatýldý. Base URL: {_baseUrl}");
        }

        /// <summary>
        /// ChatBot'a mesaj gönder
        /// </summary>
        public async Task<ChatCevapDto?> MesajGonder(string mesaj, string? sessionId = null)
        {
            try
            {
                Console.WriteLine($"[CHATBOT SERVIS] Mesaj gönderiliyor: {mesaj}");

                var dto = new ChatMesajDto
                {
                    Mesaj = mesaj,
                    SessionId = sessionId
                };

                var response = await _http.PostAsJsonAsync($"{_baseUrl}/mesaj", dto);
                
                if (response.IsSuccessStatusCode)
                {
                    var cevap = await response.Content.ReadFromJsonAsync<ChatCevapDto>();
                    Console.WriteLine($"[CHATBOT SERVIS] Cevap alýndý: {cevap?.Cevap}");
                    return cevap;
                }
                else
                {
                    Console.WriteLine($"[CHATBOT SERVIS] Hata: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CHATBOT SERVIS] Ýstisna: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Chat geçmiþini getir
        /// </summary>
        public async Task<List<ChatGezmisDto>?> GecmisGetir(string sessionId)
        {
            try
            {
                var response = await _http.GetAsync($"{_baseUrl}/gecmis?sessionId={sessionId}");
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ChatGezmisDto>>();
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CHATBOT SERVIS] Geçmiþ getirme hatasý: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Chat geçmiþini temizle
        /// </summary>
        public async Task<bool> GecmisTemizle(string sessionId)
        {
            try
            {
                var response = await _http.DeleteAsync($"{_baseUrl}/temizle/{sessionId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CHATBOT SERVIS] Temizleme hatasý: {ex.Message}");
                return false;
            }
        }
    }

    // DTOs
    public class ChatMesajDto
    {
        public string Mesaj { get; set; } = string.Empty;
        public string? SessionId { get; set; }
    }

    public class ChatCevapDto
    {
        public string Cevap { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public DateTime ZamanDamgasi { get; set; }
        public List<string>? HizliCevaplar { get; set; }
    }

    public class ChatGezmisDto
    {
        public string Mesaj { get; set; } = string.Empty;
        public string Gonderen { get; set; } = string.Empty; // "user" veya "bot"
        public DateTime GondermeTarihi { get; set; }
    }
}
