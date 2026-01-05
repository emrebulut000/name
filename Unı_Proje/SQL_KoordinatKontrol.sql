-- 1. Tablo yapýsýný kontrol et
DESC urunler;

-- 2. Koordinatý olan/olmayan ürün sayýsý
SELECT 
    COUNT(*) AS ToplamUrun,
    SUM(CASE WHEN Latitude IS NOT NULL AND Longitude IS NOT NULL THEN 1 ELSE 0 END) AS KoordinatOlanlar,
    SUM(CASE WHEN Latitude IS NULL OR Longitude IS NULL THEN 1 ELSE 0 END) AS KoordinatOlmayanlar
FROM urunler;

-- 3. Son eklenen ürünlerin koordinatlarý
SELECT Id, Ad, Konum, Latitude, Longitude, EklemeTarihi
FROM urunler 
ORDER BY Id DESC 
LIMIT 10;

-- 4. Koordinatý olmayan ürünler
SELECT Id, Ad, Konum, Latitude, Longitude
FROM urunler 
WHERE Latitude IS NULL OR Longitude IS NULL;
