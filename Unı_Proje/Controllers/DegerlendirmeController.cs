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
    public class DegerlendirmeController : ControllerBase
    {
        private readonly ProjeDbContext _context;

        public DegerlendirmeController(ProjeDbContext context)
        {
            _context = context;
        }

        // 1. DEÐERLENDÝRME OLUÞTUR (Satýn alma sonrasý)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DegerlendirmeYap([FromBody] DegerlendirmeOlusturDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Sipariþ kontrolü - Bu kullanýcýnýn sipariþi mi?
            var siparis = await _context.Siparisler
                .Include(s => s.SiparisDetaylari)
                .ThenInclude(d => d.Urun)
                .FirstOrDefaultAsync(s => s.Id == dto.SiparisId && s.KullaniciId == userId);

            if (siparis == null)
                return NotFound("Sipariþ bulunamadý veya size ait deðil.");

            // Sipariþ durumu kontrolü (Sadece tamamlanan sipariþler için)
            if (siparis.Durum != SiparisDurumu.Teslim_Edildi)
                return BadRequest("Sadece tamamlanan sipariþleri deðerlendirebilirsiniz.");

            // Satýcý ID'sini al (Ýlk ürünün satýcýsý)
            var saticiId = siparis.SiparisDetaylari.FirstOrDefault()?.Urun?.KullaniciId;
            if (saticiId == null)
                return BadRequest("Satýcý bilgisi bulunamadý.");

            // Daha önce deðerlendirme yapýlmýþ mý?
            var mevcutDegerlendirme = await _context.SaticiDegerlendirmeleri
                .AnyAsync(d => d.SiparisId == dto.SiparisId && d.DegerlendirenId == userId);

            if (mevcutDegerlendirme)
                return BadRequest("Bu sipariþ için zaten deðerlendirme yaptýnýz.");

            // Yeni deðerlendirme oluþtur
            var degerlendirme = new SaticiDegerlendirme
            {
                SaticiId = saticiId.Value,
                DegerlendirenId = userId,
                SiparisId = dto.SiparisId,
                Puan = dto.Puan,
                Yorum = dto.Yorum,
                DegerlendirmeTarihi = DateTime.Now
            };

            _context.SaticiDegerlendirmeleri.Add(degerlendirme);
            await _context.SaveChangesAsync();

            // Satýcýnýn ortalama puanýný güncelle
            await SaticiPuanGuncelle(saticiId.Value);

            return Ok(new { message = "Deðerlendirmeniz kaydedildi. Teþekkürler!" });
        }

        // 2. SATICI DEÐERLENDÝRMELERÝNÝ GETÝR
        [HttpGet("satici/{saticiId}")]
        public async Task<ActionResult<List<DegerlendirmeDto>>> SaticiDegerlendirmeleri(int saticiId)
        {
            var degerlendirmeler = await _context.SaticiDegerlendirmeleri
                .Where(d => d.SaticiId == saticiId)
                .Include(d => d.Degerlendiren)
                .Include(d => d.Satici)
                .OrderByDescending(d => d.DegerlendirmeTarihi)
                .Select(d => new DegerlendirmeDto
                {
                    Id = d.Id,
                    SaticiId = d.SaticiId,
                    SaticiAdi = d.Satici.KullaniciAdi,
                    DegerlendirenId = d.DegerlendirenId,
                    DegerlendirenAdi = d.Degerlendiren.KullaniciAdi,
                    Puan = d.Puan,
                    Yorum = d.Yorum,
                    DegerlendirmeTarihi = d.DegerlendirmeTarihi
                })
                .ToListAsync();

            return Ok(degerlendirmeler);
        }

        // 3. SATICI PUAN ÖZETÝ (Ýstatistikler)
        [HttpGet("satici/{saticiId}/ozet")]
        public async Task<ActionResult<SaticiPuanOzetiDto>> SaticiPuanOzeti(int saticiId)
        {
            var satici = await _context.Kullanicilar.FindAsync(saticiId);
            if (satici == null)
                return NotFound("Satýcý bulunamadý.");

            var degerlendirmeler = await _context.SaticiDegerlendirmeleri
                .Where(d => d.SaticiId == saticiId)
                .ToListAsync();

            var ozet = new SaticiPuanOzetiDto
            {
                SaticiId = saticiId,
                SaticiAdi = satici.KullaniciAdi,
                OrtalamaPuan = satici.OrtalamaPuan,
                ToplamDegerlendirme = satici.ToplamDegerlendirme,
                Puan5Yildiz = degerlendirmeler.Count(d => d.Puan == 5),
                Puan4Yildiz = degerlendirmeler.Count(d => d.Puan == 4),
                Puan3Yildiz = degerlendirmeler.Count(d => d.Puan == 3),
                Puan2Yildiz = degerlendirmeler.Count(d => d.Puan == 2),
                Puan1Yildiz = degerlendirmeler.Count(d => d.Puan == 1)
            };

            return Ok(ozet);
        }

        // 4. DEÐERLENDÝRME YAPILDI MI KONTROLÜ
        [HttpGet("kontrol/{siparisId}")]
        [Authorize]
        public async Task<ActionResult<bool>> DegerlendirmeYapildiMi(int siparisId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var yapildiMi = await _context.SaticiDegerlendirmeleri
                .AnyAsync(d => d.SiparisId == siparisId && d.DegerlendirenId == userId);

            return Ok(yapildiMi);
        }

        // 5. BENÝM DEÐERLENDÝRMELERÝM (Kullanýcýnýn yaptýðý deðerlendirmeler)
        [HttpGet("benimlerim")]
        [Authorize]
        public async Task<ActionResult<List<DegerlendirmeDto>>> BenimDegerlendirmelerim()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var degerlendirmeler = await _context.SaticiDegerlendirmeleri
                .Where(d => d.DegerlendirenId == userId)
                .Include(d => d.Satici)
                .Include(d => d.Degerlendiren)
                .OrderByDescending(d => d.DegerlendirmeTarihi)
                .Select(d => new DegerlendirmeDto
                {
                    Id = d.Id,
                    SaticiId = d.SaticiId,
                    SaticiAdi = d.Satici.KullaniciAdi,
                    DegerlendirenId = d.DegerlendirenId,
                    DegerlendirenAdi = d.Degerlendiren.KullaniciAdi,
                    Puan = d.Puan,
                    Yorum = d.Yorum,
                    DegerlendirmeTarihi = d.DegerlendirmeTarihi
                })
                .ToListAsync();

            return Ok(degerlendirmeler);
        }

        // YARDIMCI METOD: Satýcýnýn ortalama puanýný hesapla ve güncelle
        private async Task SaticiPuanGuncelle(int saticiId)
        {
            var degerlendirmeler = await _context.SaticiDegerlendirmeleri
                .Where(d => d.SaticiId == saticiId)
                .ToListAsync();

            var satici = await _context.Kullanicilar.FindAsync(saticiId);
            if (satici != null)
            {
                satici.ToplamDegerlendirme = degerlendirmeler.Count;
                satici.OrtalamaPuan = degerlendirmeler.Any()
                    ? (decimal)Math.Round(degerlendirmeler.Average(d => d.Puan), 2)
                    : null;

                await _context.SaveChangesAsync();
            }
        }
    }
}
