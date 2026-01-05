using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Uný_Proje.Data;
using Uný_Proje.Models;

namespace Uný_Proje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TekliflerController : ControllerBase
    {
        private readonly ProjeDbContext _context;

        public TekliflerController(ProjeDbContext context)
        {
            _context = context;
        }

        // 1. TEKLÝF GÖNDER (Alýcý)
        [HttpPost]
        public async Task<IActionResult> TeklifGonder([FromBody] TeklifGonderDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Ürünü kontrol et
                var urun = await _context.Urunler
                    .Include(u => u.Kullanici)
                    .FirstOrDefaultAsync(u => u.Id == dto.UrunId);

                if (urun == null)
                    return NotFound("Ürün bulunamadý.");

                // Kendi ürününe teklif veremez
                if (urun.KullaniciId == userId)
                    return BadRequest("Kendi ürününüze teklif veremezsiniz.");

                // Ayný ürüne bekleyen teklifi var mý kontrol et
                var mevcutTeklif = await _context.Teklifler
                    .FirstOrDefaultAsync(t => t.UrunId == dto.UrunId && 
                                             t.AliciId == userId && 
                                             t.Durum == TeklifDurumu.Bekliyor);

                if (mevcutTeklif != null)
                    return BadRequest("Bu ürün için zaten bekleyen bir teklifiniz var.");

                // Teklif tutarý kontrolü
                if (dto.TeklifTutari <= 0)
                    return BadRequest("Teklif tutarý 0'dan büyük olmalýdýr.");

                if (dto.TeklifTutari >= urun.Fiyat)
                    return BadRequest($"Teklif tutarý ürün fiyatýndan ({urun.Fiyat:N2} TL) düþük olmalýdýr.");

                // Yeni teklif oluþtur
                var teklif = new Teklif
                {
                    UrunId = dto.UrunId,
                    AliciId = userId,
                    TeklifTutari = dto.TeklifTutari,
                    Aciklama = dto.Aciklama,
                    Durum = TeklifDurumu.Bekliyor,
                    OlusturmaTarihi = DateTime.Now
                };

                _context.Teklifler.Add(teklif);
                await _context.SaveChangesAsync();

                // Satýcýya bildirim gönder
                await BildirimlerController.BildirimOlustur(
                    _context,
                    urun.KullaniciId,
                    BildirimTipleri.Satis,
                    "Yeni Teklif Aldýnýz",
                    $"{urun.Ad} için {dto.TeklifTutari:N2} TL teklif aldýnýz.",
                    teklif.Id
                );

                Console.WriteLine($"[TEKLÝF] Kullanýcý {userId}, ürün #{dto.UrunId} için {dto.TeklifTutari:N2} TL teklif gönderdi.");

                return Ok(new { message = "Teklifiniz baþarýyla gönderildi.", teklifId = teklif.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Teklif gönderme hatasý: {ex.Message}");
                return StatusCode(500, "Bir hata oluþtu.");
            }
        }

        // 2. TEKLÝFLERÝMÝ GETÝR (Alýcý - gönderdiði teklifler)
        [HttpGet("gonderdigim")]
        public async Task<ActionResult> GonderdigimTeklifleriGetir()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var teklifler = await _context.Teklifler
                .Include(t => t.Urun)
                    .ThenInclude(u => u.Kullanici)
                .Include(t => t.Urun.Kategori)
                .Where(t => t.AliciId == userId)
                .OrderByDescending(t => t.OlusturmaTarihi)
                .Select(t => new
                {
                    t.Id,
                    UrunId = t.Urun.Id,
                    UrunAd = t.Urun.Ad,
                    UrunResim = t.Urun.ResimUrl,
                    UrunFiyat = t.Urun.Fiyat,
                    KategoriAd = t.Urun.Kategori != null ? t.Urun.Kategori.Ad : "",
                    SaticiAd = t.Urun.Kullanici.KullaniciAdi,
                    t.TeklifTutari,
                    t.Aciklama,
                    Durum = t.Durum.ToString(),
                    DurumText = t.Durum.ToTurkce(),
                    t.OlusturmaTarihi,
                    t.YanitTarihi,
                    t.SaticiYaniti
                })
                .ToListAsync();

            return Ok(teklifler);
        }

        // 3. GELEN TEKLÝFLERÝ GETÝR (Satýcý - aldýðý teklifler)
        [HttpGet("gelen")]
        public async Task<ActionResult> GelenTeklifleriGetir()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var teklifler = await _context.Teklifler
                .Include(t => t.Urun)
                .Include(t => t.Alici)
                .Where(t => t.Urun.KullaniciId == userId)
                .OrderByDescending(t => t.OlusturmaTarihi)
                .Select(t => new
                {
                    t.Id,
                    UrunId = t.Urun.Id,
                    UrunAd = t.Urun.Ad,
                    UrunResim = t.Urun.ResimUrl,
                    UrunFiyat = t.Urun.Fiyat,
                    AliciAd = t.Alici.KullaniciAdi,
                    AliciEmail = t.Alici.Email,
                    t.TeklifTutari,
                    t.Aciklama,
                    Durum = t.Durum.ToString(),
                    DurumText = t.Durum.ToTurkce(),
                    t.OlusturmaTarihi,
                    t.YanitTarihi,
                    t.SaticiYaniti
                })
                .ToListAsync();

            return Ok(teklifler);
        }

        // 4. TEKLÝFÝ KABUL ET (Satýcý)
        [HttpPut("{id}/kabul")]
        public async Task<IActionResult> TeklifiKabulEt(int id, [FromBody] TeklifYanitDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var teklif = await _context.Teklifler
                    .Include(t => t.Urun)
                    .Include(t => t.Alici)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (teklif == null)
                    return NotFound("Teklif bulunamadý.");

                // Sadece ürünün sahibi kabul edebilir
                if (teklif.Urun.KullaniciId != userId)
                    return Forbid();

                // Sadece bekleyen teklifler kabul edilebilir
                if (teklif.Durum != TeklifDurumu.Bekliyor)
                    return BadRequest("Bu teklif zaten yanýtlanmýþ.");

                // Teklifi kabul et
                teklif.Durum = TeklifDurumu.KabulEdildi;
                teklif.YanitTarihi = DateTime.Now;
                teklif.SaticiYaniti = dto.Yanit ?? "Teklifinizi kabul ediyorum.";

                // ÖNEMLÝ: Ürün fiyatýný DEÐÝÞTÝRME! Sadece teklifi onayla.
                // Sepete eklerken özel fiyat kullanýlacak.

                await _context.SaveChangesAsync();

                // Alýcýya bildirim gönder
                await BildirimlerController.BildirimOlustur(
                    _context,
                    teklif.AliciId,
                    BildirimTipleri.Satis,
                    "Teklifiniz Kabul Edildi!",
                    $"{teklif.Urun.Ad} için {teklif.TeklifTutari:N2} TL teklifiniz kabul edildi. Sepetinize ekleyebilirsiniz!",
                    teklif.Id
                );

                Console.WriteLine($"[TEKLÝF] Satýcý {userId}, teklif #{id} kabul etti. Özel fiyat: {teklif.TeklifTutari:N2} TL (Ürün fiyatý deðiþmedi: {teklif.Urun.Fiyat:N2} TL)");

                return Ok(new { message = "Teklif kabul edildi. Alýcý sepetine özel fiyatla ekleyebilir." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Teklif kabul hatasý: {ex.Message}");
                return StatusCode(500, "Bir hata oluþtu.");
            }
        }

        // 5. TEKLÝFÝ REDDET (Satýcý)
        [HttpPut("{id}/reddet")]
        public async Task<IActionResult> TeklifiReddet(int id, [FromBody] TeklifYanitDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var teklif = await _context.Teklifler
                    .Include(t => t.Urun)
                    .Include(t => t.Alici)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (teklif == null)
                    return NotFound("Teklif bulunamadý.");

                if (teklif.Urun.KullaniciId != userId)
                    return Forbid();

                if (teklif.Durum != TeklifDurumu.Bekliyor)
                    return BadRequest("Bu teklif zaten yanýtlanmýþ.");

                // Teklifi reddet
                teklif.Durum = TeklifDurumu.Reddedildi;
                teklif.YanitTarihi = DateTime.Now;
                teklif.SaticiYaniti = dto.Yanit ?? "Teklifinizi reddediyorum.";

                await _context.SaveChangesAsync();

                // Alýcýya bildirim gönder
                await BildirimlerController.BildirimOlustur(
                    _context,
                    teklif.AliciId,
                    BildirimTipleri.Satis,
                    "Teklifiniz Reddedildi",
                    $"{teklif.Urun.Ad} için {teklif.TeklifTutari:N2} TL teklifiniz reddedildi.",
                    teklif.Id
                );

                Console.WriteLine($"[TEKLÝF] Satýcý {userId}, teklif #{id} reddetti.");

                return Ok(new { message = "Teklif reddedildi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Teklif reddetme hatasý: {ex.Message}");
                return StatusCode(500, "Bir hata oluþtu.");
            }
        }

        // 6. TEKLÝFÝ ÝPTAL ET (Alýcý)
        [HttpPut("{id}/iptal")]
        public async Task<IActionResult> TeklifiIptalEt(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var teklif = await _context.Teklifler
                    .FirstOrDefaultAsync(t => t.Id == id && t.AliciId == userId);

                if (teklif == null)
                    return NotFound("Teklif bulunamadý.");

                if (teklif.Durum != TeklifDurumu.Bekliyor)
                    return BadRequest("Sadece bekleyen teklifler iptal edilebilir.");

                teklif.Durum = TeklifDurumu.IptalEdildi;
                teklif.YanitTarihi = DateTime.Now;

                await _context.SaveChangesAsync();

                Console.WriteLine($"[TEKLÝF] Kullanýcý {userId}, teklif #{id} iptal etti.");

                return Ok(new { message = "Teklif iptal edildi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Teklif iptal hatasý: {ex.Message}");
                return StatusCode(500, "Bir hata oluþtu.");
            }
        }

        // 7. TEKLÝF DETAYI
        [HttpGet("{id}")]
        public async Task<ActionResult> TeklifDetay(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var teklif = await _context.Teklifler
                .Include(t => t.Urun)
                    .ThenInclude(u => u.Kullanici)
                .Include(t => t.Alici)
                .Where(t => t.Id == id && (t.AliciId == userId || t.Urun.KullaniciId == userId))
                .Select(t => new
                {
                    t.Id,
                    UrunId = t.Urun.Id,
                    UrunAd = t.Urun.Ad,
                    UrunResim = t.Urun.ResimUrl,
                    UrunFiyat = t.Urun.Fiyat,
                    SaticiAd = t.Urun.Kullanici.KullaniciAdi,
                    AliciAd = t.Alici.KullaniciAdi,
                    t.TeklifTutari,
                    t.Aciklama,
                    Durum = t.Durum.ToString(),
                    DurumText = t.Durum.ToTurkce(),
                    t.OlusturmaTarihi,
                    t.YanitTarihi,
                    t.SaticiYaniti
                })
                .FirstOrDefaultAsync();

            if (teklif == null)
                return NotFound("Teklif bulunamadý.");

            return Ok(teklif);
        }
    }

    // DTOs
    public class TeklifGonderDto
    {
        public int UrunId { get; set; }
        public decimal TeklifTutari { get; set; }
        public string? Aciklama { get; set; }
    }

    public class TeklifYanitDto
    {
        public string? Yanit { get; set; }
    }
}
