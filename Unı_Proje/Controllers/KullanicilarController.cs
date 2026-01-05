using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Unı_Proje.Data;
using Unı_Proje.DTOs;
using Unı_Proje.Models;

namespace Unı_Proje.Controllers
{
    // DEVRE DIŞI - Port 7000'deki controller kullanılıyor
    // [Route("api/[controller]")]
    [Route("api/v1/[controller]")] // Farklı route
    [ApiController]
    public class KullanicilarController : ControllerBase
    {
        private readonly ProjeDbContext _context;

        public KullanicilarController(ProjeDbContext context)
        {
            _context = context;
        }

        // 1. PROFİL BİLGİLERİNİ GETİR (GET /api/kullanicilar/ben)
        [HttpGet("ben")]
        [Authorize]
        public async Task<ActionResult<KullaniciProfilDto>> GetProfil()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                return Unauthorized();

            var kullanici = await _context.Kullanicilar.FindAsync(userId);
            if (kullanici == null) return NotFound("Kullanıcı bulunamadı.");

            return new KullaniciProfilDto
            {
                AdSoyad = kullanici.KullaniciAdi,
                Email = kullanici.Email,
                ProfilResmiUrl = kullanici.ProfilResmiUrl,

                // 👇 YENİ EKLENENLER: Veritabanından okuyup Frontend'e gönderiyoruz
                Telefon = kullanici.Telefon,
                Bio = kullanici.Bio,
                KayitTarihi = kullanici.KayitTarihi
            };
        }

        // 2. PROFİLİ GÜNCELLE (PUT /api/kullanicilar/guncelle)
        [HttpPut("guncelle")]
        [Authorize]
        public async Task<IActionResult> GuncelleProfil(KullaniciProfilDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                return Unauthorized();

            var kullanici = await _context.Kullanicilar.FindAsync(userId);
            if (kullanici == null) return NotFound();

            // Bilgileri güncelle
            kullanici.KullaniciAdi = dto.AdSoyad;
            kullanici.ProfilResmiUrl = dto.ProfilResmiUrl;

            // 👇 YENİ EKLENENLER: Frontend'den gelen veriyi veritabanına yazıyoruz
            kullanici.Telefon = dto.Telefon;
            kullanici.Bio = dto.Bio;

            // Şifre Değişikliği Kontrolü
            if (!string.IsNullOrEmpty(dto.MevcutSifre) && !string.IsNullOrEmpty(dto.YeniSifre))
            {
                // Mevcut şifreyi BCrypt ile doğrula
                if (!BCrypt.Net.BCrypt.Verify(dto.MevcutSifre, kullanici.SifreHash))
                {
                    return BadRequest("Mevcut şifreniz yanlış.");
                }

                // Yeni şifreyi hashle ve kaydet
                kullanici.SifreHash = BCrypt.Net.BCrypt.HashPassword(dto.YeniSifre);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Profil güncellendi." });
        }
    }
}