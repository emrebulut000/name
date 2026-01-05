using Unı_Proje.Web.Controllers;
using System.Text.Json.Serialization;
using Unı_Proje.Web.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Unı_Proje.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Unı_Proje.Data;
using Unı_Proje.Web.Client.Pages; // Login sayfasını tanıması için


var builder = WebApplication.CreateBuilder(args);

// ==============================================================================
// 1. SERVİSLERİN EKLENMESİ
// ==============================================================================

// 👇 EN KRİTİK KISIM BURASI 👇
// "AddApplicationPart" komutu, sunucuya ResimController'ın nerede olduğunu parmakla gösterir.
builder.Services.AddControllers()
    .AddApplicationPart(typeof(ResimController).Assembly)
    .AddJsonOptions(options =>
    {
        // Sonsuz döngü hatasını engeller
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // Büyük/Küçük harf duyarlılığını kaldırır (Url vs url)
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// CORS Ayarı - GÜVENLİK İYİLEŞTİRMESİ
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Development: Daha esnek ama kontrollü
        options.AddPolicy("DevelopmentPolicy",
            policy =>
            {
                var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins:Development").Get<string[]>() 
                    ?? new[] { "https://localhost:7000", "https://localhost:7130" };
                
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
    }
    else
    {
        // Production: Sıkı güvenlik
        options.AddPolicy("ProductionPolicy",
            policy =>
            {
                var allowedOrigin = builder.Configuration["AllowedOrigins:Production"] 
                    ?? "https://yourdomain.com";
                
                policy.WithOrigins(allowedOrigin)
                      .WithMethods("GET", "POST", "PUT", "DELETE")
                      .WithHeaders("Content-Type", "Authorization", "Accept")
                      .AllowCredentials()
                      .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
            });
    }
});

// Razor Bileşenleri
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// HttpClient
builder.Services.AddScoped(sp => new HttpClient());

// API Configuration Service (Client tarafında kullanılıyor)
builder.Services.AddScoped<Unı_Proje.Web.Client.Services.ApiConfigService>();

// ChatBot Service (Client tarafında kullanılıyor)
builder.Services.AddScoped<Unı_Proje.Web.Client.Services.ChatBotServis>();

// ChatBot Service (Server tarafında kullanılıyor - API Controller için)
builder.Services.AddScoped<Unı_Proje.Services.ChatBotServis>();

// Veritabanı Bağlantısı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ProjeDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Auth Servisleri (Sunucu Tarafı)
builder.Services.AddScoped<AuthenticationStateProvider, DummyAuthStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();

// JWT Authentication (API için)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"OnChallenge error: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
})
.AddCookie(); // Blazor sayfaları için cookie authentication

var app = builder.Build();

// ==============================================================================
// 2. MIDDLEWARE (UYGULAMA AKIŞI) AYARLARI
// ==============================================================================

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    // Development CORS policy
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    
    // GÜVENLİK HEADER'LARI (Production)
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
        await next();
    });
}

app.UseHttpsRedirection();

// 👇 BU ÇOK ÖNEMLİ: Resimlerin tarayıcıda açılabilmesi için şart
app.UseStaticFiles();

app.UseRouting();

// CORS'u aktif et - Environment'a göre farklı policy
app.UseCors(app.Environment.IsDevelopment() ? "DevelopmentPolicy" : "ProductionPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// API Controller'larını Haritala
app.MapControllers();

// Blazor Sayfalarını Haritala
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Login).Assembly);

app.Run();