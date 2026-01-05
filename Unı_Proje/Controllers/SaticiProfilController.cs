using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uný_Proje.Data;
using Uný_Proje.DTOs;

namespace Uný_Proje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaticiProfilController : ControllerBase
    {
        private readonly ProjeDbContext _context;

        public SaticiProfilController(ProjeDbContext context)
        {
            _context = context;
        }

        // 1. SATICI PROFÝLÝNÝ GETÝR
        [HttpGet("{saticiId}")]
        public async Task<ActionResult<SaticiProfilDto>> GetSaticiProfil(int saticiId)
        {
            var satici = await _context.Kullanicilar.FindAsync(saticiId);
            if (satici == null)
                return NotFound("Satýcý bulunamadý.");

            // Satýcýnýn ürünlerini getir (sadece stoktaki)
            var urunler = await _context.Urunler
                .Where(u => u.KullaniciId == saticiId && u.StokMiktari > 0)
                .Include(u => u.Kategori)
                .OrderByDescending(u => u.EklemeTarihi)
                .Select(u => new SaticiUrunDto
                {
                    Id = u.Id,
                    Ad = u.Ad,
                    Fiyat = u.Fiyat,
                    ResimUrl = u.ResimUrl,
                    KategoriAdi = u.Kategori.Ad,
                    EklemeTarihi = u.EklemeTarihi,
                    StokMiktari = u.StokMiktari
                })
                .ToListAsync();

            // Satýcý deðerlendirmelerini getir (son 10)
            var degerlendirmeler = await _context.SaticiDegerlendirmeleri
                .Where(d => d.SaticiId == saticiId)
                .Include(d => d.Degerlendiren)
                .OrderByDescending(d => d.DegerlendirmeTarihi)
                .Take(10)
                .Select(d => new SaticiDegerlendirmeDto
                {
                    DegerlendirenAdi = d.Degerlendiren.KullaniciAdi,
                    Puan = d.Puan,
                    Yorum = d.Yorum,
                    Tarih = d.DegerlendirmeTarihi
                })
                .ToListAsync();

            // Puan daðýlýmý
            var puanDagilimi = await _context.SaticiDegerlendirmeleri
                .Where(d => d.SaticiId == saticiId)
                .GroupBy(d => d.Puan)
                .Select(g => new { Puan = g.Key, Adet = g.Count() })
                .ToListAsync();

            var profil = new SaticiProfilDto
            {
                SaticiId = satici.Id,
                SaticiAdi = satici.KullaniciAdi,
                ProfilResmi = satici.ProfilResmiUrl,
                Bio = satici.Bio,
                Telefon = satici.Telefon,
                UyelikTarihi = satici.KayitTarihi,
                OrtalamaPuan = satici.OrtalamaPuan,
                ToplamDegerlendirme = satici.ToplamDegerlendirme,
                Puan5Yildiz = puanDagilimi.FirstOrDefault(p => p.Puan == 5)?.Adet ?? 0,
                Puan4Yildiz = puanDagilimi.FirstOrDefault(p => p.Puan == 4)?.Adet ?? 0,
                Puan3Yildiz = puanDagilimi.FirstOrDefault(p => p.Puan == 3)?.Adet ?? 0,
                Puan2Yildiz = puanDagilimi.FirstOrDefault(p => p.Puan == 2)?.Adet ?? 0,
                Puan1Yildiz = puanDagilimi.FirstOrDefault(p => p.Puan == 1)?.Adet ?? 0,
                Urunler = urunler,
                Degerlendirmeler = degerlendirmeler,
                ToplamUrunSayisi = urunler.Count
            };

            return Ok(profil);
        }
    }
}
