using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uný_Proje.Data;
using Uný_Proje.Models;
using Uný_Proje.Services;

namespace Uný_Proje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SifreSifirlaController : ControllerBase
    {
        private readonly ProjeDbContext _context;
        private readonly EmailServis _emailServis;

        public SifreSifirlaController(ProjeDbContext context, EmailServis emailServis)
        {
            _context = context;
            _emailServis = emailServis;
        }

        // 1. ÞÝFRE SIFIRLAMA KODU GÖNDER
        [HttpPost("kod-gonder")]
        public async Task<IActionResult> KodGonder([FromBody] KodGonderDto dto)
        {
            try
            {
                // Email ile kullanýcýyý bul
                var kullanici = await _context.Kullanicilar
                    .FirstOrDefaultAsync(k => k.Email == dto.Email);

                if (kullanici == null)
                {
                    return NotFound("Bu email adresi ile kayýtlý kullanýcý bulunamadý.");
                }

                // 6 haneli rastgele kod oluþtur
                var resetKodu = GenerateRandomCode();

                // Eski tokenlarý sil (kullanýlmamýþ olanlarý)
                var eskiTokenlar = await _context.SifreResetTokenleri
                    .Where(t => t.KullaniciId == kullanici.Id && !t.Kullanildi)
                    .ToListAsync();
                _context.SifreResetTokenleri.RemoveRange(eskiTokenlar);

                // Yeni token oluþtur
                var token = new SifreResetToken
                {
                    KullaniciId = kullanici.Id,
                    Token = resetKodu,
                    OlusturmaTarihi = DateTime.Now,
                    SonKullanmaTarihi = DateTime.Now.AddMinutes(15), // 15 dakika geçerli
                    Kullanildi = false
                };

                _context.SifreResetTokenleri.Add(token);
                await _context.SaveChangesAsync();

                // Email gönder
                var emailHtml = _emailServis.SifreResetEmailHtml(kullanici.KullaniciAdi, resetKodu);
                var emailGonderildi = await _emailServis.EmailGonder(
                    kullanici.Email,
                    "Þifre Sýfýrlama Kodu",
                    emailHtml
                );

                if (!emailGonderildi)
                {
                    // Email gönderilemedi ama kodu console'a yazdýr (development için)
                    Console.WriteLine($"?? EMAIL GÖNDERÝLEMEDÝ! Reset Kodu: {resetKodu} (Kullanýcý: {kullanici.Email})");
                    
                    // Development ortamýnda kodu response'ta döndür
                    return Ok(new { 
                        message = "Email gönderilemedi. Geliþtirme modu: Kod console'da gösteriliyor.",
                        resetKodu = resetKodu // SADECE TEST ÝÇÝN!
                    });
                }

                Console.WriteLine($"? Email gönderildi: {kullanici.Email} - Kod: {resetKodu}");
                return Ok(new { message = "Þifre sýfýrlama kodu email adresinize gönderildi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kod gönderme hatasý: {ex.Message}");
                return StatusCode(500, "Bir hata oluþtu. Lütfen daha sonra tekrar deneyin.");
            }
        }

        // 2. KODU DOÐRULA VE YENÝ ÞÝFRE OLUÞTUR
        [HttpPost("sifre-yenile")]
        public async Task<IActionResult> SifreYenile([FromBody] SifreYenileDto dto)
        {
            try
            {
                // Kullanýcýyý bul
                var kullanici = await _context.Kullanicilar
                    .FirstOrDefaultAsync(k => k.Email == dto.Email);

                if (kullanici == null)
                {
                    return NotFound("Kullanýcý bulunamadý.");
                }

                // Token'ý bul ve doðrula
                var token = await _context.SifreResetTokenleri
                    .FirstOrDefaultAsync(t => 
                        t.KullaniciId == kullanici.Id && 
                        t.Token == dto.Kod && 
                        !t.Kullanildi &&
                        t.SonKullanmaTarihi > DateTime.Now
                    );

                if (token == null)
                {
                    return BadRequest("Geçersiz veya süresi dolmuþ doðrulama kodu.");
                }

                // Þifreyi BCrypt ile hashle ve güncelle (diðer yerlerle uyumlu)
                kullanici.SifreHash = BCrypt.Net.BCrypt.HashPassword(dto.YeniSifre);
                
                // Token'ý kullanýldý olarak iþaretle
                token.Kullanildi = true;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Þifreniz baþarýyla güncellendi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Þifre yenileme hatasý: {ex.Message}");
                return StatusCode(500, "Bir hata oluþtu. Lütfen daha sonra tekrar deneyin.");
            }
        }

        // 3. KODU DOÐRULA (Sadece kontrol için)
        [HttpPost("kod-dogrula")]
        public async Task<IActionResult> KodDogrula([FromBody] KodDogrulaDto dto)
        {
            try
            {
                var kullanici = await _context.Kullanicilar
                    .FirstOrDefaultAsync(k => k.Email == dto.Email);

                if (kullanici == null)
                {
                    return NotFound("Kullanýcý bulunamadý.");
                }

                var token = await _context.SifreResetTokenleri
                    .FirstOrDefaultAsync(t => 
                        t.KullaniciId == kullanici.Id && 
                        t.Token == dto.Kod && 
                        !t.Kullanildi &&
                        t.SonKullanmaTarihi > DateTime.Now
                    );

                if (token == null)
                {
                    return BadRequest("Geçersiz veya süresi dolmuþ doðrulama kodu.");
                }

                return Ok(new { message = "Kod doðrulandý." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kod doðrulama hatasý: {ex.Message}");
                return StatusCode(500, "Bir hata oluþtu.");
            }
        }

        // Yardýmcý metodlar
        private string GenerateRandomCode()
        {
            return Random.Shared.Next(100000, 999999).ToString();
        }
    }

    // DTOs
    public class KodGonderDto
    {
        public string Email { get; set; } = "";
    }

    public class SifreYenileDto
    {
        public string Email { get; set; } = "";
        public string Kod { get; set; } = "";
        public string YeniSifre { get; set; } = "";
    }

    public class KodDogrulaDto
    {
        public string Email { get; set; } = "";
        public string Kod { get; set; } = "";
    }
}
