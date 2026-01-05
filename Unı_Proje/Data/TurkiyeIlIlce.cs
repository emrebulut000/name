using System.Collections.Generic;
using System.Linq;

namespace Uný_Proje.Data
{
    public static class TurkiyeIlIlce
    {
        public static Dictionary<string, List<string>> IlIlceListesi = new Dictionary<string, List<string>>
        {
            { "Adana", new List<string> { "Aladað", "Ceyhan", "Çukurova", "Feke", "Ýmamoðlu", "Karaisalý", "Karataþ", "Kozan", "Pozantý", "Saimbeyli", "Sarýçam", "Seyhan", "Tufanbeyli", "Yumurtalýk", "Yüreðir" } },
            { "Adýyaman", new List<string> { "Besni", "Çelikhan", "Gerger", "Gölbaþý", "Kahta", "Merkez", "Samsat", "Sincik", "Tut" } },
            { "Afyonkarahisar", new List<string> { "Baþmakçý", "Bayat", "Bolvadin", "Çay", "Çobanlar", "Dazkýrý", "Dinar", "Emirdað", "Evciler", "Hocalar", "Ýhsaniye", "Ýscehisar", "Kýzýlören", "Merkez", "Sandýklý", "Sinanpaþa", "Sultandaðý", "Þuhut" } },
            { "Aðrý", new List<string> { "Diyadin", "Doðubayazýt", "Eleþkirt", "Hamur", "Merkez", "Patnos", "Taþlýçay", "Tutak" } },
            { "Aksaray", new List<string> { "Aðaçören", "Eskil", "Gülaðaç", "Güzelyurt", "Merkez", "Ortaköy", "Sarýyahþi", "Sultanhaný" } },
            { "Amasya", new List<string> { "Göynücek", "Gümüþhacýköy", "Hamamözü", "Merkez", "Merzifon", "Suluova", "Taþova" } },
            { "Ankara", new List<string> { "Akyurt", "Altýndað", "Ayaþ", "Bala", "Beypazarý", "Çamlýdere", "Çankaya", "Çubuk", "Elmadað", "Etimesgut", "Evren", "Gölbaþý", "Güdül", "Haymana", "Kalecik", "Kahramankazan", "Keçiören", "Kýzýlcahamam", "Mamak", "Nallýhan", "Polatlý", "Pursaklar", "Sincan", "Þereflikoçhisar", "Yenimahalle" } },
            { "Antalya", new List<string> { "Akseki", "Aksu", "Alanya", "Demre", "Döþemealtý", "Elmalý", "Finike", "Gazipaþa", "Gündoðmuþ", "Ýbradý", "Kaþ", "Kemer", "Kepez", "Konyaaltý", "Korkuteli", "Kumluca", "Manavgat", "Muratpaþa", "Serik" } },
            { "Ardahan", new List<string> { "Çýldýr", "Damal", "Göle", "Hanak", "Merkez", "Posof" } },
            { "Artvin", new List<string> { "Ardanuç", "Arhavi", "Borçka", "Hopa", "Merkez", "Murgul", "Þavþat", "Yusufeli" } },
            { "Aydýn", new List<string> { "Bozdoðan", "Buharkent", "Çine", "Didim", "Efeler", "Germencik", "Ýncirliova", "Karacasu", "Karpuzlu", "Koçarlý", "Köþk", "Kuþadasý", "Kuyucak", "Nazilli", "Söke", "Sultanhisar", "Yenipazar" } },
            { "Balýkesir", new List<string> { "Altýeylül", "Ayvalýk", "Balya", "Bandýrma", "Bigadiç", "Burhaniye", "Dursunbey", "Edremit", "Erdek", "Gömeç", "Gönen", "Havran", "Ývrindi", "Karesi", "Kepsut", "Manyas", "Marmara", "Savaþtepe", "Sýndýrgý", "Susurluk" } },
            { "Bartýn", new List<string> { "Amasra", "Kurucaþile", "Merkez", "Ulus" } },
            { "Batman", new List<string> { "Beþiri", "Gercüþ", "Hasankeyf", "Kozluk", "Merkez", "Sason" } },
            { "Bayburt", new List<string> { "Aydýntepe", "Demirözü", "Merkez" } },
            { "Bilecik", new List<string> { "Bozüyük", "Gölpazarý", "Ýnhisar", "Merkez", "Osmaneli", "Pazaryeri", "Söðüt", "Yenipazar" } },
            { "Bingöl", new List<string> { "Adaklý", "Genç", "Karlýova", "Kiðý", "Merkez", "Solhan", "Yayladere", "Yedisu" } },
            { "Bitlis", new List<string> { "Adilcevaz", "Ahlat", "Güroymak", "Hizan", "Merkez", "Mutki", "Tatvan" } },
            { "Bolu", new List<string> { "Dörtdivan", "Gerede", "Göynük", "Kýbrýscýk", "Mengen", "Merkez", "Mudurnu", "Seben", "Yeniçaða" } },
            { "Burdur", new List<string> { "Aðlasun", "Altýnyayla", "Bucak", "Çavdýr", "Çeltikçi", "Gölhisar", "Karamanlý", "Kemer", "Merkez", "Tefenni", "Yeþilova" } },
            { "Bursa", new List<string> { "Büyükorhan", "Gemlik", "Gürsu", "Harmancýk", "Ýnegöl", "Ýznik", "Karacabey", "Keles", "Kestel", "Mudanya", "Mustafakemalpaþa", "Nilüfer", "Orhaneli", "Orhangazi", "Osmangazi", "Yeniþehir", "Yýldýrým" } },
            { "Çanakkale", new List<string> { "Ayvacýk", "Bayramiç", "Biga", "Bozcaada", "Çan", "Eceabat", "Ezine", "Gelibolu", "Gökçeada", "Lapseki", "Merkez", "Yenice" } },
            { "Çankýrý", new List<string> { "Atkaracalar", "Bayramören", "Çerkeþ", "Eldivan", "Ilgaz", "Kýzýlýrmak", "Korgun", "Kurþunlu", "Merkez", "Orta", "Þabanözü", "Yapraklý" } },
            { "Çorum", new List<string> { "Alaca", "Bayat", "Boðazkale", "Dodurga", "Ýskilip", "Kargý", "Laçin", "Mecitözü", "Merkez", "Oðuzlar", "Ortaköy", "Osmancýk", "Sungurlu", "Uðurludað" } },
            { "Denizli", new List<string> { "Acýpayam", "Babadað", "Baklan", "Bekilli", "Beyaðaç", "Bozkurt", "Buldan", "Çal", "Çameli", "Çardak", "Çivril", "Güney", "Honaz", "Kale", "Merkezefendi", "Pamukkale", "Sarayköy", "Serinhisar", "Tavas" } },
            { "Diyarbakýr", new List<string> { "Baðlar", "Bismil", "Çermik", "Çýnar", "Dicle", "Eðil", "Ergani", "Hani", "Hazro", "Kayapýnar", "Kocaköy", "Kulp", "Lice", "Silvan", "Sur", "Yeniþehir" } },
            { "Düzce", new List<string> { "Akçakoca", "Cumayeri", "Çilimli", "Gölyaka", "Gümüþova", "Kaynaþlý", "Merkez", "Yýðýlca" } },
            { "Edirne", new List<string> { "Enez", "Havsa", "Ýpsala", "Keþan", "Lalapaþa", "Meriç", "Merkez", "Süloðlu", "Uzunköprü" } },
            { "Elazýð", new List<string> { "Aðýn", "Alacakaya", "Arýcak", "Baskil", "Karakoçan", "Keban", "Kovancýlar", "Maden", "Merkez", "Palu", "Sivrice" } },
            { "Erzincan", new List<string> { "Çayýrlý", "Ýliç", "Kemah", "Kemaliye", "Merkez", "Otlukbeli", "Refahiye", "Tercan", "Üzümlü" } },
            { "Erzurum", new List<string> { "Aþkale", "Aziziye", "Çat", "Hýnýs", "Horasan", "Ýspir", "Karaçoban", "Karayazý", "Köprüköy", "Narman", "Oltu", "Olur", "Palandöken", "Pasinler", "Pazaryolu", "Þenkaya", "Tekman", "Tortum", "Uzundere", "Yakutiye" } },
            { "Eskiþehir", new List<string> { "Alpu", "Beylikova", "Çifteler", "Günyüzü", "Han", "Ýnönü", "Mahmudiye", "Mihalgazi", "Mihalýççýk", "Odunpazarý", "Sarýcakaya", "Seyitgazi", "Sivrihisar", "Tepebaþý" } },
            { "Gaziantep", new List<string> { "Araban", "Ýslahiye", "Karkamýþ", "Nizip", "Nurdaðý", "Oðuzeli", "Þahinbey", "Þehitkamil", "Yavuzeli" } },
            { "Giresun", new List<string> { "Alucra", "Bulancak", "Çamoluk", "Çanakçý", "Dereli", "Doðankent", "Espiye", "Eynesil", "Görele", "Güce", "Keþap", "Merkez", "Piraziz", "Þebinkarahisar", "Tirebolu", "Yaðlýdere" } },
            { "Gümüþhane", new List<string> { "Kelkit", "Köse", "Kürtün", "Merkez", "Þiran", "Torul" } },
            { "Hakkari", new List<string> { "Çukurca", "Merkez", "Þemdinli", "Yüksekova" } },
            { "Hatay", new List<string> { "Altýnözü", "Antakya", "Arsuz", "Belen", "Defne", "Dörtyol", "Erzin", "Hassa", "Ýskenderun", "Kýrýkhan", "Kumlu", "Payas", "Reyhanlý", "Samandað", "Yayladaðý" } },
            { "Iðdýr", new List<string> { "Aralýk", "Karakoyunlu", "Merkez", "Tuzluca" } },
            { "Isparta", new List<string> { "Aksu", "Atabey", "Eðirdir", "Gelendost", "Gönen", "Keçiborlu", "Merkez", "Senirkent", "Sütçüler", "Þarkikaraaðaç", "Uluborlu", "Yalvaç", "Yeniþarbademli" } },
            { "Ýstanbul", new List<string> { "Adalar", "Arnavutköy", "Ataþehir", "Avcýlar", "Baðcýlar", "Bahçelievler", "Bakýrköy", "Baþakþehir", "Bayrampaþa", "Beþiktaþ", "Beykoz", "Beylikdüzü", "Beyoðlu", "Büyükçekmece", "Çatalca", "Çekmeköy", "Esenler", "Esenyurt", "Eyüpsultan", "Fatih", "Gaziosmanpaþa", "Güngören", "Kadýköy", "Kaðýthane", "Kartal", "Küçükçekmece", "Maltepe", "Pendik", "Sancaktepe", "Sarýyer", "Silivri", "Sultanbeyli", "Sultangazi", "Þile", "Þiþli", "Tuzla", "Ümraniye", "Üsküdar", "Zeytinburnu" } },
            { "Ýzmir", new List<string> { "Aliaða", "Balçova", "Bayýndýr", "Bayraklý", "Bergama", "Beydað", "Bornova", "Buca", "Çeþme", "Çiðli", "Dikili", "Foça", "Gaziemir", "Güzelbahçe", "Karabaðlar", "Karaburun", "Karþýyaka", "Kemalpaþa", "Kýnýk", "Kiraz", "Konak", "Menderes", "Menemen", "Narlýdere", "Ödemiþ", "Seferihisar", "Selçuk", "Tire", "Torbalý", "Urla" } },
            { "Kahramanmaraþ", new List<string> { "Afþin", "Andýrýn", "Çaðlayancerit", "Dulkadiroðlu", "Ekinözü", "Elbistan", "Göksun", "Nurhak", "Onikiþubat", "Pazarcýk", "Türkoðlu" } },
            { "Karabük", new List<string> { "Eflani", "Eskipazar", "Merkez", "Ovacýk", "Safranbolu", "Yenice" } },
            { "Karaman", new List<string> { "Ayrancý", "Baþyayla", "Ermenek", "Kazýmkarabekir", "Merkez", "Sarýveliler" } },
            { "Kars", new List<string> { "Akyaka", "Arpaçay", "Digor", "Kaðýzman", "Merkez", "Sarýkamýþ", "Selim", "Susuz" } },
            { "Kastamonu", new List<string> { "Abana", "Aðlý", "Araç", "Azdavay", "Bozkurt", "Cide", "Çatalzeytin", "Daday", "Devrekani", "Doðanyurt", "Hanönü", "Ýhsangazi", "Ýnebolu", "Küre", "Merkez", "Pýnarbaþý", "Seydiler", "Þenpazar", "Taþköprü", "Tosya" } },
            { "Kayseri", new List<string> { "Akkýþla", "Bünyan", "Develi", "Felahiye", "Hacýlar", "Ýncesu", "Kocasinan", "Melikgazi", "Özvatan", "Pýnarbaþý", "Sarýoðlan", "Sarýz", "Talas", "Tomarza", "Yahyalý", "Yeþilhisar" } },
            { "Kýrýkkale", new List<string> { "Bahþýlý", "Balýþeyh", "Çelebi", "Delice", "Karakeçili", "Keskin", "Merkez", "Sulakyurt", "Yahþihan" } },
            { "Kýrklareli", new List<string> { "Babaeski", "Demirköy", "Kofçaz", "Lüleburgaz", "Merkez", "Pehlivanköy", "Pýnarhisar", "Vize" } },
            { "Kýrþehir", new List<string> { "Akçakent", "Akpýnar", "Boztepe", "Çiçekdaðý", "Kaman", "Merkez", "Mucur" } },
            { "Kilis", new List<string> { "Elbeyli", "Merkez", "Musabeyli", "Polateli" } },
            { "Kocaeli", new List<string> { "Baþiskele", "Çayýrova", "Darýca", "Derince", "Dilovasý", "Gebze", "Gölcük", "Ýzmit", "Kandýra", "Karamürsel", "Kartepe", "Körfez" } },
            { "Konya", new List<string> { "Ahýrlý", "Akören", "Akþehir", "Altýnekin", "Beyþehir", "Bozkýr", "Cihanbeyli", "Çeltik", "Çumra", "Derbent", "Derebucak", "Doðanhisar", "Emirgazi", "Ereðli", "Güneysýnýr", "Hadim", "Halkapýnar", "Hüyük", "Ilgýn", "Kadýnhaný", "Karapýnar", "Karatay", "Kulu", "Meram", "Sarayönü", "Selçuklu", "Seydiþehir", "Taþkent", "Tuzlukçu", "Yalýhüyük", "Yunak" } },
            { "Kütahya", new List<string> { "Altýntaþ", "Aslanapa", "Çavdarhisar", "Domaniç", "Dumlupýnar", "Emet", "Gediz", "Hisarcýk", "Merkez", "Pazarlar", "Simav", "Þaphane", "Tavþanlý" } },
            { "Malatya", new List<string> { "Akçadað", "Arapgir", "Arguvan", "Battalgazi", "Darende", "Doðanþehir", "Doðanyol", "Hekimhan", "Kale", "Kuluncak", "Pütürge", "Yazýhan", "Yeþilyurt" } },
            { "Manisa", new List<string> { "Ahmetli", "Akhisar", "Alaþehir", "Demirci", "Gölmarmara", "Gördes", "Kýrkaðaç", "Köprübaþý", "Kula", "Salihli", "Sarýgöl", "Saruhanlý", "Selendi", "Soma", "Þehzadeler", "Turgutlu", "Yunusemre" } },
            { "Mardin", new List<string> { "Artuklu", "Dargeçit", "Derik", "Kýzýltepe", "Mazýdaðý", "Midyat", "Nusaybin", "Ömerli", "Savur", "Yeþilli" } },
            { "Mersin", new List<string> { "Akdeniz", "Anamur", "Aydýncýk", "Bozyazý", "Çamlýyayla", "Erdemli", "Gülnar", "Mezitli", "Mut", "Silifke", "Tarsus", "Toroslar", "Yeniþehir" } },
            { "Muðla", new List<string> { "Bodrum", "Dalaman", "Datça", "Fethiye", "Kavaklýdere", "Köyceðiz", "Marmaris", "Menteþe", "Milas", "Ortaca", "Seydikemer", "Ula", "Yataðan" } },
            { "Muþ", new List<string> { "Bulanýk", "Hasköy", "Korkut", "Malazgirt", "Merkez", "Varto" } },
            { "Nevþehir", new List<string> { "Acýgöl", "Avanos", "Derinkuyu", "Gülþehir", "Hacýbektaþ", "Kozaklý", "Merkez", "Ürgüp" } },
            { "Niðde", new List<string> { "Altunhisar", "Bor", "Çamardý", "Çiftlik", "Merkez", "Ulukýþla" } },
            { "Ordu", new List<string> { "Akkuþ", "Altýnordu", "Aybastý", "Çamaþ", "Çatalpýnar", "Çaybaþý", "Fatsa", "Gölköy", "Gülyalý", "Gürgentepe", "Ýkizce", "Kabadüz", "Kabataþ", "Korgan", "Kumru", "Mesudiye", "Perþembe", "Ulubey", "Ünye" } },
            { "Osmaniye", new List<string> { "Bahçe", "Düziçi", "Hasanbeyli", "Kadirli", "Merkez", "Sumbas", "Toprakkale" } },
            { "Rize", new List<string> { "Ardeþen", "Çamlýhemþin", "Çayeli", "Derepazarý", "Fýndýklý", "Güneysu", "Hemþin", "Ýkizdere", "Ýyidere", "Kalkandere", "Merkez", "Pazar" } },
            { "Sakarya", new List<string> { "Adapazarý", "Akyazý", "Arifiye", "Erenler", "Ferizli", "Geyve", "Hendek", "Karapürçek", "Karasu", "Kaynarca", "Kocaali", "Pamukova", "Sapanca", "Serdivan", "Söðütlü", "Taraklý" } },
            { "Samsun", new List<string> { "Alaçam", "Asarcýk", "Atakum", "Ayvacýk", "Bafra", "Canik", "Çarþamba", "Havza", "Ýlkadým", "Kavak", "Ladik", "Ondokuzmayýs", "Salýpazarý", "Tekkeköy", "Terme", "Vezirköprü", "Yakakent" } },
            { "Siirt", new List<string> { "Baykan", "Eruh", "Kurtalan", "Merkez", "Pervari", "Þirvan", "Tillo" } },
            { "Sinop", new List<string> { "Ayancýk", "Boyabat", "Dikmen", "Duraðan", "Erfelek", "Gerze", "Merkez", "Saraydüzü", "Türkeli" } },
            { "Sivas", new List<string> { "Akýncýlar", "Altýnyayla", "Divriði", "Doðanþar", "Gemerek", "Gölova", "Gürün", "Hafik", "Ýmranlý", "Kangal", "Koyulhisar", "Merkez", "Suþehri", "Þarkýþla", "Ulaþ", "Yýldýzeli", "Zara" } },
            { "Þanlýurfa", new List<string> { "Akçakale", "Birecik", "Bozova", "Ceylanpýnar", "Eyyübiye", "Halfeti", "Haliliye", "Harran", "Hilvan", "Karaköprü", "Siverek", "Suruç", "Viranþehir" } },
            { "Þýrnak", new List<string> { "Beytüþþebap", "Cizre", "Güçlükonak", "Ýdil", "Merkez", "Silopi", "Uludere" } },
            { "Tekirdað", new List<string> { "Çerkezköy", "Çorlu", "Ergene", "Hayrabolu", "Kapaklý", "Malkara", "Marmaraereðlisi", "Muratlý", "Saray", "Süleymanpaþa", "Þarköy" } },
            { "Tokat", new List<string> { "Almus", "Artova", "Baþçiftlik", "Erbaa", "Merkez", "Niksar", "Pazar", "Reþadiye", "Sulusaray", "Turhal", "Yeþilyurt", "Zile" } },
            { "Trabzon", new List<string> { "Akçaabat", "Araklý", "Arsin", "Beþikdüzü", "Çarþýbaþý", "Çaykara", "Dernekpazarý", "Düzköy", "Hayrat", "Köprübaþý", "Maçka", "Of", "Ortahisar", "Þalpazarý", "Sürmene", "Tonya", "Vakfýkebir", "Yomra" } },
            { "Tunceli", new List<string> { "Çemiþgezek", "Hozat", "Mazgirt", "Merkez", "Nazýmiye", "Ovacýk", "Pertek", "Pülümür" } },
            { "Uþak", new List<string> { "Banaz", "Eþme", "Karahallý", "Merkez", "Sivaslý", "Ulubey" } },
            { "Van", new List<string> { "Bahçesaray", "Baþkale", "Çaldýran", "Çatak", "Edremit", "Erciþ", "Gevaþ", "Gürpýnar", "Ýpekyolu", "Muradiye", "Özalp", "Saray", "Tuþba" } },
            { "Yalova", new List<string> { "Altýnova", "Armutlu", "Çiftlikköy", "Çýnarcýk", "Merkez", "Termal" } },
            { "Yozgat", new List<string> { "Akdaðmadeni", "Aydýncýk", "Boðazlýyan", "Çandýr", "Çayýralan", "Çekerek", "Kadýþehri", "Merkez", "Saraykent", "Sarýkaya", "Sorgun", "Þefaatli", "Yenifakýlý", "Yerköy" } },
            { "Zonguldak", new List<string> { "Alaplý", "Çaycuma", "Devrek", "Ereðli", "Gökçebey", "Kilimli", "Kozlu", "Merkez" } }
        };

        public static List<string> Iller => IlIlceListesi.Keys.OrderBy(x => x).ToList();

        public static List<string> GetIlceler(string il)
        {
            if (string.IsNullOrWhiteSpace(il))
                return new List<string>();

            return IlIlceListesi.ContainsKey(il) 
                ? IlIlceListesi[il].OrderBy(x => x).ToList() 
                : new List<string>();
        }
    }
}
