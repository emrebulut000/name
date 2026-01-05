using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Uný_Proje.Data;
using Uný_Proje.DTOs;
using Uný_Proje.Models;

namespace Uný_Proje.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SiparislerController : ControllerBase
    {
        private readonly ProjeDbContext _context;

        public SiparislerController(ProjeDbContext context)
        {
            _context = context;
        }

        // 1. KULLANICININ TÜM SÝPARÝÞLERÝNÝ GETÝR
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<SiparisListeDto>>> GetSiparislerim()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var siparisler = await _context.Siparisler
                .Where(s => s.KullaniciId == userId)
                .Include(s => s.SiparisDetaylari)
                .OrderByDescending(s => s.SiparisTarihi)
                .Select(s => new SiparisListeDto
                {
                    Id = s.Id,
                    SiparisTarihi = s.SiparisTarihi,
                    ToplamTutar = s.ToplamTutar,
                    Durum = s.Durum.ToString(),
                    UrunSayisi = s.SiparisDetaylari.Count
                })
                .ToListAsync();

            return Ok(siparisler);
        }

        // 2. TEK BÝR SÝPARÝÞÝN DETAYINI GETÝR
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<SiparisDetayDto>> GetSiparisDetay(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var siparis = await _context.Siparisler
                .Where(s => s.Id == id && s.KullaniciId == userId)
                .Include(s => s.SiparisDetaylari)
                .ThenInclude(sd => sd.Urun)
                .FirstOrDefaultAsync();

            if (siparis == null)
                return NotFound("Sipariþ bulunamadý.");

            var detayDto = new SiparisDetayDto
            {
                Id = siparis.Id,
                SiparisTarihi = siparis.SiparisTarihi,
                ToplamTutar = siparis.ToplamTutar,
                Durum = GetDurumMetni(siparis.Durum),
                TeslimatAdresi = siparis.TeslimatAdresi,
                TeslimatTelefonu = siparis.TeslimatTelefonu,
                Urunler = siparis.SiparisDetaylari.Select(sd => new SiparisUrunDto
                {
                    Id = sd.Id,
                    UrunAdi = sd.UrunAdi,
                    BirimFiyat = sd.BirimFiyat,
                    Adet = sd.Adet,
                    AraToplam = sd.AraToplam,
                    ResimUrl = sd.ResimUrl
                }).ToList()
            };

            return Ok(detayDto);
        }

        // 3. SÝPARÝÞ ÝPTAL ET
        [HttpPut("{id}/iptal")]
        [Authorize]
        public async Task<IActionResult> SiparisIptal(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var siparis = await _context.Siparisler
                .FirstOrDefaultAsync(s => s.Id == id && s.KullaniciId == userId);

            if (siparis == null)
                return NotFound("Sipariþ bulunamadý.");

            // Sadece "Onaylandý" veya "Hazýrlanýyor" durumundaki sipariþler iptal edilebilir
            if (siparis.Durum != SiparisDurumu.Onaylandi && siparis.Durum != SiparisDurumu.Hazirlaniyor)
                return BadRequest("Bu sipariþ iptal edilemez.");

            siparis.Durum = SiparisDurumu.Iptal_Edildi;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sipariþ iptal edildi." });
        }

        // Yardýmcý metod: Enum'u Türkçe metne çevir
        private string GetDurumMetni(SiparisDurumu durum)
        {
            return durum switch
            {
                SiparisDurumu.Onaylandi => "Onaylandý",
                SiparisDurumu.Hazirlaniyor => "Hazýrlanýyor",
                SiparisDurumu.Kargoya_Verildi => "Kargoya Verildi",
                SiparisDurumu.Teslim_Edildi => "Teslim Edildi",
                SiparisDurumu.Iptal_Edildi => "Ýptal Edildi",
                _ => "Bilinmeyen"
            };
        }
    }
}
