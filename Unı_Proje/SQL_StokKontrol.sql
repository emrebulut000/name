-- ========================================
-- STOK SORUN GÝDERME VE DOÐRULAMA
-- ========================================

USE ikinciel_proje_db;

-- 1. Mevcut ürünlerin durumunu kontrol et
SELECT 
    Id, 
    Ad, 
    Fiyat,
    StokMiktari,
    CASE 
        WHEN StokMiktari IS NULL THEN 'NULL DEÐER!'
        WHEN StokMiktari = 0 THEN 'Stokta Yok'
        WHEN StokMiktari > 0 THEN 'Stokta Var'
    END as StokDurumu
FROM urunler;

-- 2. NULL deðerleri düzelt (eðer varsa)
UPDATE urunler 
SET StokMiktari = 10 
WHERE StokMiktari IS NULL OR StokMiktari = 0;

-- 3. Doðrulama yap
SELECT 
    COUNT(*) as ToplamUrun,
    SUM(CASE WHEN StokMiktari > 0 THEN 1 ELSE 0 END) as StoktaOlanlar,
    SUM(CASE WHEN StokMiktari = 0 THEN 1 ELSE 0 END) as StoktaOlmayanlar,
    SUM(CASE WHEN StokMiktari IS NULL THEN 1 ELSE 0 END) as NullDegerler
FROM urunler;

-- 4. Tablo yapýsýný kontrol et
DESCRIBE urunler;

-- ========================================
-- EXPECTED OUTPUT:
-- ========================================
-- StokMiktari kolonu:
-- - Type: int
-- - Null: NO
-- - Default: 0
-- 
-- Tüm ürünlerin StokMiktari > 0 olmalý
-- ========================================
