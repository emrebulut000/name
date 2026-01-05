// Gerekli kütüphaneler ve diğer sınıflar
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Unı_Proje.Data;
using Unı_Proje.Models;
// ----- YENİ EKLENEN KÜTÜPHANELER (JWT İÇİN) -----
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    // Veritabanı ve Konfigürasyon nesnelerimiz
    private readonly ProjeDbContext _context;
    private readonly IConfiguration _configuration; // appsettings.json dosyasını okumak için

    // ----- GÜNCELLENEN CONSTRUCTOR -----
    // .NET, IConfiguration servisini de otomatik olarak buraya enjekte edecek.
    public AuthController(ProjeDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration; // Değeri atıyoruz
    }

    // Register Fonksiyonu (Değişiklik yok)
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] KullaniciKayitDto kullaniciKayitDto)
    {
        if (await _context.Kullanicilar.AnyAsync(k => k.Email == kullaniciKayitDto.Email))
        {
            return BadRequest("Bu e-posta adresi zaten kullanılıyor.");
        }
        if (await _context.Kullanicilar.AnyAsync(k => k.KullaniciAdi == kullaniciKayitDto.KullaniciAdi))
        {
            return BadRequest("Bu kullanıcı adı zaten alınmış.");
        }

        string sifreHash = BCrypt.Net.BCrypt.HashPassword(kullaniciKayitDto.Sifre);

        var yeniKullanici = new Kullanici
        {
            KullaniciAdi = kullaniciKayitDto.KullaniciAdi,
            Email = kullaniciKayitDto.Email,
            SifreHash = sifreHash,
            KayitTarihi = DateTime.UtcNow
        };

        await _context.Kullanicilar.AddAsync(yeniKullanici);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Kullanıcı kaydı başarıyla tamamlandı." });
    }

    // ----- GÜNCELLENEN LOGIN FONKSİYONU -----
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] KullaniciGirisDto girisDto)
    {
        var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == girisDto.Email);

        if (kullanici == null || !BCrypt.Net.BCrypt.Verify(girisDto.Sifre, kullanici.SifreHash))
        {
            // Kullanıcı bulunamazsa VEYA şifre yanlışsa, aynı hatayı döndür.
            return Unauthorized("Hatalı kimlik bilgileri.");
        }

        // --- BAŞARI DURUMU: KULLANICIYA TOKEN OLUŞTUR VE DÖNDÜR ---
        string token = CreateToken(kullanici);

        return Ok(new { token }); // Cevap olarak { "token": "..." } formatında token'ı döndür.
    }

    // ********************************************************************
    // YENİ EKLENEN TOKEN OLUŞTURMA METODU
    // ********************************************************************
    private string CreateToken(Kullanici kullanici)
    {
        // Token'ın "payload" kısmında yani içinde taşıyacağı bilgileri (claims) tanımlıyoruz.
        List<Claim> claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()), // Kullanıcının ID'si
            new Claim(ClaimTypes.Name, kullanici.KullaniciAdi),           // Kullanıcının adı
            new Claim(ClaimTypes.Email, kullanici.Email)                  // Kullanıcının e-postası
        };

        // appsettings.json dosyasındaki gizli anahtarımızı (Jwt:Key) alıyoruz.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetSection("Jwt:Key").Value!));

        // Bu anahtarı kullanarak token'ı imzalayacağımız kimlik bilgilerini oluşturuyoruz.
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        // Token'ın tanımını yapıyoruz:
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),       // İçinde hangi bilgileri taşıyacağı
            Expires = DateTime.Now.AddDays(100),          // Ne kadar süre geçerli olacağı (Örn: 1 gün)
            SigningCredentials = creds,                 // Hangi anahtar ve algoritmayla imzalanacağı
            Issuer = _configuration.GetSection("Jwt:Issuer").Value,     // Kimin verdiği (appsettings.json'dan)
            Audience = _configuration.GetSection("Jwt:Audience").Value  // Kimin için verildiği (appsettings.json'dan)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Oluşturulan token'ı metin formatında geri döndürüyoruz.
        return tokenHandler.WriteToken(token);
    }
}