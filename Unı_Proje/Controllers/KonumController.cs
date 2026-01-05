using Microsoft.AspNetCore.Mvc;
using Uný_Proje.Data;

namespace Uný_Proje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KonumController : ControllerBase
    {
        // Tüm illeri getir
        [HttpGet("iller")]
        public ActionResult<List<string>> GetIller()
        {
            return Ok(TurkiyeIlIlce.Iller);
        }

        // Belirli bir ilin ilçelerini getir
        [HttpGet("ilceler/{il}")]
        public ActionResult<List<string>> GetIlceler(string il)
        {
            var ilceler = TurkiyeIlIlce.GetIlceler(il);
            
            if (!ilceler.Any())
            {
                return NotFound($"'{il}' iline ait ilçe bulunamadý.");
            }

            return Ok(ilceler);
        }
    }
}
