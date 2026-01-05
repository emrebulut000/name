using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Uný_Proje.Data;
using System.ComponentModel.DataAnnotations;

namespace Uný_Proje.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KullanicilarController : ControllerBase
    {
        private readonly ProjeDbContext _context;

        public KullanicilarController(ProjeDbContext context)
        {
            _context = context;
        }

        // 1. PROFÝL BÝLGÝLERÝNÝ GETÝR (GET /api/kullanicilar/ben)
        [HttpGet("ben")]
        [Authorize]
        public async Task<IActionResult> GetProfil()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                Console.WriteLine($"[PROFIL BEN] User ID String: {userIdString}");
                
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    Console.WriteLine("[PROFIL BEN] Unauthorized - UserId parse edilemedi");
                    return Unauthorized("Kullanýcý kimliði doðrulanamadý");
                }

                Console.WriteLine($"[PROFIL BEN] Kullanýcý ID: {userId} için profil getiriliyor...");

                var kullanici = await _context.Kullanicilar.FindAsync(userId);
                
                if (kullanici == null)
                {
                    Console.WriteLine($"[PROFIL BEN] Kullanýcý {userId} bulunamadý");
                    return NotFound("Kullanýcý bulunamadý.");
                }

                Console.WriteLine($"[PROFIL BEN] Kullanýcý bulundu: {kullanici.KullaniciAdi}");

                return Ok(new 
                {
                    id = kullanici.Id,
                    adSoyad = kullanici.KullaniciAdi,
                    email = kullanici.Email,
                    profilResmiUrl = kullanici.ProfilResmiUrl,
                    telefon = kullanici.Telefon,
                    bio = kullanici.Bio,
                    kayitTarihi = kullanici.KayitTarihi
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PROFIL BEN HATA] {ex.Message}");
                Console.WriteLine($"[PROFIL BEN STACK] {ex.StackTrace}");
                return StatusCode(500, new { message = "Profil getirilirken hata oluþtu", error = ex.Message });
            }
        }

        // 2. PROFÝLÝ GÜNCELLE (PUT /api/kullanicilar/guncelle)
        [HttpPut("guncelle")]
        [Authorize]
        public async Task<IActionResult> GuncelleProfil([FromBody] ProfilGuncelleRequest dto)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                    return Unauthorized();

                Console.WriteLine($"[PROFIL GÜNCELLE] Kullanýcý {userId} profili güncelleniyor...");

                var kullanici = await _context.Kullanicilar.FindAsync(userId);
                if (kullanici == null)
                {
                    Console.WriteLine($"[PROFIL GÜNCELLE] Kullanýcý {userId} bulunamadý");
                    return NotFound();
                }

                // Bilgileri güncelle
                kullanici.KullaniciAdi = dto.AdSoyad;
                kullanici.ProfilResmiUrl = dto.ProfilResmiUrl;
                kullanici.Telefon = dto.Telefon;
                kullanici.Bio = dto.Bio;

                // Þifre Deðiþikliði Kontrolü
                if (!string.IsNullOrEmpty(dto.MevcutSifre) && !string.IsNullOrEmpty(dto.YeniSifre))
                {
                    Console.WriteLine($"[PROFIL GÜNCELLE] Þifre deðiþtirme isteði");
                    
                    // Mevcut þifreyi BCrypt ile doðrula
                    if (!BCrypt.Net.BCrypt.Verify(dto.MevcutSifre, kullanici.SifreHash))
                    {
                        Console.WriteLine($"[PROFIL GÜNCELLE] Mevcut þifre hatalý");
                        return BadRequest("Mevcut þifreniz yanlýþ.");
                    }

                    // Yeni þifreyi hashle ve kaydet
                    kullanici.SifreHash = BCrypt.Net.BCrypt.HashPassword(dto.YeniSifre);
                    Console.WriteLine($"[PROFIL GÜNCELLE] Þifre baþarýyla deðiþtirildi");
                }

                await _context.SaveChangesAsync();
                Console.WriteLine($"[PROFIL GÜNCELLE] Profil baþarýyla güncellendi");
                
                return Ok(new { message = "Profil güncellendi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PROFIL GÜNCELLE HATA] {ex.Message}");
                Console.WriteLine($"[PROFIL GÜNCELLE STACK] {ex.StackTrace}");
                return StatusCode(500, new { message = "Profil güncellenirken hata oluþtu", error = ex.Message });
            }
        }
    }

    // Request DTO
    public class ProfilGuncelleRequest
    {
        public string AdSoyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfilResmiUrl { get; set; }
        public string? Telefon { get; set; }
        public string? Bio { get; set; }
        public DateTime? KayitTarihi { get; set; }
        public string? MevcutSifre { get; set; }
        public string? YeniSifre { get; set; }
    }
}
