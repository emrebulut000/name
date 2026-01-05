-- =========================================
-- UTF-8 KARAKTER SETÝ DÜZELTMESÝ
-- =========================================

USE ikinciel_proje_db;

-- 1. Veritabaný charset'ini UTF-8'e çevir
ALTER DATABASE ikinciel_proje_db 
CHARACTER SET = utf8mb4 
COLLATE = utf8mb4_unicode_ci;

-- 2. Tüm tablolarý UTF-8'e çevir
ALTER TABLE bildirimler CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE chat_mesajlari CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE favoriler CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE kategoriler CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE kullanicilar CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE mesajlar CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE satici_degerlendirmeleri CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE sepet_urunleri CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE sifre_reset_tokenleri CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE siparis_detaylari CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE siparisler CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE teklifler CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE urunler CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 3. Bozuk karakterleri düzelt (örnekler)
-- Bildirimler tablosu
UPDATE bildirimler SET Mesaj = REPLACE(Mesaj, '??', '??') WHERE Mesaj LIKE '%??%';
UPDATE bildirimler SET Baslik = REPLACE(Baslik, '??', '?') WHERE Baslik LIKE '%??%';

-- ChatBot mesajlarý
UPDATE chat_mesajlari SET Mesaj = REPLACE(Mesaj, '??', '??') WHERE Mesaj LIKE '%??%';

-- 4. Kontrol et
SELECT 
    TABLE_NAME, 
    TABLE_COLLATION 
FROM 
    information_schema.TABLES 
WHERE 
    TABLE_SCHEMA = 'ikinciel_proje_db';

SELECT '? UTF-8 dönüþümü tamamlandý!' AS Sonuc;
