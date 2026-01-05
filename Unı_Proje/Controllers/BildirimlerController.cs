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
    public class BildirimlerController : ControllerBase
    {
        private readonly ProjeDbContext _context;

        public BildirimlerController(ProjeDbContext context)
        {
            _context = context;
        }

        // 1. KULLANICININ TÜM BÝLDÝRÝMLERÝNÝ GETÝR
        [HttpGet]
        public async Task<ActionResult> GetBildirimler()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var bildirimler = await _context.Bildirimler
                .Where(b => b.KullaniciId == userId)
                .OrderByDescending(b => b.OlusturmaTarihi)
                .Take(50) // Son 50 bildirim
                .Select(b => new
                {
                    b.Id,
                    b.Tip,
                    b.Baslik,
                    b.Mesaj,
                    b.IliskiliId,
                    b.Okundu,
                    OlusturmaTarihi = b.OlusturmaTarihi
                })
                .ToListAsync();

            return Ok(bildirimler);
        }

        // 2. OKUNMAMIÞ BÝLDÝRÝM SAYISI
        [HttpGet("okunmamis-sayisi")]
        public async Task<ActionResult<int>> GetOkunmamisSayisi()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var sayi = await _context.Bildirimler
                .Where(b => b.KullaniciId == userId && !b.Okundu)
                .CountAsync();

            return Ok(sayi);
        }

        // 3. BÝLDÝRÝMÝ OKUNDU OLARAK ÝÞARETLE
        [HttpPut("{id}/okundu")]
        public async Task<IActionResult> OkunduIsaretle(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var bildirim = await _context.Bildirimler
                .FirstOrDefaultAsync(b => b.Id == id && b.KullaniciId == userId);

            if (bildirim == null)
                return NotFound();

            bildirim.Okundu = true;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // 4. TÜM BÝLDÝRÝMLERÝ OKUNDU OLARAK ÝÞARETLE
        [HttpPut("hepsini-okundu")]
        public async Task<IActionResult> HepsiniOkunduIsaretle()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _context.Bildirimler
                .Where(b => b.KullaniciId == userId && !b.Okundu)
                .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.Okundu, true));

            return Ok(new { message = "Tüm bildirimler okundu olarak iþaretlendi." });
        }

        // 5. BÝLDÝRÝMÝ SÝL
        [HttpDelete("{id}")]
        public async Task<IActionResult> BildirimSil(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var bildirim = await _context.Bildirimler
                .FirstOrDefaultAsync(b => b.Id == id && b.KullaniciId == userId);

            if (bildirim == null)
                return NotFound();

            _context.Bildirimler.Remove(bildirim);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bildirim silindi." });
        }

        // 6. TÜM BÝLDÝRÝMLERÝ SÝL
        [HttpDelete("hepsini-sil")]
        public async Task<IActionResult> HepsinSil()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _context.Bildirimler
                .Where(b => b.KullaniciId == userId)
                .ExecuteDeleteAsync();

            return Ok(new { message = "Tüm bildirimler silindi." });
        }

        // 7. BÝLDÝRÝM OLUÞTUR (Ýç kullaným için - diðer controller'lardan çaðrýlacak)
        public static async Task BildirimOlustur(
            ProjeDbContext context,
            int kullaniciId,
            string tip,
            string baslik,
            string mesaj,
            int? iliskiliId = null)
        {
            var bildirim = new Bildirim
            {
                KullaniciId = kullaniciId,
                Tip = tip,
                Baslik = baslik,
                Mesaj = mesaj,
                IliskiliId = iliskiliId,
                OlusturmaTarihi = DateTime.Now
            };

            context.Bildirimler.Add(bildirim);
            await context.SaveChangesAsync();

            Console.WriteLine($"[BÝLDÝRÝM] Kullanýcý {kullaniciId} için '{tip}' bildirimi oluþturuldu: {baslik}");
        }
    }
}
