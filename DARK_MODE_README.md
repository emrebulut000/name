# ?? Dark Mode Özelliði

Projenize baþarýyla **Dark Mode (Karanlýk Tema)** özelliði eklenmiþtir!

## ?? Eklenen Dosyalar

### 1. **ThemeService.cs** (`Services/ThemeService.cs`)
Tema yönetimini saðlayan servis:
- ? Light/Dark mode geçiþi
- ? LocalStorage'da kullanýcý tercihini kaydetme
- ? Tarayýcý tema tercihini algýlama
- ? Otomatik tema deðiþimi

### 2. **ThemeToggle.razor** (`Components/ThemeToggle.razor`)
Animasyonlu tema deðiþtirme butonu component'i:
- ?? Modern ve þýk tasarým
- ?? Smooth animasyonlar
- ?? Güneþ/Ay ikonlarý ile görsel geri bildirim
- ?? Responsive tasarým

### 3. **app.css** (Güncellenmiþ)
Dark mode için CSS deðiþkenleri ve stiller:
- ?? Tam dark mode renk paleti
- ?? Smooth geçiþ animasyonlarý
- ?? Tüm component'ler için dark mode desteði

## ?? Kullaným

### Otomatik Entegrasyon
Dark mode butonu **TopNavbar**'a otomatik olarak eklenmiþtir. Kullanýcýlar üst menüden kolayca tema deðiþtirebilirler.

### Manuel Kullaným (Ýsteðe Baðlý)
Baþka bir sayfada tema deðiþtirme butonu eklemek isterseniz:

```razor
@page "/ayarlar"
@using Uný_Proje.Web.Client.Components

<h3>Ayarlar</h3>

<div class="mb-3">
    <label class="form-label fw-bold">Tema Seçimi</label>
    <ThemeToggle showLabel="true" />
</div>
```

### Programatik Kullaným
C# kodundan tema deðiþtirmek için:

```csharp
@inject ThemeService ThemeService

@code {
    private async Task DarkModaGec()
    {
        await ThemeService.SetThemeAsync("dark");
    }

    private async Task LightModaGec()
    {
        await ThemeService.SetThemeAsync("light");
    }

    private async Task TemaToggle()
    {
        await ThemeService.ToggleThemeAsync();
    }

    private bool IsDarkMode()
    {
        return ThemeService.IsDarkMode;
    }
}
```

## ?? Özelleþtirme

### Renk Deðiþikliði
Dark mode renklerini özelleþtirmek için `app.css` dosyasýndaki `[data-theme="dark"]` bölümünü düzenleyin:

```css
[data-theme="dark"] {
    --primary-color: #818cf8;  /* Ana renk */
    --gray-900: #f9fafb;       /* Ana metin rengi */
    /* ... diðer renkler */
}
```

### Yeni Component'lere Dark Mode Desteði Eklemek
Yeni oluþturduðunuz component'lere dark mode desteði eklemek için:

```css
/* Light mode stilleri */
.my-component {
    background: white;
    color: #1f2937;
}

/* Dark mode stilleri */
[data-theme="dark"] .my-component {
    background: #1e293b;
    color: #f9fafb;
}
```

## ?? Teknik Detaylar

### Tema Kaydedilmesi
Kullanýcýnýn tema tercihi **LocalStorage**'da `theme` anahtarý altýnda saklanýr:
```javascript
localStorage.setItem('theme', 'dark'); // veya 'light'
```

### Otomatik Algýlama
Eðer kullanýcý daha önce tema seçmemiþse, sistem tarayýcýnýn tercihini algýlar:
```javascript
window.matchMedia('(prefers-color-scheme: dark)').matches
```

### CSS Deðiþkenleri (CSS Variables)
Tüm renkler CSS deðiþkenleri kullanýlarak yönetilir. Bu sayede:
- ? Tek bir yerden tüm renkleri yönetebilirsiniz
- ? Tema deðiþimi anýnda gerçekleþir
- ? Performans optimizasyonu saðlanýr

## ?? Desteklenen Component'ler

Dark mode aþaðýdaki tüm component'lerde otomatik çalýþýr:
- ? Navbar & TopNavbar
- ? Cards (Kartlar)
- ? Forms (Formlar)
- ? Buttons (Butonlar)
- ? Modals (Pop-up'lar)
- ? Alerts (Uyarýlar)
- ? Tables (Tablolar)
- ? Dropdown menus
- ? Badges
- ? Breadcrumbs
- ? Pagination
- ? Footer

## ?? Tarayýcý Desteði

| Tarayýcý | Destek |
|----------|--------|
| Chrome 76+ | ? Tam Destek |
| Firefox 67+ | ? Tam Destek |
| Safari 12.1+ | ? Tam Destek |
| Edge 79+ | ? Tam Destek |
| Opera 63+ | ? Tam Destek |

## ?? Ýpuçlarý

### 1. Smooth Transitions
Tüm element'lere otomatik geçiþ animasyonu eklenmiþtir:
```css
* {
    transition: background-color 0.3s ease, border-color 0.3s ease, color 0.3s ease;
}
```

### 2. Custom Scrollbar
Dark mode'da scrollbar da otomatik olarak karanlýk temaya uyarlanýr.

### 3. Image Brightness
Eðer resimlerin dark mode'da çok parlak görünmesini engellemek isterseniz:
```css
[data-theme="dark"] img {
    filter: brightness(0.8);
    transition: filter 0.3s ease;
}

[data-theme="dark"] img:hover {
    filter: brightness(1);
}
```

## ?? Sorun Giderme

### Tema Deðiþmiyor
1. Tarayýcý console'unu açýn (`F12`)
2. LocalStorage'ý kontrol edin
3. `localStorage.clear()` komutuyla temizleyin
4. Sayfayý yenileyin

### Bazý Elementler Karanlýk Modda Yanlýþ Görünüyor
CSS'te ilgili element için dark mode stili ekleyin:
```css
[data-theme="dark"] .problem-element {
    /* Dark mode stilleri */
}
```

### Performans Sorunu
Eðer tema deðiþimi yavaþsa, bazý transition'larý devre dýþý býrakabilirsiniz:
```css
.no-transition * {
    transition: none !important;
}
```

## ?? Kaynaklar

- [CSS Custom Properties](https://developer.mozilla.org/en-US/docs/Web/CSS/Using_CSS_custom_properties)
- [prefers-color-scheme](https://developer.mozilla.org/en-US/docs/Web/CSS/@media/prefers-color-scheme)
- [LocalStorage API](https://developer.mozilla.org/en-US/docs/Web/API/Window/localStorage)

## ?? Sonuç

Dark mode özelliði baþarýyla entegre edilmiþtir! Kullanýcýlarýnýz artýk göz yormayan karanlýk temayý kullanabilirler. 

**Geliþtirici:** Bulut Yazýlým Ekibi  
**Tarih:** 2025  
**Versiyon:** 1.0.0

---

Herhangi bir sorunuz veya öneriniz varsa lütfen bildirin! ??
