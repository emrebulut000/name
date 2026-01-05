-- ========================================
-- SATICI PUANLAMA SÝSTEMÝ - VERÝTABANI TABLOLARI
-- ========================================

USE ikinciel_proje_db;

-- 1. KULLANICILAR TABLOSUNA PUAN ALANLARI EKLE
ALTER TABLE kullanicilar 
ADD COLUMN OrtalamaPuan DECIMAL(3,2) NULL DEFAULT NULL,
ADD COLUMN ToplamDegerlendirme INT NOT NULL DEFAULT 0;

-- 2. SATICI DEÐERLENDÝRMELERÝ TABLOSUNU OLUÞTUR
CREATE TABLE IF NOT EXISTS satici_degerlendirmeleri (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    SaticiId INT NOT NULL,
    DegerlendirenId INT NOT NULL,
    SiparisId INT NOT NULL,
    Puan INT NOT NULL CHECK (Puan BETWEEN 1 AND 5),
    Yorum VARCHAR(500) NULL,
    DegerlendirmeTarihi DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Keys
    CONSTRAINT FK_Degerlendirme_Satici FOREIGN KEY (SaticiId) 
        REFERENCES kullanicilar(Id) ON DELETE RESTRICT,
    
    CONSTRAINT FK_Degerlendirme_Degerlendiren FOREIGN KEY (DegerlendirenId) 
        REFERENCES kullanicilar(Id) ON DELETE RESTRICT,
    
    CONSTRAINT FK_Degerlendirme_Siparis FOREIGN KEY (SiparisId) 
        REFERENCES siparisler(Id) ON DELETE CASCADE,
    
    -- Bir sipariþ için bir kiþi sadece bir kez deðerlendirme yapabilir
    CONSTRAINT UQ_Siparis_Degerlendiren UNIQUE (SiparisId, DegerlendirenId)
);

-- 3. ÝNDEXLER (Performans için)
CREATE INDEX idx_satici_degerlendirmeleri_satici ON satici_degerlendirmeleri(SaticiId);
CREATE INDEX idx_satici_degerlendirmeleri_degerlendiren ON satici_degerlendirmeleri(DegerlendirenId);
CREATE INDEX idx_satici_degerlendirmeleri_siparis ON satici_degerlendirmeleri(SiparisId);

-- 4. DOÐRULAMA
DESCRIBE satici_degerlendirmeleri;
DESCRIBE kullanicilar;

SELECT 'Satýcý Puanlama Sistemi tablolarý baþarýyla oluþturuldu!' as Mesaj;
