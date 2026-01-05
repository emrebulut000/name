using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Uný_Proje.Data;
using Uný_Proje.DTOs;
using Uný_Proje.Models;
using Uný_Proje.Services;

namespace Uný_Proje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {
        private readonly ProjeDbContext _context;
        private readonly ChatBotServis _chatBotServis;

        public ChatBotController(ProjeDbContext context, ChatBotServis chatBotServis)
        {
            _context = context;
            _chatBotServis = chatBotServis;
        }

        /// <summary>
        /// ChatBot'a mesaj gönder ve cevap al
        /// </summary>
        [HttpPost("mesaj")]
        public async Task<ActionResult<ChatCevapDto>> MesajGonder([FromBody] ChatMesajDto mesajDto)
        {
            try
            {
                Console.WriteLine($"[CHATBOT API] Mesaj alýndý: {mesajDto.Mesaj}");

                // Session ID kontrolü (yoksa yeni oluþtur)
                var sessionId = mesajDto.SessionId ?? Guid.NewGuid().ToString();

                // Kullanýcý ID'sini al (giriþ yapmýþsa)
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int? userId = null;
                if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int parsedUserId))
                {
                    userId = parsedUserId;
                }

                // Kullanýcý mesajýný kaydet (opsiyonel - DB'ye kaydetmek isterseniz)
                var kullaniciMesaj = new ChatMesaj
                {
                    KullaniciId = userId,
                    SessionId = sessionId,
                    Mesaj = mesajDto.Mesaj,
                    Gonderen = "user",
                    GondermeTarihi = DateTime.Now
                };
                _context.ChatMesajlari.Add(kullaniciMesaj);

                // ChatBot cevabýný üret
                var botCevap = _chatBotServis.CevapUret(mesajDto.Mesaj);
                Console.WriteLine($"[CHATBOT API] Bot cevabý: {botCevap}");

                // Bot cevabýný kaydet
                var botMesaj = new ChatMesaj
                {
                    KullaniciId = userId,
                    SessionId = sessionId,
                    Mesaj = botCevap,
                    Gonderen = "bot",
                    GondermeTarihi = DateTime.Now
                };
                _context.ChatMesajlari.Add(botMesaj);

                await _context.SaveChangesAsync();

                // Hýzlý cevaplarý al
                var hizliCevaplar = _chatBotServis.HizliCevaplarGetir();

                return Ok(new ChatCevapDto
                {
                    Cevap = botCevap,
                    SessionId = sessionId,
                    ZamanDamgasi = DateTime.Now,
                    HizliCevaplar = hizliCevaplar
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CHATBOT API] Hata: {ex.Message}");
                return StatusCode(500, new { message = "ChatBot hatasý", error = ex.Message });
            }
        }

        /// <summary>
        /// Kullanýcýnýn chat geçmiþini getir
        /// </summary>
        [HttpGet("gecmis")]
        public async Task<IActionResult> ChatGecmisiGetir([FromQuery] string? sessionId)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(sessionId))
                {
                    return BadRequest("Session ID gerekli");
                }

                var mesajlar = await Task.Run(() => _context.ChatMesajlari
                    .Where(m => m.SessionId == sessionId)
                    .OrderBy(m => m.GondermeTarihi)
                    .Select(m => new
                    {
                        m.Mesaj,
                        m.Gonderen,
                        m.GondermeTarihi
                    })
                    .ToList());

                return Ok(mesajlar);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CHATBOT API] Geçmiþ getirme hatasý: {ex.Message}");
                return StatusCode(500, new { message = "Geçmiþ getirilemedi", error = ex.Message });
            }
        }

        /// <summary>
        /// Chat oturumunu temizle
        /// </summary>
        [HttpDelete("temizle/{sessionId}")]
        public async Task<IActionResult> ChatTemizle(string sessionId)
        {
            try
            {
                var mesajlar = _context.ChatMesajlari.Where(m => m.SessionId == sessionId);
                _context.ChatMesajlari.RemoveRange(mesajlar);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Chat geçmiþi temizlendi" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CHATBOT API] Temizleme hatasý: {ex.Message}");
                return StatusCode(500, new { message = "Temizleme hatasý", error = ex.Message });
            }
        }
    }
}
