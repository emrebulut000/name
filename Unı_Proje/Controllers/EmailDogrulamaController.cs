using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Uný_Proje.Data;
using Uný_Proje.Models;

namespace Uný_Proje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailDogrulamaController : ControllerBase
    {
        private readonly ProjeDbContext _context;
        private readonly IConfiguration _configuration;

        public EmailDogrulamaController(ProjeDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// 1. Email adresine doðrulama kodu gönder
        /// </summary>
        [HttpPost("kod-gonder")]
        public async Task<IActionResult> KodGonder([FromBody] EmailKodGonderDto dto)
        {
            try
            {
                Console.WriteLine($"[EMAIL DOÐRULAMA] Kod gönderme isteði: {dto.Email}");

                // Email formatý kontrolü
                if (!IsValidEmail(dto.Email))
                {
                    return BadRequest("Geçersiz email adresi formatý.");
                }

                // Email zaten kayýtlý mý kontrol et
                var mevcutKullanici = await _context.Kullanicilar
                    .FirstOrDefaultAsync(k => k.Email == dto.Email);

                if (mevcutKullanici != null)
                {
                    return BadRequest("Bu email adresi zaten kayýtlý.");
                }

                // 6 haneli rastgele kod oluþtur
                var kod = GenerateRandomCode();
                Console.WriteLine($"[EMAIL DOÐRULAMA] Oluþturulan kod: {kod}");

                // Eski kullanýlmamýþ tokenlarý temizle
                var eskiTokenlar = await _context.EmailDogrulamaTokenleri
                    .Where(t => t.Email == dto.Email && !t.Kullanildi)
                    .ToListAsync();
                
                _context.EmailDogrulamaTokenleri.RemoveRange(eskiTokenlar);

                // Yeni token kaydet
                var token = new EmailDogrulamaToken
                {
                    Email = dto.Email,
                    Token = kod,
                    OlusturmaTarihi = DateTime.Now,
                    SonKullanmaTarihi = DateTime.Now.AddMinutes(10), // 10 dakika geçerli
                    Kullanildi = false
                };

                _context.EmailDogrulamaTokenleri.Add(token);
                await _context.SaveChangesAsync();

                // Email gönder
                bool emailGonderildi = await EmailGonder(dto.Email, kod);

                if (!emailGonderildi)
                {
                    return StatusCode(500, "Email gönderilemedi. Lütfen daha sonra tekrar deneyin.");
                }

                Console.WriteLine($"[EMAIL DOÐRULAMA] ? Kod baþarýyla gönderildi: {dto.Email}");
                return Ok(new { message = "Doðrulama kodu email adresinize gönderildi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL DOÐRULAMA] ? Hata: {ex.Message}");
                return StatusCode(500, "Bir hata oluþtu. Lütfen daha sonra tekrar deneyin.");
            }
        }

        /// <summary>
        /// 2. Girilen kodu doðrula
        /// </summary>
        [HttpPost("kod-dogrula")]
        public async Task<IActionResult> KodDogrula([FromBody] EmailKodDogrulaDto dto)
        {
            try
            {
                Console.WriteLine($"[EMAIL DOÐRULAMA] Kod doðrulama: {dto.Email} - {dto.Kod}");

                var token = await _context.EmailDogrulamaTokenleri
                    .FirstOrDefaultAsync(t =>
                        t.Email == dto.Email &&
                        t.Token == dto.Kod &&
                        !t.Kullanildi &&
                        t.SonKullanmaTarihi > DateTime.Now
                    );

                if (token == null)
                {
                    Console.WriteLine($"[EMAIL DOÐRULAMA] ? Geçersiz kod");
                    return BadRequest("Geçersiz veya süresi dolmuþ doðrulama kodu.");
                }

                // Token'ý kullanýldý olarak iþaretle
                token.Kullanildi = true;
                await _context.SaveChangesAsync();

                Console.WriteLine($"[EMAIL DOÐRULAMA] ? Kod doðrulandý: {dto.Email}");
                return Ok(new { message = "Email adresi doðrulandý." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL DOÐRULAMA] ? Hata: {ex.Message}");
                return StatusCode(500, "Bir hata oluþtu.");
            }
        }

        // Yardýmcý metodlar

        private string GenerateRandomCode()
        {
            return Random.Shared.Next(100000, 999999).ToString();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email && email.Contains("@") && email.Contains(".");
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> EmailGonder(string toEmail, string kod)
        {
            try
            {
                // SMTP ayarlarýný appsettings.json'dan al
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var senderEmail = _configuration["Email:SenderEmail"];
                var senderPassword = _configuration["Email:SenderPassword"];
                var senderName = _configuration["Email:SenderName"] ?? "Ýkinci El Market";

                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
                {
                    Console.WriteLine("[EMAIL] ?? SMTP ayarlarý yapýlandýrýlmamýþ. Kod konsola yazýlýyor:");
                    Console.WriteLine($"[EMAIL] ?? {toEmail} ? KOD: {kod}");
                    // Geliþtirme ortamýnda true döndür
                    return true;
                }

                var smtpClient = new SmtpClient(smtpHost)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = "Email Doðrulama Kodu - Ýkinci El Market",
                    Body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f8f9fa; border-radius: 10px;'>
                                <h2 style='color: #0d6efd; text-align: center;'>?? Email Doðrulama</h2>
                                <p>Merhaba,</p>
                                <p>Ýkinci El Market'e hoþ geldiniz! Email adresinizi doðrulamak için aþaðýdaki kodu kullanýn:</p>
                                <div style='background-color: #fff; padding: 20px; border-radius: 5px; text-align: center; margin: 20px 0;'>
                                    <h1 style='color: #0d6efd; font-size: 36px; letter-spacing: 5px; margin: 0;'>{kod}</h1>
                                </div>
                                <p style='color: #666;'>Bu kod <strong>10 dakika</strong> süreyle geçerlidir.</p>
                                <p style='color: #666;'>Eðer bu iþlemi siz yapmadýysanýz, bu emaili görmezden gelebilirsiniz.</p>
                                <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;' />
                                <p style='color: #999; font-size: 12px; text-align: center;'>
                                    © 2025 Ýkinci El Market - Bulut Yazýlým Ekibi
                                </p>
                            </div>
                        </body>
                        </html>
                    ",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"[EMAIL] ? Email gönderildi: {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL] ? Email gönderme hatasý: {ex.Message}");
                // Geliþtirme ortamýnda kodu konsola yaz
                Console.WriteLine($"[EMAIL] ?? {toEmail} ? KOD: {kod}");
                return true; // Geliþtirme ortamýnda true döndür
            }
        }
    }

    // DTOs - Email Doðrulama için özel isimler
    public class EmailKodGonderDto
    {
        public string Email { get; set; } = string.Empty;
    }

    public class EmailKodDogrulaDto
    {
        public string Email { get; set; } = string.Empty;
        public string Kod { get; set; } = string.Empty;
    }
}
