-- ============================================
-- HÝZLI TEST VE DÜZELTME SCRÝPTÝ
-- ============================================

-- 1. MIGRATION KONTROLÜ
-- Latitude ve Longitude kolonlarý var mý?
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'ikinciel_proje_db' 
  AND TABLE_NAME = 'urunler'
  AND COLUMN_NAME IN ('Latitude', 'Longitude');

-- Eðer yukarýda sonuç gelmiyorsa, kolonlarý ekle:
-- ALTER TABLE urunler ADD COLUMN Latitude DOUBLE NULL, ADD COLUMN Longitude DOUBLE NULL;

-- ============================================
-- 2. MEVCUT DURUM ANALÝZÝ
-- ============================================

-- Toplam koordinat durumu
SELECT 
    COUNT(*) AS ToplamUrun,
    SUM(CASE WHEN Latitude IS NOT NULL AND Longitude IS NOT NULL THEN 1 ELSE 0 END) AS TamKoordinatOlanlar,
    SUM(CASE WHEN Latitude IS NULL AND Longitude IS NULL THEN 1 ELSE 0 END) AS HicKoordinatOlmayanlar,
    SUM(CASE WHEN (Latitude IS NULL AND Longitude IS NOT NULL) OR (Latitude IS NOT NULL AND Longitude IS NULL) THEN 1 ELSE 0 END) AS YariKoordinatOlanlar
FROM urunler;

-- ============================================
-- 3. SON EKLENEN ÜRÜNLER (PROBLEM TESPÝTÝ)
-- ============================================

SELECT 
    Id, 
    Ad, 
    Konum,
    Latitude,
    Longitude,
    CASE 
        WHEN Latitude IS NOT NULL AND Longitude IS NOT NULL THEN '? TAM'
        WHEN Latitude IS NULL AND Longitude IS NULL THEN '? YOK'
        ELSE '?? EKSIK'
    END AS KoordinatDurumu,
    EklemeTarihi
FROM urunler 
ORDER BY Id DESC 
LIMIT 15;

-- ============================================
-- 4. MANUEL ÖRNEK KOORDINAT EKLEME (ACÝL ÇÖZÜM)
-- ============================================

-- Ýstanbul için örnek (41.0082, 28.9784)
UPDATE urunler 
SET Latitude = 41.0082, Longitude = 28.9784
WHERE (Latitude IS NULL OR Longitude IS NULL) 
  AND Konum LIKE '%Ýstanbul%'
LIMIT 10;

-- Ankara için örnek (39.9334, 32.8597)
UPDATE urunler 
SET Latitude = 39.9334, Longitude = 32.8597
WHERE (Latitude IS NULL OR Longitude IS NULL) 
  AND Konum LIKE '%Ankara%'
LIMIT 10;

-- Ýzmir için örnek (38.4237, 27.1428)
UPDATE urunler 
SET Latitude = 38.4237, Longitude = 27.1428
WHERE (Latitude IS NULL OR Longitude IS NULL) 
  AND Konum LIKE '%Ýzmir%'
LIMIT 10;

-- Bursa için örnek (40.1826, 29.0665)
UPDATE urunler 
SET Latitude = 40.1826, Longitude = 29.0665
WHERE (Latitude IS NULL OR Longitude IS NULL) 
  AND Konum LIKE '%Bursa%'
LIMIT 10;

-- Antalya için örnek (36.8969, 30.7133)
UPDATE urunler 
SET Latitude = 36.8969, Longitude = 30.7133
WHERE (Latitude IS NULL OR Longitude IS NULL) 
  AND Konum LIKE '%Antalya%'
LIMIT 10;

-- ============================================
-- 5. SONUÇ KONTROLÜ
-- ============================================

-- Güncellenmiþ durumu kontrol et
SELECT 
    COUNT(*) AS ToplamUrun,
    SUM(CASE WHEN Latitude IS NOT NULL AND Longitude IS NOT NULL THEN 1 ELSE 0 END) AS KoordinatOlanlar,
    ROUND(SUM(CASE WHEN Latitude IS NOT NULL AND Longitude IS NOT NULL THEN 1 ELSE 0 END) * 100.0 / COUNT(*), 2) AS YuzdeBaþarý
FROM urunler;

-- Koordinat haritasý
SELECT 
    Konum,
    COUNT(*) AS UrunSayisi,
    MIN(Latitude) AS MinLat,
    MAX(Latitude) AS MaxLat,
    MIN(Longitude) AS MinLon,
    MAX(Longitude) AS MaxLon
FROM urunler
WHERE Latitude IS NOT NULL
GROUP BY Konum
ORDER BY UrunSayisi DESC;

-- ============================================
-- 6. MESAFE FÝLTRESÝ TEST SORGUSU
-- ============================================

-- Kullanýcý konumu: Ýstanbul (41.0082, 28.9784)
-- 200km içindeki ürünleri bul

SET @kullanici_lat = 41.0082;
SET @kullanici_lon = 28.9784;
SET @max_mesafe_km = 200;

SELECT 
    Id,
    Ad,
    Konum,
    Latitude,
    Longitude,
    ROUND(
        6371 * ACOS(
            COS(RADIANS(@kullanici_lat)) * COS(RADIANS(Latitude)) * 
            COS(RADIANS(Longitude) - RADIANS(@kullanici_lon)) + 
            SIN(RADIANS(@kullanici_lat)) * SIN(RADIANS(Latitude))
        ), 
        2
    ) AS MesafeKM
FROM urunler
WHERE Latitude IS NOT NULL 
  AND Longitude IS NOT NULL
HAVING MesafeKM <= @max_mesafe_km
ORDER BY MesafeKM ASC
LIMIT 20;

-- ============================================
-- 7. HÝÇ ÜRÜN GÖZÜKMESÝ DURUMU ÝÇÝN ACÝL ÇÖZÜM
-- ============================================

-- EÐER HÝÇ ÜRÜN GÖZÜKMÜYORSA, TÜM ÜRÜNLERÝ ÝSTANBUL'A AYARLA (GEÇICI)
-- UPDATE urunler SET Latitude = 41.0082, Longitude = 28.9784 WHERE Latitude IS NULL;

SELECT 'Script tamamlandý! Yukarýdaki sorgularý sýrayla çalýþtýrýn.' AS Mesaj;
