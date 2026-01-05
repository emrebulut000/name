-- ========================================
-- SEPET TABLOSU OLUÞTURMA
-- ========================================

USE ikinciel_proje_db;

-- Sepet Tablosunu Oluþtur
CREATE TABLE IF NOT EXISTS sepet_urunleri (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    KullaniciId INT NOT NULL,
    UrunId INT NOT NULL,
    EklemeTarihi DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Keys
    FOREIGN KEY (KullaniciId) REFERENCES kullanicilar(Id) ON DELETE CASCADE,
    FOREIGN KEY (UrunId) REFERENCES urunler(Id) ON DELETE CASCADE,
    
    -- Unique constraint: Bir kullanýcý ayný ürünü sepete sadece 1 kez ekleyebilir
    UNIQUE KEY UK_Sepet_KullaniciUrun (KullaniciId, UrunId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Doðrulama
SELECT 'Sepet tablosu baþarýyla oluþturuldu!' as Mesaj;
DESCRIBE sepet_urunleri;

-- ========================================
-- NOTLAR:
-- ========================================
-- * Bu tablo kullanýcýlarýn sepetindeki ürünleri saklar
-- * Cascade delete: Kullanýcý veya ürün silinirse sepetten de silinir
-- * Unique constraint: Ayný ürün tekrar eklenemez
-- ========================================
