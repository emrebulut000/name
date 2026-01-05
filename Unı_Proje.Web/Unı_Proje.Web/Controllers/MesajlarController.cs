using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Uný_Proje.Data;
using Uný_Proje.Models;

namespace Uný_Proje.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MesajlarController : ControllerBase
    {
        private readonly ProjeDbContext _context;

        public MesajlarController(ProjeDbContext context)
        {
            _context = context;
        }

        // 1. KULLANICININ TÜM KONUÞMALARINI GETÝR (Konuþma listesi)
        [HttpGet("konusmalar")]
        [Authorize]
        public async Task<ActionResult> GetKonusmalar()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Kullanýcýnýn hem gönderdiði hem aldýðý mesajlarý al
                var mesajlar = await _context.Mesajlar
                    .Where(m => m.GonderenId == userId || m.AliciId == userId)
                    .Include(m => m.Gonderen)
                    .Include(m => m.Alici)
                    .Include(m => m.Urun)
                    .OrderByDescending(m => m.GonderimTarihi)
                    .ToListAsync();

                // Konuþmalarý grupla (her kullanýcý için en son mesaj)
                var konusmalar = mesajlar
                    .GroupBy(m => m.GonderenId == userId ? m.AliciId : m.GonderenId)
                    .Select(g =>
                    {
                        var sonMesaj = g.First();
                        var karsiTaraf = sonMesaj.GonderenId == userId ? sonMesaj.Alici : sonMesaj.Gonderen;
                        var okunmamisSayisi = g.Count(m => m.AliciId == userId && !m.Okundu);

                        return new
                        {
                            KarsiTarafId = karsiTaraf.Id,
                            KarsiTarafAdi = karsiTaraf.KullaniciAdi,
                            KarsiTarafResim = karsiTaraf.ProfilResmiUrl,
                            SonMesaj = sonMesaj.Icerik,
                            SonMesajTarihi = sonMesaj.GonderimTarihi,
                            UrunId = sonMesaj.UrunId,
                            UrunAd = sonMesaj.Urun?.Ad,
                            OkunmamisSayisi = okunmamisSayisi,
                            BenGonderdim = sonMesaj.GonderenId == userId
                        };
                    })
                    .OrderByDescending(k => k.SonMesajTarihi)
                    .ToList();

                return Ok(konusmalar);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Konuþmalar getirme hatasý: {ex.Message}");
                return Ok(new List<object>());
            }
        }

        // 2. BÝR KULLANICI ÝLE MESAJLAÞMAYI GETÝR
        [HttpGet("konusma/{karsiTarafId}")]
        [Authorize]
        public async Task<ActionResult> GetKonusma(int karsiTarafId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var mesajlar = await _context.Mesajlar
                    .Where(m => (m.GonderenId == userId && m.AliciId == karsiTarafId) ||
                               (m.GonderenId == karsiTarafId && m.AliciId == userId))
                    .Include(m => m.Gonderen)
                    .Include(m => m.Alici)
                    .Include(m => m.Urun)
                    .OrderBy(m => m.GonderimTarihi)
                    .Select(m => new
                    {
                        Id = m.Id,
                        GonderenId = m.GonderenId,
                        GonderenAdi = m.Gonderen.KullaniciAdi,
                        AliciId = m.AliciId,
                        AliciAdi = m.Alici.KullaniciAdi,
                        Icerik = m.Icerik,
                        GonderimTarihi = m.GonderimTarihi,
                        Okundu = m.Okundu,
                        UrunId = m.UrunId,
                        UrunAd = m.Urun != null ? m.Urun.Ad : null,
                        UrunResim = m.Urun != null ? m.Urun.ResimUrl : null
                    })
                    .ToListAsync();

                // Okunmamýþ mesajlarý okundu olarak iþaretle
                var okunmamisMesajlar = await _context.Mesajlar
                    .Where(m => m.AliciId == userId && m.GonderenId == karsiTarafId && !m.Okundu)
                    .ToListAsync();

                foreach (var mesaj in okunmamisMesajlar)
                {
                    mesaj.Okundu = true;
                    mesaj.OkunmaTarihi = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return Ok(mesajlar);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Konuþma getirme hatasý: {ex.Message}");
                return Ok(new List<object>());
            }
        }

        // 3. MESAJ GÖNDER
        [HttpPost("gonder")]
        [Authorize]
        public async Task<IActionResult> MesajGonder([FromBody] MesajGonderDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Kendine mesaj gönderme kontrolü
                if (userId == dto.AliciId)
                {
                    return BadRequest("Kendinize mesaj gönderemezsiniz.");
                }

                var yeniMesaj = new Mesaj
                {
                    GonderenId = userId,
                    AliciId = dto.AliciId,
                    UrunId = dto.UrunId,
                    Icerik = dto.Icerik,
                    GonderimTarihi = DateTime.Now,
                    Okundu = false
                };

                _context.Mesajlar.Add(yeniMesaj);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Mesaj gönderildi.", mesajId = yeniMesaj.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mesaj gönderme hatasý: {ex.Message}");
                return StatusCode(500, "Mesaj gönderilemedi.");
            }
        }

        // 4. OKUNMAMIÞ MESAJ SAYISI
        [HttpGet("okunmamis-sayisi")]
        [Authorize]
        public async Task<ActionResult<int>> GetOkunmamisSayisi()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var sayisi = await _context.Mesajlar
                    .CountAsync(m => m.AliciId == userId && !m.Okundu);

                return Ok(sayisi);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Okunmamýþ sayýsý hatasý: {ex.Message}");
                return Ok(0);
            }
        }

        // 5. MESAJ SÝL
        [HttpDelete("sil/{mesajId}")]
        [Authorize]
        public async Task<IActionResult> MesajSil(int mesajId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var mesaj = await _context.Mesajlar
                    .FirstOrDefaultAsync(m => m.Id == mesajId && 
                        (m.GonderenId == userId || m.AliciId == userId));

                if (mesaj == null)
                {
                    return NotFound("Mesaj bulunamadý.");
                }

                _context.Mesajlar.Remove(mesaj);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Mesaj silindi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mesaj silme hatasý: {ex.Message}");
                return StatusCode(500, "Mesaj silinemedi.");
            }
        }
    }

    // DTO (Data Transfer Object)
    public class MesajGonderDto
    {
        public int AliciId { get; set; }
        public int? UrunId { get; set; }
        public string Icerik { get; set; } = string.Empty;
    }
}
