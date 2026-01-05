-- ========================================
-- SEPET TABLOSU KOLON ADINI DÜZELT
-- ========================================

USE ikinciel_proje_db;

-- Mevcut kolon adýný kontrol et
DESCRIBE sepet_urunleri;

-- Kolon adýný düzelt: EklemeTarihi -> EklenmeTarihi
ALTER TABLE sepet_urunleri 
CHANGE COLUMN EklemeTarihi EklenmeTarihi DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;

-- Doðrulama
DESCRIBE sepet_urunleri;

SELECT 'Kolon adý baþarýyla güncellendi!' as Mesaj;
