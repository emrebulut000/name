namespace Uný_Proje.Services
{
    /// <summary>
    /// ?? Bovi - Yardýmsever ChatBot Asistaný
    /// Kiþilik: Arkadaþ canlýsý, neþeli, yardýmsever
    /// Özellik: Ýnek emojileri ve samimi dil kullanýr
    /// </summary>
    public class ChatBotServis
    {
        // ?? Bovi'nin kiþilik özellikleri
        private readonly Random _random = new();
        private readonly string[] _boviReaksiyonlari = new[]
        {
            "??", "?", "??", "??", "?", "??", "??", "??"
        };

        // Anahtar kelime bazlý cevaplar
        private readonly Dictionary<string, string> _cevaplar = new()
        {
            // Selamlama
            { "merhaba", "?? Merhaba! Ben Bovi, senin yardýmcý arkadaþýn! Ýkinci El Market'e hoþ geldin! Bugün sana nasýl yardýmcý olabilirim? ?" },
            { "selam", "?? Selam! Ben Bovi! Sana yardýmcý olmak için buradayým! Ne aramýþtýn? ??" },
            { "hey", "?? Hey! Bovi burada! Bugün ne konuda yardýma ihtiyacýn var? ??" },
            { "bovi", "?? Evet, ben Bovi! Senin en iyi alýþveriþ arkadaþýn! Size nasýl yardýmcý olabilirim? ??" },
            { "kimsin", "?? Ben Bovi! Ýkinci El Market'in sevimli ve yardýmsever asistanýyým! Alýþveriþte her konuda yanýndayým! ?" },
            { "adýn ne", "?? Adým Bovi! Senin alýþveriþ arkadaþýn olmaktan mutluyum! ??" },
            
            // Sipariþ
            { "sipariþ", "?? Sipariþin mi var? Harika! 'Sipariþlerim' sayfasýndan durumunu kontrol edebilirsin! Sipariþ takip numaranla da takip edebilirsin. Ben de merak ediyorum, ne sipariþ ettin? ???" },
            { "kargo", "?? Kargo durumunu 'Sipariþlerim' sayfasýndan takip edebilirsin! Genelde 2-5 iþ günü sürüyor. Heyecanlý bekleyiþ baþlasýn! ????" },
            { "teslimat", "?? Teslimat genelde 2-5 iþ günü içinde kapýnda! Sipariþ detaylarýndan kargo takip numarasýný bulabilirsin. Sabýrsýzlýkla bekliyorsun deðil mi? ????" },
            
            // Ödeme
            { "ödeme", "?? Ödeme konusunda endiþelenme! Kredi kartý, banka kartý veya havale ile güvenle ödeme yapabilirsin. Tüm ödemeler süper güvenli! ????" },
            { "kredi kartý", "?? Visa, Mastercard ve American Express kabul ediyoruz! Ödeme bilgilerin bende güvende, merak etme! ???" },
            { "havale", "?? Havale mi yapacaksýn? Sipariþ sonrasý sana IBAN numarasý göndereceðim! Kolay gelsin! ????" },
            
            // Ýade
            { "iade", "?? Ürün beðenmedin mi? Üzüldüm ama sorun yok! 14 gün içinde 'Sipariþlerim' sayfasýndan iade talebi oluþturabilirsin. Ürün hasarsýz ve kullanýlmamýþ olmalý tabii. ????" },
            { "deðiþim", "?? Deðiþim mi istiyorsun? Harika fikir! Ýade talebinde 'Deðiþim istiyorum' seçeneðini iþaretle, halledelim! ???" },
            
            // Hesap
            { "þifre", "?? Þifreni mi unuttun? Olur böyle þeyler! 'Þifremi Unuttum' linkine týkla, e-postana sýfýrlama linki göndereyim! ????" },
            { "üyelik", "?? Aramýza katýlmak mý istiyorsun? Harika! 'Kayýt Ol' butonuna týkla, e-posta ve þifre ile hemen üye ol! Hoþ geldin! ????" },
            { "profil", "?? Profil bilgilerini güncellemek mi istiyorsun? 'Profilim' sayfasýna git, istediðin gibi düzenle! ???" },
            
            // Ürün
            { "ürün", "?? Ürün mü arýyorsun? Ana sayfada tonla seçenek var! Kategorilere göre filtreleyebilir, arama yapabilirsin. Ne aramýþtýn? ????" },
            { "fiyat", "?? Fiyatlar satýcýlar tarafýndan belirleniyor. Ama bazý ürünlerde 'Teklif Ver' özelliði var, pazarlýk yapabilirsin! Ýyi alýþveriþler! ????" },
            { "stok", "?? Stok durumunu ürün detay sayfasýnda görebilirsin. Stokta yoksa bildirim alabilirsin, yeniden gelince haber veririz! ????" },
            
            // Ýletiþim
            { "iletiþim", "?? Bizimle iletiþime geçmek mi istiyorsun? 'Ýletiþim' sayfasýndan yazabilirsin! E-posta: destek@ikincielpazar.com ????" },
            { "telefon", "?? Telefon mu etmek istiyorsun? Ýþte numara: 0850 123 45 67 (Hafta içi 09:00-18:00) Ekibimiz seni bekliyor! ????" },
            
            // Satýcý
            { "satýcý", "?? Satýcý olmak mý istiyorsun? Harika! Üye olduktan sonra 'Yeni Ürün Ekle' sayfasýndan ilan verebilirsin. Baþarýlar! ???" },
            { "ilan", "?? Ýlan vermek çok kolay! 'Yeni Ürün Ekle' butonuna týkla, fotoðraf ekle, açýklama yaz, fiyat belirle. Hepsi bu kadar! ????" },
            
            // Teþekkür & Veda
            { "teþekkür", "?? Rica ederim! Yardýmcý olabildiysem çok mutluyum! Baþka bir þey lazým olursa buradayým! ????" },
            { "teþekkürler", "?? Ne demek! Senin için her zaman buradayým! Ýyi alýþveriþler! ????" },
            { "saðol", "?? Estaðfurullah! Yardýmcý olmak benim iþim! Her zaman yanýndayým! ????" },
            { "güle güle", "?? Görüþürüz! Ýyi alýþveriþler dilerim! Tekrar gel ha! ????" },
            { "hoþça kal", "?? Hoþça kal! Seni tekrar görmek için sabýrsýzlanýyorum! ???" },
            { "bay", "?? Baay! Kendine iyi bak! Görüþmek üzere! ????" }
        };

        // ?? Bovi'nin hýzlý cevap önerileri
        private readonly List<List<string>> _hizliCevapSetleri = new()
        {
            new List<string> { "?? Bovi kimdir?", "?? Sipariþ durumu", "?? Kargo takibi" },
            new List<string> { "?? Ödeme seçenekleri", "?? Ýade iþlemi", "?? Ürün deðiþimi" },
            new List<string> { "??? Ürün arama", "?? Satýcý ol", "?? Ýletiþim" },
            new List<string> { "?? Hesap ayarlarý", "?? Þifre sýfýrlama", "? Yardým" }
        };

        public ChatBotServis()
        {
            Console.WriteLine("[?? BOVI] Bovi servisi baþlatýldý! Müþterilere yardým etmeye hazýrým!");
        }

        /// <summary>
        /// ?? Bovi'nin mesaja kiþilikli cevap üretme metodu
        /// </summary>
        public string CevapUret(string mesaj)
        {
            var mesajKucuk = mesaj.ToLowerInvariant().Trim();
            Console.WriteLine($"[?? BOVI] Mesaj alýndý: {mesaj}");

            // Anahtar kelime aramasý
            foreach (var cevap in _cevaplar)
            {
                if (mesajKucuk.Contains(cevap.Key))
                {
                    Console.WriteLine($"[?? BOVI] Anahtar kelime bulundu: {cevap.Key}");
                    return cevap.Value;
                }
            }

            // ?? Bovi'nin özel durumlarý
            if (mesajKucuk.Contains("kaç") && mesajKucuk.Contains("gün"))
            {
                return "?? Kargo teslimat süresi genellikle 2-5 iþ günü! Hýzlý kargo ile 1-2 iþ günü içinde kapýnda! Sabýrsýzlanma, çok yakýnda elinde olacak! ???";
            }

            if (mesajKucuk.Contains("nasýl") && mesajKucuk.Contains("satýn"))
            {
                return "?? Ürün satýn almak çok kolay! Ýþte adýmlar:\n\n1?? Beðendiðin ürünü seç\n2?? 'Sepete Ekle' butonuna týkla\n3?? Sepetini kontrol et\n4?? 'Sipariþi Tamamla' ile ödeme yap\n\nGördün mü? Çok basit! ????";
            }

            if (mesajKucuk.Contains("güvenli") || mesajKucuk.Contains("dolandýrýcý"))
            {
                return "?? Merak etme, platform süper güvenli! Tüm satýcýlar doðrulanýyor ve ödeme bilgilerin þifreleniyor. Þüpheli bir durum olursa hemen bize söyle! ?????";
            }

            if (mesajKucuk.Contains("yardým") || mesajKucuk.Contains("help"))
            {
                return "?? Yardým mý lazým? Tam yerindesin! Ben Bovi, senin için buradayým! Sorunu anlat, birlikte çözelim! ???";
            }

            if (mesajKucuk.Contains("indirim") || mesajKucuk.Contains("kampanya"))
            {
                return "?? Ýndirimler ve kampanyalar için ana sayfayý takip et! Sürekli yeni fýrsatlar ekliyoruz. Gözün kulaðýn açýk olsun! ????";
            }

            if (mesajKucuk.Contains("sevdim") || mesajKucuk.Contains("harika") || mesajKucuk.Contains("süper"))
            {
                return "?? Vay be, çok mutlu oldum! Beðenmene sevindim! Ýyi alýþveriþler! " + RastgeleReaksiyon() + "??";
            }

            // ?? Varsayýlan Bovi cevabý
            Console.WriteLine("[?? BOVI] Varsayýlan cevap döndürülüyor.");
            return "?? Hmm, tam anlayamadým ama sana yardýmcý olmak istiyorum! " + RastgeleReaksiyon() + "\n\n" +
                   "Þu konularda yardýmcý olabilirim:\n" +
                   "?? Sipariþ ve kargo durumu\n" +
                   "?? Ödeme iþlemleri\n" +
                   "?? Ýade ve deðiþim\n" +
                   "?? Hesap yönetimi\n" +
                   "??? Ürün arama ve fiyat\n" +
                   "?? Satýcý olma\n\n" +
                   "Hangi konuda yardýma ihtiyacýn var? Anlat bakalým! ??";
        }

        /// <summary>
        /// ?? Bovi'nin rastgele emoji reaksiyonu
        /// </summary>
        private string RastgeleReaksiyon()
        {
            return _boviReaksiyonlari[_random.Next(_boviReaksiyonlari.Length)];
        }

        /// <summary>
        /// Rastgele hýzlý cevap önerileri döndürür
        /// </summary>
        public List<string> HizliCevaplarGetir()
        {
            var random = new Random();
            var index = random.Next(_hizliCevapSetleri.Count);
            return _hizliCevapSetleri[index];
        }
    }
}
