-- ====================================================
-- BÝLDÝRÝMLER TABLOSU
-- ====================================================

USE ikinciel_proje_db;

-- 1. Bildirimler tablosunu oluþtur
CREATE TABLE IF NOT EXISTS bildirimler (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    KullaniciId INT NOT NULL,
    Tip VARCHAR(50) NOT NULL,
    Baslik VARCHAR(200) NOT NULL,
    Mesaj VARCHAR(500) NOT NULL,
    IliskiliId INT NULL,
    Okundu BOOLEAN NOT NULL DEFAULT FALSE,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (KullaniciId) REFERENCES kullanicilar(Id) ON DELETE CASCADE,
    INDEX idx_kullanici (KullaniciId),
    INDEX idx_okundu (Okundu),
    INDEX idx_tarih (OlusturmaTarihi)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 2. Örnek bildirimler ekle (test için)
INSERT INTO bildirimler (KullaniciId, Tip, Baslik, Mesaj, IliskiliId, Okundu) VALUES
(1, 'siparis', 'Sipariþ Durumu Güncellendi', 'Sipariþiniz #1 kargoya verildi.', 1, FALSE),
(1, 'degerlendirme', 'Yeni Deðerlendirme', 'Ürününüze 5 yýldýzlý deðerlendirme yapýldý.', 1, FALSE),
(2, 'mesaj', 'Yeni Mesaj', 'Satýcý size mesaj gönderdi.', 1, TRUE);

-- 3. Kontrol et
SELECT * FROM bildirimler ORDER BY OlusturmaTarihi DESC;
