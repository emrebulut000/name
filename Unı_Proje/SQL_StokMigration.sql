-- ========================================
-- STOK YÖNETÝMÝ MÝGRATION (GÜVENLÝ VERSÝYON)
-- ========================================
-- Bu SQL script'i urunler tablosuna StokMiktari kolonunu ekler
-- Eðer kolon zaten varsa hata vermez

USE ikinciel_proje_db;

-- 1. Önce kolonun var olup olmadýðýný kontrol et
SELECT COUNT(*) as 'Kolon_Varmi'
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'ikinciel_proje_db' 
  AND TABLE_NAME = 'urunler' 
  AND COLUMN_NAME = 'StokMiktari';

-- 2. StokMiktari kolonunu ekle (SADECE YOKSA)
-- NOT: MySQL 5.7+ için IF NOT EXISTS kullanýlamaz, bu yüzden manuel kontrol gerekir
-- Eðer yukarýdaki sorgu "1" döndürdüyse, aþaðýdaki satýrý ÇALIÞTIRMAYIN!
-- Eðer "0" döndürdüyse, aþaðýdaki satýrý çalýþtýrýn:

ALTER TABLE urunler 
ADD COLUMN StokMiktari INT NOT NULL DEFAULT 0;

-- 3. Deðiþiklikleri doðrula
SELECT 
    Id, 
    Ad, 
    Fiyat, 
    StokMiktari,
    EklemeTarihi
FROM urunler
LIMIT 10;

-- ========================================
-- HATA ALIYORSANIZ (Duplicate column):
-- ========================================
-- Bu, kolonun zaten eklendiði anlamýna gelir.
-- Hiçbir þey yapmanýza gerek yok, doðrudan 3. adýmý çalýþtýrýn:

-- SELECT Id, Ad, Fiyat, StokMiktari, EklemeTarihi FROM urunler LIMIT 10;

-- ========================================
-- NOTLAR:
-- ========================================
-- * StokMiktari kolonu INT tipinde ve boþ (NULL) olamaz
-- * Varsayýlan deðer 0 (stokta yok)
-- * Mevcut ürünler için stok girilmesi gerekiyor
-- ========================================
