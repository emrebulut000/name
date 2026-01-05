-- ? HIZLI KONUM EKLEME - BU SQL'Ý ÇALIÞTIRIN
USE ikinciel_proje_db;

-- 1. Mevcut durumu gör
SELECT 'ÖNCE' AS Durum, Id, Ad, Konum FROM urunler ORDER BY Id LIMIT 10;

-- 2. TÜM ürünlere konum ekle
UPDATE urunler 
SET Konum = CASE 
    WHEN Id % 5 = 0 THEN 'Bursa, Osmangazi'
    WHEN Id % 5 = 1 THEN 'Ýstanbul, Kadýköy'
    WHEN Id % 5 = 2 THEN 'Ankara, Çankaya'
    WHEN Id % 5 = 3 THEN 'Ýzmir, Konak'
    ELSE 'Antalya, Muratpaþa'
END
WHERE Konum IS NULL OR Konum = '';

-- 3. Kaç satýr güncellendi?
SELECT 'Güncellendi' AS Mesaj, ROW_COUNT() AS SatirSayisi;

-- 4. Sonucu kontrol et
SELECT 'SONRA' AS Durum, Id, Ad, Konum FROM urunler ORDER BY Id LIMIT 10;

-- 5. Özet istatistik
SELECT 
    COUNT(*) AS ToplamUrun,
    SUM(CASE WHEN Konum IS NOT NULL AND Konum != '' THEN 1 ELSE 0 END) AS KonumuOlan,
    SUM(CASE WHEN Konum IS NULL OR Konum = '' THEN 1 ELSE 0 END) AS KonumuOlmayan
FROM urunler;
