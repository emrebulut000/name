using Microsoft.EntityFrameworkCore;
using Unı_Proje.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Unı_Proje.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ----- CORS POLICY TANIMI - GÜVENLİK İYİLEŞTİRMESİ -----
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("DevelopmentPolicy",
            policy =>
            {
                policy.WithOrigins("https://localhost:7000", "https://localhost:7130", "http://localhost:5160")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
    }
    else
    {
        options.AddPolicy("ProductionPolicy",
            policy =>
            {
                var allowedOrigin = builder.Configuration["AllowedOrigins:Production"] ?? "https://yourdomain.com";
                policy.WithOrigins(allowedOrigin)
                      .WithMethods("GET", "POST", "PUT", "DELETE")
                      .WithHeaders("Content-Type", "Authorization", "Accept")
                      .AllowCredentials()
                      .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
            });
    }
});

// JSON Serialization ayarları eklendi
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Sonsuz döngü hatasını engeller
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // Büyük/Küçük harf duyarlılığını kaldırır
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddRazorPages();

// Veritabanı Bağlantısı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ProjeDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Email Servisi
builder.Services.AddScoped<Unı_Proje.Services.EmailServis>();

// ChatBot Servisi
builder.Services.AddScoped<Unı_Proje.Services.ChatBotServis>();

// 🌍 Geocoding Servisi (Google Maps API)
builder.Services.AddHttpClient<Unı_Proje.Services.GeocodingService>();
builder.Services.AddScoped<Unı_Proje.Services.GeocodingService>();

// Swagger/OpenAPI (JWT destekli)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "İkinci El Ticaret API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Kimlik Doğrulama (JWT Ayarları) - GÜNCELLENMİŞ
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            ClockSkew = TimeSpan.Zero // Token süre toleransını sıfırla (opsiyonel)
        };

        // DEBUG için event handlers eklendi
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
    });

var app = builder.Build();

// ----- HTTP İSTEK PIPELINE'I -----

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    
    // GÜVENLİK HEADER'LARI (Production) - Duplicate önlenmesi için indexer kullanımı
    app.Use(async (context, next) =>
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        await next();
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(app.Environment.IsDevelopment() ? "DevelopmentPolicy" : "ProductionPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();