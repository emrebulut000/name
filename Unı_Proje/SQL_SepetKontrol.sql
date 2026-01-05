-- ========================================
-- SEPET TABLOSU KONTROL VE DÜZELTME
-- ========================================

USE ikinciel_proje_db;

-- 1. Sepet tablosunun yapýsýný kontrol et
DESCRIBE sepet_urunleri;

-- 2. Foreign key'leri kontrol et
SELECT 
    CONSTRAINT_NAME,
    TABLE_NAME,
    COLUMN_NAME,
    REFERENCED_TABLE_NAME,
    REFERENCED_COLUMN_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = 'ikinciel_proje_db'
  AND TABLE_NAME = 'sepet_urunleri'
  AND REFERENCED_TABLE_NAME IS NOT NULL;

-- 3. Mevcut sepet verilerini kontrol et
SELECT COUNT(*) as ToplamSepetKaydi FROM sepet_urunleri;

-- 4. Kullanýcýlarý kontrol et (ID 1 var mý?)
SELECT Id, KullaniciAdi, Email FROM kullanicilar LIMIT 5;

-- 5. Ürünleri kontrol et (ID 11 var mý?)
SELECT Id, Ad, StokMiktari FROM urunler WHERE Id IN (11, 1, 2, 3) ORDER BY Id;

-- ========================================
-- SORUN ÇÖZÜMÜ (Gerekirse)
-- ========================================

-- Eðer sepet tablosu yoksa veya hatalýysa:
-- DROP TABLE IF EXISTS sepet_urunleri;

-- CREATE TABLE sepet_urunleri (
--     Id INT AUTO_INCREMENT PRIMARY KEY,
--     KullaniciId INT NOT NULL,
--     UrunId INT NOT NULL,
--     EklemeTarihi DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
--     FOREIGN KEY (KullaniciId) REFERENCES kullanicilar(Id) ON DELETE CASCADE,
--     FOREIGN KEY (UrunId) REFERENCES urunler(Id) ON DELETE CASCADE,
--     UNIQUE KEY UK_Sepet_KullaniciUrun (KullaniciId, UrunId)
-- );
