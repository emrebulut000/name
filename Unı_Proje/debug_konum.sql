-- ?? KONUM SORUNU TESPÝT VE ÇÖZÜM
-- MySQL Workbench'te çalýþtýrýn

USE ikinciel_proje_db;

-- 1. Veritabaný ve tablo kontrolü
SELECT DATABASE() AS 'Aktif Veritabani';

SHOW TABLES LIKE 'urunler';

-- 2. Konum sütunu var mý?
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'ikinciel_proje_db' 
  AND TABLE_NAME = 'urunler' 
  AND COLUMN_NAME = 'Konum';

-- Eðer yukarýdaki sorgu 0 satýr döndüyse, sütun yok demektir. Ekleyin:
-- ALTER TABLE urunler ADD COLUMN Konum VARCHAR(200) NULL;

-- 3. Tablo yapýsýný göster
DESCRIBE urunler;

-- 4. Mevcut ürünleri göster
SELECT 
    Id, 
    Ad, 
    Konum,
    Fiyat,
    StokMiktari,
    CASE 
        WHEN Konum IS NULL THEN '? NULL'
        WHEN Konum = '' THEN '?? BOÞ STRING'
        ELSE CONCAT('? ', Konum)
    END AS KonumDurumu
FROM urunler 
ORDER BY Id;

-- 5. Ýstatistikler
SELECT 
    COUNT(*) AS ToplamUrun,
    SUM(CASE WHEN Konum IS NOT NULL AND Konum != '' THEN 1 ELSE 0 END) AS KonumuDoluOlanlar,
    SUM(CASE WHEN Konum IS NULL THEN 1 ELSE 0 END) AS KonumNULL,
    SUM(CASE WHEN Konum = '' THEN 1 ELSE 0 END) AS KonumBosString
FROM urunler;

-- 6. Test verisi ekle (TÜM NULL OLANLARA - ID'ye bakmadan)
UPDATE urunler 
SET Konum = CASE 
    WHEN Id % 5 = 0 THEN 'Bursa, Osmangazi'
    WHEN Id % 5 = 1 THEN 'Ýstanbul, Kadýköy'
    WHEN Id % 5 = 2 THEN 'Ankara, Çankaya'
    WHEN Id % 5 = 3 THEN 'Ýzmir, Konak'
    ELSE 'Antalya, Muratpaþa'
END
WHERE Konum IS NULL OR Konum = '';

-- Etkilenen satýr sayýsýný görmek için:
SELECT ROW_COUNT() AS 'Güncellenen Satir Sayisi';

-- 7. Sonucu kontrol et
SELECT Id, Ad, Konum, Fiyat FROM urunler ORDER BY Id LIMIT 10;

-- 8. JSON formatýnda test et (API'nin döndürdüðü gibi)
SELECT JSON_OBJECT(
    'Id', Id,
    'Ad', Ad,
    'Konum', Konum,
    'Fiyat', Fiyat
) AS JsonData
FROM urunler 
LIMIT 3;
