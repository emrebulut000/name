using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Uný_Proje.Data;
using Uný_Proje.DTOs;
using Uný_Proje.Models;

namespace Uný_Proje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrunResimController : ControllerBase
    {
        private readonly ProjeDbContext _context;

        public UrunResimController(ProjeDbContext context)
        {
            _context = context;
        }

        // 1. ÜRÜNÜN TÜM RESÝMLERÝNÝ GETÝR
        [HttpGet("urun/{urunId}")]
        public async Task<ActionResult<List<UrunResimDto>>> GetUrunResimleri(int urunId)
        {
            var resimler = await _context.UrunResimleri
                .Where(r => r.UrunId == urunId)
                .OrderBy(r => r.Sira)
                .Select(r => new UrunResimDto
                {
                    Id = r.Id,
                    UrunId = r.UrunId,
                    ResimUrl = r.ResimUrl,
                    Sira = r.Sira,
                    AnaResimMi = r.AnaResimMi
                })
                .ToListAsync();

            return Ok(resimler);
        }

        // 2. TEK RESÝM EKLE
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UrunResimDto>> ResimEkle(UrunResimEkleDto dto)
        {
            // Kullanýcý doðrulama
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Geçersiz kullanýcý kimliði.");
            }

            // Ürün kontrolü ve yetki kontrolü
            var urun = await _context.Urunler.FindAsync(dto.UrunId);
            if (urun == null)
            {
                return NotFound("Ürün bulunamadý.");
            }

            if (urun.KullaniciId != userId)
            {
                return Forbid("Bu ürüne resim ekleme yetkiniz yok.");
            }

            // Resim sayýsý kontrolü (max 5)
            var mevcutResimSayisi = await _context.UrunResimleri
                .CountAsync(r => r.UrunId == dto.UrunId);

            if (mevcutResimSayisi >= 5)
            {
                return BadRequest("Bir ürüne en fazla 5 resim ekleyebilirsiniz.");
            }

            // Eðer ana resim seçildiyse, diðer ana resimleri kaldýr
            if (dto.AnaResimMi)
            {
                var digerAnaResimler = await _context.UrunResimleri
                    .Where(r => r.UrunId == dto.UrunId && r.AnaResimMi)
                    .ToListAsync();

                foreach (var resim in digerAnaResimler)
                {
                    resim.AnaResimMi = false;
                }
            }

            // Yeni resmi ekle
            var yeniResim = new UrunResim
            {
                UrunId = dto.UrunId,
                ResimUrl = dto.ResimUrl,
                Sira = dto.Sira > 0 ? dto.Sira : mevcutResimSayisi + 1,
                AnaResimMi = dto.AnaResimMi,
                EklemeTarihi = DateTime.Now
            };

            _context.UrunResimleri.Add(yeniResim);
            await _context.SaveChangesAsync();

            return Ok(new UrunResimDto
            {
                Id = yeniResim.Id,
                UrunId = yeniResim.UrunId,
                ResimUrl = yeniResim.ResimUrl,
                Sira = yeniResim.Sira,
                AnaResimMi = yeniResim.AnaResimMi
            });
        }

        // 3. ÇOKLU RESÝM EKLE
        [HttpPost("coklu")]
        [Authorize]
        public async Task<ActionResult> CokluResimEkle(CokluResimYukleDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var urun = await _context.Urunler.FindAsync(dto.UrunId);
            if (urun == null || urun.KullaniciId != userId)
            {
                return Forbid();
            }

            var mevcutResimSayisi = await _context.UrunResimleri
                .CountAsync(r => r.UrunId == dto.UrunId);

            if (mevcutResimSayisi + dto.ResimUrlListesi.Count > 5)
            {
                return BadRequest($"Toplam resim sayýsý 5'i geçemez. Þu an {mevcutResimSayisi} resim var.");
            }

            // Tüm ana resimleri kaldýr
            var mevcutAnaResimler = await _context.UrunResimleri
                .Where(r => r.UrunId == dto.UrunId && r.AnaResimMi)
                .ToListAsync();

            foreach (var resim in mevcutAnaResimler)
            {
                resim.AnaResimMi = false;
            }

            // Yeni resimleri ekle
            var yeniResimler = new List<UrunResim>();
            for (int i = 0; i < dto.ResimUrlListesi.Count; i++)
            {
                var yeniResim = new UrunResim
                {
                    UrunId = dto.UrunId,
                    ResimUrl = dto.ResimUrlListesi[i],
                    Sira = mevcutResimSayisi + i + 1,
                    AnaResimMi = dto.AnaResimIndex.HasValue && dto.AnaResimIndex.Value == i,
                    EklemeTarihi = DateTime.Now
                };
                yeniResimler.Add(yeniResim);
            }

            _context.UrunResimleri.AddRange(yeniResimler);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"{yeniResimler.Count} resim baþarýyla eklendi.", eklenenSayisi = yeniResimler.Count });
        }

        // 4. RESÝM SÝL
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> ResimSil(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var resim = await _context.UrunResimleri
                .Include(r => r.Urun)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (resim == null)
            {
                return NotFound("Resim bulunamadý.");
            }

            if (resim.Urun?.KullaniciId != userId)
            {
                return Forbid("Bu resmi silme yetkiniz yok.");
            }

            _context.UrunResimleri.Remove(resim);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Resim silindi." });
        }

        // 5. ANA RESÝM AYARLA
        [HttpPut("ana-resim/{id}")]
        [Authorize]
        public async Task<ActionResult> AnaResimAyarla(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var resim = await _context.UrunResimleri
                .Include(r => r.Urun)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (resim == null)
            {
                return NotFound();
            }

            if (resim.Urun?.KullaniciId != userId)
            {
                return Forbid();
            }

            // Diðer ana resimleri kaldýr
            var digerAnaResimler = await _context.UrunResimleri
                .Where(r => r.UrunId == resim.UrunId && r.AnaResimMi && r.Id != id)
                .ToListAsync();

            foreach (var r in digerAnaResimler)
            {
                r.AnaResimMi = false;
            }

            // Bu resmi ana resim yap
            resim.AnaResimMi = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ana resim ayarlandý." });
        }

        // 6. RESÝM SIRASINI GÜNCELLE
        [HttpPut("sira")]
        [Authorize]
        public async Task<ActionResult> SiraGuncelle([FromBody] List<SiraGuncelleDto> siralar)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            foreach (var sira in siralar)
            {
                var resim = await _context.UrunResimleri
                    .Include(r => r.Urun)
                    .FirstOrDefaultAsync(r => r.Id == sira.Id);

                if (resim != null && resim.Urun?.KullaniciId == userId)
                {
                    resim.Sira = sira.YeniSira;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Sýralama güncellendi." });
        }
    }

    // Sýra güncelleme DTO'su
    public class SiraGuncelleDto
    {
        public int Id { get; set; }
        public int YeniSira { get; set; }
    }
}
