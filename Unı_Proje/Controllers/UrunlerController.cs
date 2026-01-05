using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Unı_Proje.Data;
using Unı_Proje.Models;
using Unı_Proje.DTOs;
using Unı_Proje.Helpers;

namespace Unı_Proje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrunlerController : ControllerBase
    {
        private readonly ProjeDbContext _context;
        private readonly Unı_Proje.Services.GeocodingService _geocodingService;

        public UrunlerController(ProjeDbContext context, Unı_Proje.Services.GeocodingService geocodingService)
        {
            _context = context;
            _geocodingService = geocodingService;
        }

        // 1. TÜM ÜRÜNLERİ GETİR (Ana Sayfa İçin - STOK BİLGİSİYLE + SAYFALAMA + SIRALAMA + MESAFE FİLTRESİ)
        [HttpGet]
        public async Task<ActionResult<object>> GetUrunler(
            [FromQuery] string? konum,
            [FromQuery] int sayfa = 1,
            [FromQuery] int sayfaBoyutu = 8,
            [FromQuery] string? siralama = "yeni",
            [FromQuery] double? kullaniciLatitude = null,
            [FromQuery] double? kullaniciLongitude = null,
            [FromQuery] int? maxMesafe = null) // km cinsinden
        {
            Console.WriteLine($"[URUNLER API] GetUrunler çağrıldı. Sayfa: {sayfa}, Boyut: {sayfaBoyutu}, Konum: {konum ?? "YOK"}, Sıralama: {siralama}, Mesafe: {maxMesafe?.ToString() ?? "YOK"}");

            var query = _context.Urunler
                                 .Include(u => u.Kategori)
                                 .AsQueryable();

            // Şehir/il bazlı filtreleme
            if (!string.IsNullOrWhiteSpace(konum))
            {
                var lowered = konum.Trim().ToLower();
                Console.WriteLine($"[URUNLER API] Konum filtresi uygulanıyor: {lowered}");
                query = query.Where(u => u.Konum != null && u.Konum.ToLower().Contains(lowered));
            }

            // 🌍 Mesafe bazlı filtreleme
            List<int>? mesafeIcindeUrunIdler = null;
            if (kullaniciLatitude.HasValue && kullaniciLongitude.HasValue && maxMesafe.HasValue)
            {
                Console.WriteLine($"[MESAFE FİLTRE] ========================================");
                Console.WriteLine($"[MESAFE FİLTRE] Kullanıcı konumu: Lat={kullaniciLatitude}, Lon={kullaniciLongitude}");
                Console.WriteLine($"[MESAFE FİLTRE] Max mesafe: {maxMesafe}km");
                
                // Koordinatları olan tüm ürünleri al
                var koordinatliUrunler = await _context.Urunler
                    .Where(u => u.Latitude.HasValue && u.Longitude.HasValue)
                    .Select(u => new { u.Id, u.Ad, u.Konum, u.Latitude, u.Longitude })
                    .ToListAsync();

                Console.WriteLine($"[MESAFE FİLTRE] Veritabanında koordinatlı ürün sayısı: {koordinatliUrunler.Count}");

                if (koordinatliUrunler.Count == 0)
                {
                    Console.WriteLine($"[MESAFE FİLTRE] ⚠️ UYARI: Hiçbir ürünün koordinatı yok!");
                    Console.WriteLine($"[MESAFE FİLTRE] ⚠️ POST /api/urunler/koordinatlari-guncelle endpoint'ini çağırın!");
                }

                // Client-side'da Haversine formülü ile mesafe hesapla
                var mesafeListesi = koordinatliUrunler
                    .Select(u => new
                    {
                        u.Id,
                        u.Ad,
                        u.Konum,
                        Mesafe = HaversineDistance(
                            kullaniciLatitude.Value, kullaniciLongitude.Value,
                            u.Latitude!.Value, u.Longitude!.Value)
                    })
                    .OrderBy(u => u.Mesafe)
                    .ToList();

                // Debug: En yakın 5 ürünü göster
                Console.WriteLine($"[MESAFE FİLTRE] En yakın 5 ürün:");
                foreach (var item in mesafeListesi.Take(5))
                {
                    Console.WriteLine($"[MESAFE FİLTRE]   - {item.Ad} ({item.Konum}): {item.Mesafe:F2}km");
                }

                mesafeIcindeUrunIdler = mesafeListesi
                    .Where(u => u.Mesafe <= maxMesafe.Value)
                    .Select(u => u.Id)
                    .ToList();

                Console.WriteLine($"[MESAFE FİLTRE] {maxMesafe}km içinde {mesafeIcindeUrunIdler.Count} ürün bulundu");
                Console.WriteLine($"[MESAFE FİLTRE] ========================================");
                
                if (mesafeIcindeUrunIdler.Count == 0)
                {
                    Console.WriteLine($"[MESAFE FİLTRE] ⚠️ Filtre sonucu boş! En yakın ürün: {mesafeListesi.FirstOrDefault()?.Mesafe:F2}km uzakta");
                }
                
                query = query.Where(u => mesafeIcindeUrunIdler.Contains(u.Id));
            }

            // 🔄 Sıralama uygula
            query = siralama?.ToLower() switch
            {
                "fiyat_artan" => query.OrderBy(u => u.Fiyat),
                "fiyat_azalan" => query.OrderByDescending(u => u.Fiyat),
                "eski" => query.OrderBy(u => u.Id),
                "yeni" or _ => query.OrderByDescending(u => u.Id) // Varsayılan: En yeni
            };

            Console.WriteLine($"[URUNLER API] Sıralama uygulandı: {siralama}");

            // Toplam ürün sayısı
            var toplamUrunSayisi = await query.CountAsync();
            var toplamSayfaSayisi = (int)Math.Ceiling(toplamUrunSayisi / (double)sayfaBoyutu);

            // Sayfalama uygula
            var urunler = await query
                                 .Skip((sayfa - 1) * sayfaBoyutu)
                                 .Take(sayfaBoyutu)
                                 .Select(u => new
                                 {
                                     u.Id,
                                     u.Ad,
                                     u.Aciklama,
                                     u.Fiyat,
                                     u.ResimUrl,
                                     u.EklemeTarihi,
                                     u.KullaniciId,
                                     u.Slug, // 🔗 SEO-friendly URL
                                     Kategori = new
                                     {
                                         u.Kategori.Id,
                                         u.Kategori.Ad
                                     },
                                     // Konum
                                     Konum = u.Konum,
                                     // 📦 Stok bilgisi
                                     u.StokMiktari,
                                     StokVarMi = u.StokMiktari > 0
                                 })
                                 .ToListAsync();

            Console.WriteLine($"[URUNLER API] {urunler.Count}/{toplamUrunSayisi} ürün döndürülüyor (Sayfa {sayfa}/{toplamSayfaSayisi})");

            return Ok(new
            {
                Urunler = urunler,
                SayfaBilgisi = new
                {
                    MevcutSayfa = sayfa,
                    SayfaBoyutu = sayfaBoyutu,
                    ToplamSayfa = toplamSayfaSayisi,
                    ToplamUrun = toplamUrunSayisi,
                    OncekiSayfa = sayfa > 1,
                    SonrakiSayfa = sayfa < toplamSayfaSayisi
                }
            });
        }

        // 2. SADECE BENİM ÜRÜNLERİMİ GETİR (İlanlarım Sayfası - STOK BİLGİSİYLE + SAYFALAMA)
        [HttpGet("benim")]
        [Authorize]
        public async Task<IActionResult> GetBenimUrunlerim(
            [FromQuery] int sayfa = 1,
            [FromQuery] int sayfaBoyutu = 10)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Geçersiz kullanıcı kimliği.");
            }

            var query = _context.Urunler.Where(u => u.KullaniciId == userId);

            // Toplam ürün sayısı
            var toplamUrunSayisi = await query.CountAsync();
            var toplamSayfaSayisi = (int)Math.Ceiling(toplamUrunSayisi / (double)sayfaBoyutu);

            var urunler = await query
                                .OrderByDescending(u => u.Id) // En yeni önce
                                .Skip((sayfa - 1) * sayfaBoyutu)
                                .Take(sayfaBoyutu)
                                .Select(u => new
                                {
                                    Id = u.Id,
                                    Ad = u.Ad,
                                    Fiyat = u.Fiyat,
                                    Aciklama = u.Aciklama,
                                    ResimUrl = u.ResimUrl,
                                    Slug = u.Slug, // 🔗 SEO-friendly URL
                                    // Konum
                                    Konum = u.Konum,
                                    // 📦 Stok bilgisi
                                    StokMiktari = u.StokMiktari,
                                    StokVarMi = u.StokMiktari > 0,
                                    Kategori = new
                                    {
                                        Id = u.Kategori.Id,
                                        Ad = u.Kategori.Ad
                                    }
                                })
                                .ToListAsync();

            Console.WriteLine($"[BENIM URUNLER] {urunler.Count}/{toplamUrunSayisi} ürün döndürülüyor (Sayfa {sayfa}/{toplamSayfaSayisi})");

            return Ok(new
            {
                Urunler = urunler,
                SayfaBilgisi = new
                {
                    MevcutSayfa = sayfa,
                    SayfaBoyutu = sayfaBoyutu,
                    ToplamSayfa = toplamSayfaSayisi,
                    ToplamUrun = toplamUrunSayisi,
                    OncekiSayfa = sayfa > 1,
                    SonrakiSayfa = sayfa < toplamSayfaSayisi
                }
            });
        }

        // 3A. TEK BİR ÜRÜN GETİR - SLUG İLE (SEO-Friendly)
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetUrunBySlug(string slug)
        {
            // Slug'dan ID çıkar
            var id = SlugHelper.ExtractIdFromSlug(slug);
            
            if (id == null)
            {
                return BadRequest("Geçersiz ürün URL'i.");
            }

            var urun = await _context.Urunler
                                     .Include(u => u.Kategori)
                                     .Include(u => u.Kullanici)
                                     .Where(u => u.Id == id.Value)
                                     .Select(u => new
                                     {
                                         u.Id,
                                         u.Ad,
                                         u.Aciklama,
                                         u.Fiyat,
                                         u.EklemeTarihi,
                                         u.KategoriId,
                                         ResimUrl = u.ResimUrl,
                                         Slug = u.Slug, // 🔗 SEO-friendly URL
                                         KategoriAdi = u.Kategori.Ad,
                                         SaticiId = u.KullaniciId,
                                         SaticiAdi = u.Kullanici.KullaniciAdi,
                                         SaticiProfilResmi = u.Kullanici.ProfilResmiUrl,
                                         // Konum
                                         Konum = u.Konum,
                                         // 📦 Stok bilgisi
                                         StokMiktari = u.StokMiktari,
                                         StokVarMi = u.StokMiktari > 0
                                     })
                                     .FirstOrDefaultAsync();

            if (urun == null)
            {
                return NotFound("Ürün bulunamadı.");
            }

            Console.WriteLine($"[URUN API] Slug ile ürün getirildi: {slug} (ID: {id})");

            return Ok(urun);
        }

        // 3B. TEK BİR ÜRÜN GETİR - ID İLE (Backward Compatibility)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUrun(int id)
        {
            var urun = await _context.Urunler
                                     .Include(u => u.Kategori)
                                     .Include(u => u.Kullanici)
                                     .Where(u => u.Id == id)
                                     .Select(u => new
                                     {
                                         u.Id,
                                         u.Ad,
                                         u.Aciklama,
                                         u.Fiyat,
                                         u.EklemeTarihi,
                                         u.KategoriId,
                                         ResimUrl = u.ResimUrl,
                                         Slug = u.Slug, // 🔗 SEO-friendly URL
                                         KategoriAdi = u.Kategori.Ad,
                                         SaticiId = u.KullaniciId,
                                         SaticiAdi = u.Kullanici.KullaniciAdi,
                                         // Konum
                                         Konum = u.Konum,
                                         // 📦 Stok bilgisi
                                         StokMiktari = u.StokMiktari,
                                         StokVarMi = u.StokMiktari > 0
                                     })
                                     .FirstOrDefaultAsync();

            if (urun == null)
            {
                return NotFound("Ürün bulunamadı.");
            }

            return Ok(urun);
        }

        // 4. YENİ ÜRÜN EKLE (STOK BİLGİSİYLE + OTOMATIK GEOCODING)
        [HttpPost("ekle")]
        [Authorize]
        public async Task<ActionResult<Urun>> PostUrun(UrunEkleDto urunDto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Geçersiz kullanıcı kimliği.");
            }

            var yeniUrun = new Urun
            {
                Ad = urunDto.Ad,
                Aciklama = urunDto.Aciklama,
                Fiyat = urunDto.Fiyat,
                KategoriId = urunDto.KategoriId,
                KullaniciId = userId,
                EklemeTarihi = DateTime.Now,
                ResimUrl = urunDto.ResimUrl,
                // Konum
                Konum = urunDto.Konum,
                // 📦 Stok bilgisi
                StokMiktari = urunDto.StokMiktari,
                // Geçici slug (ID olmadan)
                Slug = SlugHelper.GenerateSlug(urunDto.Ad)
            };

            // 🌍 OTOMATIK KOORDINAT EKLE (Google Geocoding API)
            if (!string.IsNullOrEmpty(urunDto.Konum))
            {
                try
                {
                    Console.WriteLine($"[GEOCODING] Yeni ürün için koordinat alınıyor: {urunDto.Konum}");
                    var (lat, lon) = await _geocodingService.GetCoordinatesAsync(urunDto.Konum);
                    
                    if (lat.HasValue && lon.HasValue)
                    {
                        yeniUrun.Latitude = lat.Value;
                        yeniUrun.Longitude = lon.Value;
                        Console.WriteLine($"[GEOCODING] ✅ Yeni ürün '{yeniUrun.Ad}' için koordinat eklendi: ({lat}, {lon})");
                    }
                    else
                    {
                        Console.WriteLine($"[GEOCODING] ⚠️ '{urunDto.Konum}' için koordinat bulunamadı - Ürün yine de eklenecek");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GEOCODING] ❌ Koordinat alma hatası: {ex.Message}");
                    // Koordinat eklenemese de ürün eklenir (fail-safe)
                }
            }

            _context.Urunler.Add(yeniUrun);
            await _context.SaveChangesAsync();

            // 🔗 ID ile benzersiz slug oluştur
            yeniUrun.Slug = SlugHelper.GenerateUniqueSlug(urunDto.Ad, yeniUrun.Id);
            await _context.SaveChangesAsync();

            Console.WriteLine($"[YENİ ÜRÜN] Ürün eklendi: ID={yeniUrun.Id}, Konum={yeniUrun.Konum}, Koordinat=({yeniUrun.Latitude}, {yeniUrun.Longitude})");

            return CreatedAtAction("GetUrunler", new { id = yeniUrun.Id }, yeniUrun);
        }

        // 5. ÜRÜN GÜNCELLEME (KONUM DEĞİŞİRSE KOORDINAT GÜNCELLE)
        [HttpPut("guncelle")]
        [Authorize]
        public async Task<IActionResult> PutUrun(UrunGuncelleDto guncelleDto)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized("Geçersiz kullanıcı kimliği.");
                }

                var urun = await _context.Urunler.FindAsync(guncelleDto.Id);
                if (urun == null)
                {
                    return NotFound("Ürün bulunamadı.");
                }

                if (urun.KullaniciId != userId)
                {
                    return StatusCode(403, "Bu ilanı düzenlemeye yetkiniz yok.");
                }

                // 🌍 Konum değiştiyse koordinatı güncelle
                if (!string.IsNullOrEmpty(guncelleDto.Konum) && guncelleDto.Konum != urun.Konum)
                {
                    try
                    {
                        Console.WriteLine($"[GEOCODING] Konum değişti: '{urun.Konum}' -> '{guncelleDto.Konum}'");
                        var (lat, lon) = await _geocodingService.GetCoordinatesAsync(guncelleDto.Konum);
                        
                        if (lat.HasValue && lon.HasValue)
                        {
                            urun.Latitude = lat.Value;
                            urun.Longitude = lon.Value;
                            Console.WriteLine($"[GEOCODING] ✅ Koordinat güncellendi: ({lat}, {lon})");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[GEOCODING] ❌ Koordinat güncelleme hatası: {ex.Message}");
                    }
                }

                urun.Ad = guncelleDto.Ad;
                urun.Aciklama = guncelleDto.Aciklama;
                urun.Fiyat = guncelleDto.Fiyat;
                urun.KategoriId = guncelleDto.KategoriId;
                // Konum
                urun.Konum = guncelleDto.Konum;
                // 📦 Stok güncelleme
                urun.StokMiktari = guncelleDto.StokMiktari;
                // 🔗 Slug güncelleme (ürün adı değiştiyse)
                urun.Slug = SlugHelper.GenerateUniqueSlug(guncelleDto.Ad, urun.Id);

                _context.Entry(urun).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Ürün başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Güncelleme hatası: {ex.Message}");
            }
        }

        // 6. ÜRÜN SİLME
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Sil(int id)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized("Kullanıcı kimliği doğrulanamadı.");
                }

                var urun = await _context.Urunler.FindAsync(id);
                if (urun == null)
                {
                    return NotFound("Silinecek ürün bulunamadı.");
                }

                if (urun.KullaniciId != userId)
                {
                    return StatusCode(403, "Bu ilanı silmeye yetkiniz yok! Sadece kendi ilanlarınızı silebilirsiniz.");
                }

                _context.Urunler.Remove(urun);
                await _context.SaveChangesAsync();

                return Ok(new { message = "İlan başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sunucu hatası: {ex.Message}");
            }
        }

        // 🌍 HAVERSINE FORMÜLÜ - İki koordinat arası mesafe hesaplama (km)
        private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Dünya'nın yarıçapı (km)
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        // 🗺️ MEVCUT ÜRÜNLERİN KOORDİNATLARINI GÜNCELLE (ADMIN)
        [HttpPost("koordinatlari-guncelle")]
        [Authorize]
        public async Task<IActionResult> KoordinatlariGuncelle()
        {
            try
            {
                // Sadece admin yapabilir (opsiyonel - kaldırılabilir)
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized();
                }

                // Koordinatı olmayan ürünleri al
                var urunler = await _context.Urunler
                    .Where(u => !string.IsNullOrEmpty(u.Konum) && 
                               (!u.Latitude.HasValue || !u.Longitude.HasValue))
                    .ToListAsync();

                Console.WriteLine($"[GEOCODING] {urunler.Count} ürün için koordinat alınacak");

                int basarili = 0;
                int basarisiz = 0;

                foreach (var urun in urunler)
                {
                    var (lat, lon) = await _geocodingService.GetCoordinatesAsync(urun.Konum);
                    
                    if (lat.HasValue && lon.HasValue)
                    {
                        urun.Latitude = lat.Value;
                        urun.Longitude = lon.Value;
                        basarili++;
                        Console.WriteLine($"[GEOCODING] ✅ {urun.Ad} -> ({lat}, {lon})");
                    }
                    else
                    {
                        basarisiz++;
                        Console.WriteLine($"[GEOCODING] ❌ {urun.Ad} -> Koordinat alınamadı");
                    }

                    // Google API rate limit'i aşmamak için bekle
                    await Task.Delay(200);
                }

                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    message = $"Koordinat güncelleme tamamlandı. Başarılı: {basarili}, Başarısız: {basarisiz}",
                    basarili,
                    basarisiz,
                    toplam = urunler.Count
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GEOCODING] Hata: {ex.Message}");
                return StatusCode(500, $"Hata: {ex.Message}");
            }
        }
    }
}