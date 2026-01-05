-- =====================================================
-- CHATBOT MESAJLARI TABLOSU
-- =====================================================
-- Bu script ChatBot özelliði için gerekli veritabaný
-- tablosunu oluþturur.
-- =====================================================

USE ikinciel_proje_db;

-- Chat mesajlarý tablosu
CREATE TABLE IF NOT EXISTS chat_mesajlari (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    KullaniciId INT NULL,
    SessionId VARCHAR(100) NOT NULL,
    Mesaj VARCHAR(1000) NOT NULL,
    Gonderen VARCHAR(10) NOT NULL,  -- 'user' veya 'bot'
    GondermeTarihi DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Key
    CONSTRAINT FK_ChatMesaj_Kullanici 
        FOREIGN KEY (KullaniciId) 
        REFERENCES kullanicilar(Id) 
        ON DELETE SET NULL,
    
    -- Index'ler (performans için)
    INDEX idx_session (SessionId),
    INDEX idx_kullanici (KullaniciId),
    INDEX idx_tarih (GondermeTarihi)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Test mesajlarý ekle (opsiyonel)
INSERT INTO chat_mesajlari (KullaniciId, SessionId, Mesaj, Gonderen, GondermeTarihi)
VALUES
    (NULL, 'test-session-1', 'Merhaba, sipariþ durumumu öðrenebilir miyim?', 'user', NOW()),
    (NULL, 'test-session-1', '?? Sipariþ durumunuzu öðrenmek için ''Sipariþlerim'' sayfasýný ziyaret edebilirsiniz.', 'bot', NOW());

SELECT '? ChatBot tablosu baþarýyla oluþturuldu!' AS Sonuc;
