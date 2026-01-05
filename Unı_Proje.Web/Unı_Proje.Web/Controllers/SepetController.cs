using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Unı_Proje.Data;
using Unı_Proje.DTOs;
using Unı_Proje.Models;

namespace Unı_Proje.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SepetController : ControllerBase
    {
        private readonly ProjeDbContext _context;

        public SepetController(ProjeDbContext context)
        {
            _context = context;
        }

        // 1. SEPETİ GETİR (Giriş yapan kullanıcının - STOK BİLGİSİYLE)
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<SepetItemDto>>> GetSepet()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                Console.WriteLine($"[SEPET] Kullanıcı {userId} için sepet istendi");

                var sepet = await _context.SepetUrunleri
                    .Where(x => x.KullaniciId == userId)
                    .Include(x => x.Urun)
                    .ThenInclude(u => u.Kategori)
                    .Select(x => new SepetItemDto
                    {
                        Id = x.Id,
                        UrunId = x.Urun.Id,
                        UrunAd = x.Urun.Ad,
                        Fiyat = x.OzelFiyat ?? x.Urun.Fiyat, // Özel fiyat varsa onu kullan!
                        ResimUrl = x.Urun.ResimUrl,
                        KategoriAd = x.Urun.Kategori.Ad,
                        // 📦 Stok bilgisi
                        StokMiktari = x.Urun.StokMiktari,
                        StokVarMi = x.Urun.StokMiktari > 0
                    })
                    .ToListAsync();

                Console.WriteLine($"[SEPET] {sepet.Count} ürün bulundu");
                return Ok(sepet);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SEPET HATA] {ex.Message}");
                Console.WriteLine($"[SEPET STACK] {ex.StackTrace}");
                return StatusCode(500, new { 
                    message = "Sepet yüklenirken hata oluştu", 
                    error = ex.Message 
                });
            }
        }

        // 2. SEPETE EKLE (STOK KONTROLÜ İLE + TEKLİF DESTEĞİ)
        [HttpPost("ekle/{urunId}")]
        [Authorize]
        public async Task<IActionResult> SepeteEkle(int urunId, [FromQuery] int? teklifId = null)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                Console.WriteLine($"[SEPETE EKLE] Kullanıcı {userId}, Ürün {urunId}");

                // 📦 Ürünü ve stok bilgisini kontrol et
                var urun = await _context.Urunler.FindAsync(urunId);
                if (urun == null)
                {
                    Console.WriteLine($"[SEPETE EKLE] Ürün {urunId} bulunamadı");
                    return NotFound("Ürün bulunamadı.");
                }

                Console.WriteLine($"[SEPETE EKLE] Ürün bulundu: {urun.Ad}, Stok: {urun.StokMiktari}, Satıcı: {urun.KullaniciId}");

                // 🚫 KENDİ ÜRÜNÜNÜ SATIN ALAMAZ!
                if (urun.KullaniciId == userId)
                {
                    Console.WriteLine($"[SEPETE EKLE] Kullanıcı kendi ürününü sepete eklemeye çalışıyor!");
                    return BadRequest("Kendi ürününüzü satın alamazsınız.");
                }

                if (urun.StokMiktari <= 0)
                {
                    Console.WriteLine($"[SEPETE EKLE] Ürün stokta yok");
                    return BadRequest("Bu ürün şu anda stokta yok.");
                }

                // Zaten sepette var mı kontrolü
                var varMi = await _context.SepetUrunleri
                    .AnyAsync(x => x.KullaniciId == userId && x.UrunId == urunId);

                if (varMi)
                {
                    Console.WriteLine($"[SEPETE EKLE] Ürün zaten sepette");
                    return BadRequest("Bu ürün zaten sepetinizde.");
                }

                // Teklif kontrolü - eğer teklif ID varsa, özel fiyat kullan
                decimal? ozelFiyat = null;
                if (teklifId.HasValue)
                {
                    var teklif = await _context.Teklifler
                        .FirstOrDefaultAsync(t => t.Id == teklifId.Value && 
                                                  t.AliciId == userId && 
                                                  t.UrunId == urunId && 
                                                  t.Durum == TeklifDurumu.KabulEdildi);
                    
                    if (teklif != null)
                    {
                        ozelFiyat = teklif.TeklifTutari;
                        Console.WriteLine($"[SEPETE EKLE] Teklif bulundu! Özel fiyat: {ozelFiyat:N2} ₺");
                    }
                }

                var yeniItem = new SepetUrun
                {
                    KullaniciId = userId,
                    UrunId = urunId,
                    OzelFiyat = ozelFiyat,
                    TeklifId = teklifId
                };

                Console.WriteLine($"[SEPETE EKLE] Yeni item oluşturuldu: UserId={userId}, UrunId={urunId}, OzelFiyat={ozelFiyat}, TeklifId={teklifId}");

                _context.SepetUrunleri.Add(yeniItem);
                Console.WriteLine($"[SEPETE EKLE] Context'e eklendi, şimdi kaydediliyor...");
                
                try
                {
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"[SEPETE EKLE] Başarılı! Sepet ID: {yeniItem.Id}");
                }
                catch (DbUpdateException dbEx)
                {
                    Console.WriteLine($"[SEPETE EKLE DB HATA] {dbEx.Message}");
                    Console.WriteLine($"[SEPETE EKLE DB INNER] {dbEx.InnerException?.Message}");
                    Console.WriteLine($"[SEPETE EKLE DB STACK] {dbEx.StackTrace}");
                    
                    return StatusCode(500, new { 
                        message = "Veritabanı hatası", 
                        error = dbEx.InnerException?.Message ?? dbEx.Message
                    });
                }

                return Ok(new { message = "Ürün sepete eklendi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SEPETE EKLE HATA] {ex.Message}");
                Console.WriteLine($"[SEPETE EKLE STACK] {ex.StackTrace}");
                return StatusCode(500, new { 
                    message = "Sepete eklenirken hata oluştu", 
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        // 3. SEPETTEN ÇIKAR
        [HttpDelete("sil/{id}")]
        [Authorize]
        public async Task<IActionResult> SepettenCikar(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var item = await _context.SepetUrunleri
                .FirstOrDefaultAsync(x => x.Id == id && x.KullaniciId == userId);

            if (item == null) return NotFound("Ürün bulunamadı.");

            _context.SepetUrunleri.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ürün sepetten çıkarıldı." });
        }

        // 4. SATIN AL (Sipariş Oluştur - STOK KONTROLÜ VE DÜŞÜRME)
        [HttpPost("satinal")]
        [Authorize]
        public async Task<IActionResult> SatinAl([FromBody] SiparisOlusturDto? dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Sepetteki ürünleri çek
            var sepetUrunleri = await _context.SepetUrunleri
                .Where(x => x.KullaniciId == userId)
                .Include(x => x.Urun)
                .ToListAsync();

            if (!sepetUrunleri.Any()) 
                return BadRequest("Sepetiniz boş.");

            // 📦 STOK KONTROLÜ - Tüm ürünlerin stoğu yeterli mi?
            var stokHatalari = new List<string>();
            foreach (var sepetItem in sepetUrunleri)
            {
                if (sepetItem.Urun.StokMiktari <= 0)
                {
                    stokHatalari.Add($"'{sepetItem.Urun.Ad}' ürünü stokta kalmamış.");
                }
            }

            if (stokHatalari.Any())
            {
                return BadRequest(new 
                { 
                    message = "Bazı ürünler stokta yok:", 
                    hatalar = stokHatalari 
                });
            }

            // Toplam tutarı hesapla (özel fiyat varsa onu kullan)
            var toplamTutar = sepetUrunleri.Sum(x => x.OzelFiyat ?? x.Urun.Fiyat);

            // Yeni sipariş oluştur
            var yeniSiparis = new Siparis
            {
                KullaniciId = userId,
                SiparisTarihi = DateTime.Now,
                ToplamTutar = toplamTutar,
                Durum = SiparisDurumu.Onaylandi,
                TeslimatAdresi = dto?.TeslimatAdresi,
                TeslimatTelefonu = dto?.TeslimatTelefonu
            };

            _context.Siparisler.Add(yeniSiparis);
            await _context.SaveChangesAsync();

            // Sipariş detaylarını oluştur VE STOKTAN DÜŞ
            foreach (var sepetItem in sepetUrunleri)
            {
                // Sipariş detayı ekle (özel fiyat varsa onu kullan)
                var detay = new SiparisDetay
                {
                    SiparisId = yeniSiparis.Id,
                    UrunId = sepetItem.UrunId,
                    UrunAdi = sepetItem.Urun.Ad,
                    BirimFiyat = sepetItem.OzelFiyat ?? sepetItem.Urun.Fiyat, // Özel fiyat!
                    Adet = 1,
                    ResimUrl = sepetItem.Urun.ResimUrl
                };
                _context.SiparisDetaylari.Add(detay);

                // 📦 STOKTAN DÜŞ
                sepetItem.Urun.StokMiktari -= 1;
                
                // Stok 0'ın altına düşmesin (güvenlik kontrolü)
                if (sepetItem.Urun.StokMiktari < 0)
                    sepetItem.Urun.StokMiktari = 0;
            }

            // Sepeti temizle
            _context.SepetUrunleri.RemoveRange(sepetUrunleri);
            
            await _context.SaveChangesAsync();

            return Ok(new { 
                message = "Siparişiniz başarıyla alındı! Ürünler stoktan düşürüldü.", 
                siparisId = yeniSiparis.Id 
            });
        }
    }
}