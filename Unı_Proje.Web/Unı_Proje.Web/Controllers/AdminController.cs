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
    [Authorize] // Sadece giriþ yapmýþ kullanýcýlar
    public class AdminController : ControllerBase
    {
        private readonly ProjeDbContext _context;

        public AdminController(ProjeDbContext context)
        {
            _context = context;
        }

        // 1. TÜM KULLANICILARI GETÝR (Sadece Admin)
        [HttpGet("kullanicilar")]
        public async Task<ActionResult> GetKullanicilar()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var aktifKullanici = await _context.Kullanicilar.FindAsync(userId);

                // Admin kontrolü
                if (aktifKullanici == null || !aktifKullanici.IsAdmin)
                {
                    return StatusCode(403, "Bu iþlem için admin yetkisi gereklidir.");
                }

                var kullanicilar = await _context.Kullanicilar
                    .Select(k => new
                    {
                        Id = k.Id,
                        KullaniciAdi = k.KullaniciAdi,
                        Email = k.Email,
                        ProfilResmiUrl = k.ProfilResmiUrl,
                        Telefon = k.Telefon,
                        Bio = k.Bio,
                        KayitTarihi = k.KayitTarihi,
                        IsAdmin = k.IsAdmin
                    })
                    .OrderByDescending(k => k.KayitTarihi)
                    .ToListAsync();

                return Ok(kullanicilar);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kullanýcýlar getirme hatasý: {ex.Message}");
                return StatusCode(500, "Bir hata oluþtu.");
            }
        }

        // 2. ÝSTATÝSTÝKLER (Dashboard)
        [HttpGet("istatistikler")]
        public async Task<ActionResult> GetIstatistikler()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var aktifKullanici = await _context.Kullanicilar.FindAsync(userId);

                if (aktifKullanici == null || !aktifKullanici.IsAdmin)
                {
                    return StatusCode(403, "Bu iþlem için admin yetkisi gereklidir.");
                }

                var toplamKullanici = await _context.Kullanicilar.CountAsync();
                var toplamUrun = await _context.Urunler.CountAsync();
                var toplamMesaj = await _context.Mesajlar.CountAsync();
                var toplamFavori = await _context.Favoriler.CountAsync();

                // Son 7 gün içinde kayýt olan kullanýcýlar
                var yeniKullanicilar = await _context.Kullanicilar
                    .Where(k => k.KayitTarihi >= DateTime.Now.AddDays(-7))
                    .CountAsync();

                // Bugün eklenen ürünler
                var bugunEklenenUrunler = await _context.Urunler
                    .Where(u => u.EklemeTarihi.Date == DateTime.Today)
                    .CountAsync();

                return Ok(new
                {
                    ToplamKullanici = toplamKullanici,
                    ToplamUrun = toplamUrun,
                    ToplamMesaj = toplamMesaj,
                    ToplamFavori = toplamFavori,
                    YeniKullanicilar = yeniKullanicilar,
                    BugunEklenenUrunler = bugunEklenenUrunler
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ýstatistikler hatasý: {ex.Message}");
                return StatusCode(500, "Bir hata oluþtu.");
            }
        }

        // 3. KULLANICI DETAYLARI
        [HttpGet("kullanici/{id}")]
        public async Task<ActionResult> GetKullaniciDetay(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var aktifKullanici = await _context.Kullanicilar.FindAsync(userId);

                if (aktifKullanici == null || !aktifKullanici.IsAdmin)
                {
                    return StatusCode(403, "Bu iþlem için admin yetkisi gereklidir.");
                }

                var kullanici = await _context.Kullanicilar
                    .Where(k => k.Id == id)
                    .Select(k => new
                    {
                        Id = k.Id,
                        KullaniciAdi = k.KullaniciAdi,
                        Email = k.Email,
                        ProfilResmiUrl = k.ProfilResmiUrl,
                        Telefon = k.Telefon,
                        Bio = k.Bio,
                        KayitTarihi = k.KayitTarihi,
                        IsAdmin = k.IsAdmin
                    })
                    .FirstOrDefaultAsync();

                if (kullanici == null)
                {
                    return NotFound("Kullanýcý bulunamadý.");
                }

                // Kullanýcýnýn istatistikleri
                var urunSayisi = await _context.Urunler.CountAsync(u => u.KullaniciId == id);
                var mesajSayisi = await _context.Mesajlar.CountAsync(m => m.GonderenId == id || m.AliciId == id);
                var favoriSayisi = await _context.Favoriler.CountAsync(f => f.KullaniciId == id);

                return Ok(new
                {
                    Kullanici = kullanici,
                    Istatistikler = new
                    {
                        UrunSayisi = urunSayisi,
                        MesajSayisi = mesajSayisi,
                        FavoriSayisi = favoriSayisi
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kullanýcý detay hatasý: {ex.Message}");
                return StatusCode(500, "Bir hata oluþtu.");
            }
        }

        // 4. KULLANICI SÝL
        [HttpDelete("kullanici/{id}")]
        public async Task<ActionResult> KullaniciSil(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var aktifKullanici = await _context.Kullanicilar.FindAsync(userId);

                if (aktifKullanici == null || !aktifKullanici.IsAdmin)
                {
                    return StatusCode(403, "Bu iþlem için admin yetkisi gereklidir.");
                }

                // Kendini silemez
                if (userId == id)
                {
                    return BadRequest("Kendi hesabýnýzý silemezsiniz.");
                }

                var kullanici = await _context.Kullanicilar.FindAsync(id);
                if (kullanici == null)
                {
                    return NotFound("Kullanýcý bulunamadý.");
                }

                // ?? GÜVENLÝK KONTROLÜ: Silinecek kullanýcý adminse
                if (kullanici.IsAdmin)
                {
                    var toplamAdminSayisi = await _context.Kullanicilar
                        .CountAsync(k => k.IsAdmin);

                    if (toplamAdminSayisi <= 1)
                    {
                        return BadRequest(new 
                        { 
                            message = "?? Sistemde en az 1 admin bulunmalýdýr. Son admin silinemez!" 
                        });
                    }
                }

                // Ýliþkili verileri sil (Cascade delete yoksa)
                var urunler = await _context.Urunler.Where(u => u.KullaniciId == id).ToListAsync();
                _context.Urunler.RemoveRange(urunler);

                var mesajlar = await _context.Mesajlar.Where(m => m.GonderenId == id || m.AliciId == id).ToListAsync();
                _context.Mesajlar.RemoveRange(mesajlar);

                var favoriler = await _context.Favoriler.Where(f => f.KullaniciId == id).ToListAsync();
                _context.Favoriler.RemoveRange(favoriler);

                var sepetler = await _context.SepetUrunleri.Where(s => s.KullaniciId == id).ToListAsync();
                _context.SepetUrunleri.RemoveRange(sepetler);

                _context.Kullanicilar.Remove(kullanici);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Kullanýcý ve iliþkili tüm veriler baþarýyla silindi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kullanýcý silme hatasý: {ex.Message}");
                return StatusCode(500, "Bir hata oluþtu.");
            }
        }

        // 5. ADMÝN YETKÝSÝ VER/KALDIR
        [HttpPut("kullanici/{id}/admin")]
        public async Task<ActionResult> AdminYetkisiDegistir(int id, [FromBody] AdminYetkiDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var aktifKullanici = await _context.Kullanicilar.FindAsync(userId);

                if (aktifKullanici == null || !aktifKullanici.IsAdmin)
                {
                    return StatusCode(403, "Bu iþlem için admin yetkisi gereklidir.");
                }

                var kullanici = await _context.Kullanicilar.FindAsync(id);
                if (kullanici == null)
                {
                    return NotFound("Kullanýcý bulunamadý.");
                }

                // ?? GÜVENLÝK KONTROLÜ: Admin yetkisi kaldýrýlýyorsa
                if (!dto.IsAdmin && kullanici.IsAdmin)
                {
                    // Sistemdeki toplam admin sayýsýný kontrol et
                    var toplamAdminSayisi = await _context.Kullanicilar
                        .CountAsync(k => k.IsAdmin);

                    if (toplamAdminSayisi <= 1)
                    {
                        return BadRequest(new 
                        { 
                            message = "?? Sistemde en az 1 admin bulunmalýdýr. Son admin yetkisi kaldýrýlamaz!" 
                        });
                    }
                }

                // Kendine admin yetkisi veremez/kaldýramaz (opsiyonel kontrol)
                if (userId == id)
                {
                    return BadRequest(new 
                    { 
                        message = "?? Kendi admin yetkinizi deðiþtiremezsiniz." 
                    });
                }

                kullanici.IsAdmin = dto.IsAdmin;
                await _context.SaveChangesAsync();

                return Ok(new { message = $"? Kullanýcý admin yetkisi {(dto.IsAdmin ? "verildi" : "kaldýrýldý")}." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Admin yetkisi deðiþtirme hatasý: {ex.Message}");
                return StatusCode(500, "Bir hata oluþtu.");
            }
        }

        // 6. AKTÝF KULLANICI ADMÝN MÝ?
        [HttpGet("admin-mi")]
        public async Task<ActionResult<bool>> AdminMi()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var kullanici = await _context.Kullanicilar.FindAsync(userId);

                return Ok(kullanici?.IsAdmin ?? false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Admin kontrolü hatasý: {ex.Message}");
                return Ok(false);
            }
        }

        // 7. SÝPARÝÞ DURUMU GÜNCELLE (+ BÝLDÝRÝM GÖNDER)
        [HttpPut("siparis/{siparisId}/durum")]
        public async Task<ActionResult> SiparisDurumGuncelle(int siparisId, [FromBody] SiparisDurumDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var aktifKullanici = await _context.Kullanicilar.FindAsync(userId);

                if (aktifKullanici == null || !aktifKullanici.IsAdmin)
                {
                    return StatusCode(403, "Bu iþlem için admin yetkisi gereklidir.");
                }

                var siparis = await _context.Siparisler.FindAsync(siparisId);
                if (siparis == null)
                {
                    return NotFound("Sipariþ bulunamadý.");
                }

                var eskiDurum = siparis.Durum;
                siparis.Durum = dto.YeniDurum;
                await _context.SaveChangesAsync();

                // ?? BÝLDÝRÝM GÖNDER
                string bildirimBaslik = $"Sipariþ #{siparisId} Durumu Güncellendi";
                string bildirimMesaj = dto.YeniDurum switch
                {
                    SiparisDurumu.Onaylandi => "Sipariþiniz onaylandý ve hazýrlanýyor.",
                    SiparisDurumu.Hazirlaniyor => "Sipariþiniz hazýrlanýyor.",
                    SiparisDurumu.Kargoya_Verildi => "Sipariþiniz kargoya verildi. Kýsa sürede elinizde olacak!",
                    SiparisDurumu.Teslim_Edildi => "Sipariþiniz teslim edildi. Afiyet olsun! ??",
                    SiparisDurumu.Iptal_Edildi => "Sipariþiniz iptal edildi.",
                    _ => $"Sipariþ durumunuz '{dto.YeniDurum}' olarak güncellendi."
                };

                // Bildirim oluþtur (Port 7130'daki BildirimlerController kullanýlamaz, direkt ekleyelim)
                var bildirim = new Bildirim
                {
                    KullaniciId = siparis.KullaniciId,
                    Tip = BildirimTipleri.Siparis,
                    Baslik = bildirimBaslik,
                    Mesaj = bildirimMesaj,
                    IliskiliId = siparisId,
                    OlusturmaTarihi = DateTime.Now
                };

                _context.Bildirimler.Add(bildirim);
                await _context.SaveChangesAsync();

                Console.WriteLine($"[BÝLDÝRÝM] Sipariþ #{siparisId} durum deðiþikliði bildirimi gönderildi: {eskiDurum} -> {dto.YeniDurum}");

                return Ok(new { message = "Sipariþ durumu güncellendi ve müþteriye bildirim gönderildi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sipariþ durum güncelleme hatasý: {ex.Message}");
                return StatusCode(500, "Bir hata oluþtu.");
            }
        }
    }

    public class AdminYetkiDto
    {
        public bool IsAdmin { get; set; }
    }

    public class SiparisDurumDto
    {
        public SiparisDurumu YeniDurum { get; set; }
    }
}
