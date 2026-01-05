-- ==============================================
-- SEO-FRIENDLY URL (SLUG) EKLEME MÝGRATION
-- ==============================================

USE ikinciel_proje_db;

-- 1. Slug sütunu ekle
ALTER TABLE urunler 
ADD COLUMN Slug VARCHAR(200) NOT NULL DEFAULT '';

-- 2. Mevcut ürünler için slug oluþtur (Basit versiyon - ID bazlý)
UPDATE urunler 
SET Slug = CONCAT(
    LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
        Ad, ' ', '-'), 'ç', 'c'), 'ð', 'g'), 'ý', 'i'), 'ö', 'o'), 'þ', 's')),
    '-', Id
);

-- 3. Index ekle (SEO performansý için)
CREATE INDEX idx_slug ON urunler(Slug);

-- 4. Unique constraint ekle (Ayný slug tekrar etmesin)
-- Not: ID eklediðimiz için zaten benzersiz olacak
-- ALTER TABLE urunler ADD UNIQUE KEY unique_slug (Slug);

SELECT '? Slug sütunu baþarýyla eklendi ve mevcut ürünler güncellendi!' AS Sonuc;
