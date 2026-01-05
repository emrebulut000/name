using Microsoft.AspNetCore.Mvc;

namespace Uný_Proje.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KategorilerController : ControllerBase
    {
        // Geçici olarak sabit kategoriler döndürelim
        // Daha sonra veritabanýndan çekebilirsiniz
        [HttpGet]
        public IActionResult KategorileriListele()
        {
            var kategoriler = new[]
            {
                new { Id = 1, Ad = "Elektronik" },
                new { Id = 2, Ad = "Mobilya" },
                new { Id = 3, Ad = "Giyim" },
                new { Id = 4, Ad = "Kitap" },
                new { Id = 5, Ad = "Spor" }
            };
            
            return Ok(kategoriler);
        }
    }
}
