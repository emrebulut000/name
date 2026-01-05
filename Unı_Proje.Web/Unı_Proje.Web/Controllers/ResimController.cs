using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Unı_Proje.Web.Controllers
{
    // DİKKAT: Adresi değiştirdik -> "api/resim"
    [Route("api/resim")]
    [ApiController]
    public class ResimController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public ResimController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("")]
        public async Task<IActionResult> Yukle(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Dosya seçilmedi.");

            // Dosya adı (her iki proje için aynı olacak)
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // 1. Blazor projesine kaydet (7000)
            string uploadsFolder1 = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder1))
            {
                Directory.CreateDirectory(uploadsFolder1);
            }
            string filePath1 = Path.Combine(uploadsFolder1, uniqueFileName);

            // 2. Eski API projesine de kaydet (7130)
            // Workspace root'a göre relative path
            string workspaceRoot = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "..", "..", "Unı_Proje"));
            string uploadsFolder2 = Path.Combine(workspaceRoot, "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder2))
            {
                Directory.CreateDirectory(uploadsFolder2);
            }
            string filePath2 = Path.Combine(uploadsFolder2, uniqueFileName);

            // Dosyayı kaydet (stream'i iki kez kullanabilmek için byte array'e çeviriyoruz)
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            // Her iki konuma da yaz
            await System.IO.File.WriteAllBytesAsync(filePath1, fileBytes);
            await System.IO.File.WriteAllBytesAsync(filePath2, fileBytes);

            // URL Oluştur - 7130 portuna dönüyoruz (Ürünler API'si burada)
            string url = $"https://localhost:7130/uploads/{uniqueFileName}";

            // Frontend "Url" beklediği için büyük harfle dönüyoruz
            return Ok(new { Url = url });
        }
    }
}