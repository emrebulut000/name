using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Uný_Proje.Web.Client.Services;
using Uný_Proje.Web.Client.Handlers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// ----- HTTP CLIENT YAPISI (TOKEN INJECTION ÝLE) -----

// 1. AuthTokenHandler'ý servis olarak kaydet
builder.Services.AddScoped<AuthTokenHandler>();

// 2. HttpClient'ý AuthTokenHandler ile yapýlandýr
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthTokenHandler>();
    handler.InnerHandler = new HttpClientHandler();

    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    };

    return httpClient;
});

// ----- ÝSTEMCÝ (CLIENT) KÝMLÝK DOÐRULAMA SERVÝSLERÝ -----

// 1. Temel yetkilendirme çekirdeði
builder.Services.AddAuthorizationCore();

// 2. KRÝTÝK EKSÝK: <AuthorizeView> bileþenlerinin çalýþmasý için bu servis ÞARTTIR.
// Bu satýr olmadan menüler güncellenmez veya boþ görünür.
builder.Services.AddCascadingAuthenticationState();

// 3. Kendi özel AuthStateProvider'ýmýzý sisteme tanýtýyoruz.
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// 4. Sayfalarda direkt @inject CustomAuthStateProvider diyebilmek için (örn: Logout sayfasý)
builder.Services.AddScoped<CustomAuthStateProvider>();

// 5. Profil güncelleme servisi
builder.Services.AddScoped<ProfilServis>();

// 6. Sepet servisi
builder.Services.AddScoped<SepetServis>();

// 7. Favori servisi
builder.Services.AddScoped<FavoriServis>();

// 8. Mesaj servisi
builder.Services.AddScoped<MesajServis>();

// 9. Admin servisi
builder.Services.AddScoped<AdminServis>();

// 10. API Configuration servisi
builder.Services.AddScoped<ApiConfigService>();

// 11. Sipariþ servisi
builder.Services.AddScoped<SiparisServis>();

// 12. Deðerlendirme servisi
builder.Services.AddScoped<DegerlendirmeServis>();

// 13. Bildirim servisi
builder.Services.AddScoped<BildirimServis>();

// 14. Teklif servisi
builder.Services.AddScoped<TeklifServis>();

// 15. Theme (Dark Mode) servisi - Singleton yapýldý (tema tercihi korunmalý)
// NavigationManager'ý otomatik inject eder (optional dependency)
builder.Services.AddSingleton<ThemeService>();

// 16. ChatBot servisi
builder.Services.AddScoped<ChatBotServis>();

// ----- Bitti -----

await builder.Build().RunAsync();