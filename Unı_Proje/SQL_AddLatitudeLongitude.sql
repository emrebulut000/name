-- Ürünler tablosuna Latitude ve Longitude kolonlarýný ekle
ALTER TABLE urunler 
ADD COLUMN Latitude DOUBLE NULL,
ADD COLUMN Longitude DOUBLE NULL;

-- Index ekle (performans için)
CREATE INDEX idx_urunler_coordinates ON urunler(Latitude, Longitude);

SELECT 'Latitude ve Longitude kolonlarý baþarýyla eklendi!' AS Mesaj;
