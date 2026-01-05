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
    public class FavorilerController : ControllerBase
    {
        private readonly ProjeDbContext _context;

        public FavorilerController(ProjeDbContext context)
        {
            _context = context;
        }

        // 1. FAVORÝLERÝ GETÝR (Giriþ yapan kullanýcýnýn)
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetFavoriler()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var favoriler = await _context.Favoriler
                    .Where(x => x.KullaniciId == userId)
                    .Include(x => x.Urun)
                    .ThenInclude(u => u.Kategori)
                    .Select(x => new
                    {
                        Id = x.Id,
                        UrunId = x.Urun.Id,
                        Ad = x.Urun.Ad,
                        Fiyat = x.Urun.Fiyat,
                        ResimUrl = x.Urun.ResimUrl,
                        Kategori = new
                        {
                            Id = x.Urun.Kategori.Id,
                            Ad = x.Urun.Kategori.Ad
                        },
                        EklemeTarihi = x.EklemeTarihi
                    })
                    .ToListAsync();

                return Ok(favoriler);
            }
            catch (Exception ex)
            {
                // Favoriler tablosu yoksa boþ liste dön
                Console.WriteLine($"Favori getirme hatasý: {ex.Message}");
                return Ok(new List<object>());
            }
        }

        // 2. FAVORÝLERE EKLE
        [HttpPost("ekle/{urunId}")]
        [Authorize]
        public async Task<IActionResult> FavorilereEkle(int urunId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Zaten favorilerde var mý kontrolü
                var varMi = await _context.Favoriler
                    .AnyAsync(x => x.KullaniciId == userId && x.UrunId == urunId);

                if (varMi) return BadRequest("Bu ürün zaten favorilerinizde.");

                var yeniFavori = new Favori
                {
                    KullaniciId = userId,
                    UrunId = urunId,
                    EklemeTarihi = DateTime.Now
                };

                _context.Favoriler.Add(yeniFavori);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Ürün favorilere eklendi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Favori ekleme hatasý: {ex.Message}");
                return StatusCode(500, "Favorilere eklenirken bir hata oluþtu. Lütfen daha sonra tekrar deneyin.");
            }
        }

        // 3. FAVORÝLERDEN ÇIKAR
        [HttpDelete("sil/{urunId}")]
        [Authorize]
        public async Task<IActionResult> FavorilerdenCikar(int urunId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var favori = await _context.Favoriler
                    .FirstOrDefaultAsync(x => x.UrunId == urunId && x.KullaniciId == userId);

                if (favori == null) return NotFound("Ürün favorilerde bulunamadý.");

                _context.Favoriler.Remove(favori);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Ürün favorilerden çýkarýldý." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Favori çýkarma hatasý: {ex.Message}");
                return StatusCode(500, "Favorilerden çýkarýlýrken bir hata oluþtu.");
            }
        }

        // 4. ÜRÜN FAVORÝDE MÝ KONTROL ET
        [HttpGet("kontrol/{urunId}")]
        [Authorize]
        public async Task<ActionResult<bool>> FavoriMi(int urunId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var varMi = await _context.Favoriler
                    .AnyAsync(x => x.KullaniciId == userId && x.UrunId == urunId);

                return Ok(varMi);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Favori kontrol hatasý: {ex.Message}");
                return Ok(false); // Hata durumunda false dön
            }
        }
    }
}
