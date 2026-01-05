using Microsoft.JSInterop;

namespace Uný_Proje.Web.Client.Services
{
    /// <summary>
    /// Dark Mode / Light Mode tema yönetim servisi
    /// </summary>
    public class ThemeService
    {
        private readonly IJSRuntime _js;
        private string _currentTheme = "light";
        private bool _isInitialized = false;
        private readonly object _initLock = new object();

        public event Action? ThemeChanged;

        public ThemeService(IJSRuntime js)
        {
            _js = js;
            Console.WriteLine("[THEME SERVICE] ?? ThemeService oluþturuldu");
        }
        
        /// <summary>
        /// Mevcut temayý döndürür
        /// </summary>
        public string CurrentTheme => _currentTheme;

        /// <summary>
        /// Dark mode aktif mi?
        /// </summary>
        public bool IsDarkMode => _currentTheme == "dark";

        /// <summary>
        /// Sayfa ilk yüklendiðinde LocalStorage'dan temayý yükler
        /// </summary>
        public async Task InitializeThemeAsync()
        {
            // Thread-safe double-check locking pattern
            if (_isInitialized)
            {
                Console.WriteLine("[THEME SERVICE] ?? Tema zaten baþlatýlmýþ, atlýyorum");
                return;
            }

            lock (_initLock)
            {
                if (_isInitialized)
                {
                    Console.WriteLine("[THEME SERVICE] ?? Tema zaten baþlatýlmýþ (lock içinde), atlýyorum");
                    return;
                }
                
                _isInitialized = true;
            }

            try
            {
                Console.WriteLine("[THEME SERVICE] ?? InitializeThemeAsync çaðrýldý");
                
                // JavaScript themeManager'dan temayý oku (TEK KAYNAK)
                var savedTheme = await _js.InvokeAsync<string?>("eval", "window.themeManager?.get() || localStorage.getItem('theme') || 'light'");
                Console.WriteLine($"[THEME SERVICE] ?? Tema okundu: {savedTheme}");

                _currentTheme = savedTheme ?? "light";
                Console.WriteLine($"[THEME SERVICE] ? Tema yüklendi: {_currentTheme}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[THEME SERVICE] ? Tema yükleme hatasý: {ex.Message}");
                lock (_initLock)
                {
                    _isInitialized = false;
                }
                _currentTheme = "light";
            }
        }

        /// <summary>
        /// Tarayýcýdan mevcut temayý okur (senkron olarak state'i günceller)
        /// </summary>
        public async Task<string> GetCurrentThemeAsync()
        {
            try
            {
                var browserTheme = await _js.InvokeAsync<string?>("eval", "window.themeManager?.get() || localStorage.getItem('theme') || 'light'");
                
                if (browserTheme != null && (browserTheme == "dark" || browserTheme == "light"))
                {
                    if (_currentTheme != browserTheme)
                    {
                        Console.WriteLine($"[THEME SERVICE] ?? State güncelleniyor: {_currentTheme} ? {browserTheme}");
                        _currentTheme = browserTheme;
                    }
                    return browserTheme;
                }
                
                return "light";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[THEME SERVICE] ? Tema okuma hatasý: {ex.Message}");
                return "light";
            }
        }

        /// <summary>
        /// Temayý deðiþtirir (light <-> dark)
        /// </summary>
        public async Task ToggleThemeAsync()
        {
            var newTheme = _currentTheme == "light" ? "dark" : "light";
            await SetThemeAsync(newTheme);
        }

        /// <summary>
        /// Belirli bir temayý ayarlar
        /// </summary>
        public async Task SetThemeAsync(string theme)
        {
            if (theme != "light" && theme != "dark")
            {
                Console.WriteLine($"[THEME SERVICE] ?? Geçersiz tema: {theme}");
                return;
            }

            Console.WriteLine($"[THEME SERVICE] ?? Tema deðiþtiriliyor: {_currentTheme} -> {theme}");
            _currentTheme = theme;

            try
            {
                // JavaScript themeManager helper'ýný kullan (TEK KAYNAK)
                var success = await _js.InvokeAsync<bool>("eval", $"window.themeManager?.set('{theme}') || false");
                
                if (success)
                {
                    Console.WriteLine($"[THEME SERVICE] ? Tema baþarýyla ayarlandý: {theme}");
                }
                else
                {
                    Console.WriteLine($"[THEME SERVICE] ?? themeManager bulunamadý, fallback kullanýlýyor");
                    await _js.InvokeVoidAsync("localStorage.setItem", "theme", theme);
                }

                // Event'i tetikle (UI güncellensin)
                ThemeChanged?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[THEME SERVICE] ? Tema kaydetme hatasý: {ex.Message}");
            }
        }
    }
}
