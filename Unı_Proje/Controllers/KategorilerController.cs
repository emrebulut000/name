using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uný_Proje.Data;

namespace Uný_Proje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KategorilerController : ControllerBase
    {
        private readonly ProjeDbContext _context;

        public KategorilerController(ProjeDbContext context)
        {
            _context = context;
        }

        // GET: api/kategoriler
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetKategoriler()
        {
            try
            {
                var kategoriler = await _context.Kategoriler
                    .OrderBy(k => k.Ad)
                    .Select(k => new
                    {
                        k.Id,
                        k.Ad
                    })
                    .ToListAsync();

                Console.WriteLine($"[KATEGORÝLER] {kategoriler.Count} kategori döndürülüyor");
                return Ok(kategoriler);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[KATEGORÝLER HATA] {ex.Message}");
                return StatusCode(500, $"Hata: {ex.Message}");
            }
        }

        // GET: api/kategoriler/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetKategori(int id)
        {
            var kategori = await _context.Kategoriler.FindAsync(id);

            if (kategori == null)
            {
                return NotFound();
            }

            return Ok(new { kategori.Id, kategori.Ad });
        }
    }
}
