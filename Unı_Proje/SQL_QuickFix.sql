-- GEÇÝCÝ TEST ÝÇÝN: TÜM ÜRÜNLERE BOL STOK EKLE
USE ikinciel_proje_db;

UPDATE urunler SET StokMiktari = 999;

SELECT 'Tüm ürünlere 999 adet stok eklendi' as Mesaj;
SELECT Id, Ad, Fiyat, StokMiktari FROM urunler;
