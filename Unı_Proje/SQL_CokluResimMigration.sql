-- ============================================
-- ÇOKLU RESÝM SÝSTEMÝ - SQL MÝGRATÝON
-- ============================================

-- Yeni tablo: urun_resimleri
CREATE TABLE IF NOT EXISTS urun_resimleri (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    UrunId INT NOT NULL,
    ResimUrl VARCHAR(500) NOT NULL,
    Sira INT DEFAULT 0,
    AnaResimMi TINYINT(1) DEFAULT 0,
    EklemeTarihi DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Key
    CONSTRAINT FK_UrunResim_Urun FOREIGN KEY (UrunId) 
        REFERENCES urunler(Id) ON DELETE CASCADE,
    
    -- Index
    INDEX idx_urun_resimleri_urunid (UrunId),
    INDEX idx_urun_resimleri_anaresim (UrunId, AnaResimMi)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Mevcut ürünlerin ResimUrl'lerini yeni tabloya kopyala (opsiyonel)
INSERT INTO urun_resimleri (UrunId, ResimUrl, Sira, AnaResimMi, EklemeTarihi)
SELECT 
    Id AS UrunId,
    ResimUrl,
    1 AS Sira,
    1 AS AnaResimMi,
    EklemeTarihi
FROM urunler
WHERE ResimUrl IS NOT NULL AND ResimUrl != '';

-- Baþarý mesajý
SELECT 'Çoklu resim tablosu oluþturuldu ve mevcut resimler kopyalandý!' AS Mesaj;

-- Kontrol
SELECT COUNT(*) AS ToplamResim FROM urun_resimleri;
