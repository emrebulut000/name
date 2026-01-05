-- ============================================
-- EMAIL DOÐRULAMA SÝSTEMÝ - VERITABANI TABLOSU
-- ============================================

USE ikinciel_proje_db;

-- Email doðrulama tokenlarý tablosu
CREATE TABLE IF NOT EXISTS email_dogrulama_tokenleri (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Email VARCHAR(255) NOT NULL,
    Token VARCHAR(6) NOT NULL,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    SonKullanmaTarihi DATETIME NOT NULL,
    Kullanildi BOOLEAN NOT NULL DEFAULT FALSE,
    
    INDEX idx_email (Email),
    INDEX idx_token (Token),
    INDEX idx_kullanildi (Kullanildi),
    INDEX idx_son_kullanma (SonKullanmaTarihi)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Ýstatistik
SELECT '? Email doðrulama tokenlarý tablosu oluþturuldu!' as Durum;

-- Tablonun yapýsýný göster
DESCRIBE email_dogrulama_tokenleri;
