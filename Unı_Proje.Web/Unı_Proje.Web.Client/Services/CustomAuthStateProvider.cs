using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Collections.Generic; // List için
using System; // Convert için

namespace Unı_Proje.Web.Client.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly HttpClient _httpClient;
        // Kullanıcının kimlik durumunu hafızada (in-memory) tutacağız
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthStateProvider(IJSRuntime jsRuntime, HttpClient httpClient)
        {
            _jsRuntime = jsRuntime;
            _httpClient = httpClient;
        }

        // Blazor bu metodu çağırarak mevcut kimlik doğrulama durumunu sorar
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Eğer kullanıcı zaten hafızada varsa (login olmuşsa), tekrar localStorage'a sorma
            if (_currentUser.Identity != null && _currentUser.Identity.IsAuthenticated)
            {
                return new AuthenticationState(_currentUser);
            }

            // Hafızada yoksa (örn. sayfa yeni yüklendi), localStorage'dan almayı dene
            string? token = null;
            try
            {
                token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            }
            catch (Exception ex)
            {
                // Prerendering sırasında JS Interop çalışmaz, bu hata beklenir.
                Console.WriteLine($"Token okuma hatası (muhtemelen prerendering): {ex.Message}");
                // Hata durumunda (veya prerendering sırasında) anonim kullanıcı döndür
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            if (string.IsNullOrEmpty(token))
            {
                // Token yoksa, kullanıcı giriş yapmamıştır (anonimdir)
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Token bulundu, parse et ve hafızaya al
            try
            {
                var user = GetClaimsPrincipalFromToken(token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _currentUser = user;
                return new AuthenticationState(_currentUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token parse etme hatası: {ex.Message}");
                // Token geçersizse, sil ve anonim kullanıcı döndür
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
                _httpClient.DefaultRequestHeaders.Authorization = null;
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        // Login.razor bu metodu çağıracak
        public async Task MarkUserAsAuthenticated(string token)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
            var user = GetClaimsPrincipalFromToken(token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _currentUser = user;

            // Durum değişikliğini Blazor'a bildir (TÜM bileşenlerin güncellenmesi için)
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        // NavMenu.razor bu metodu çağıracak
        public async Task MarkUserAsLoggedOut()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

            // Durum değişikliğini Blazor'a bildir
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        // --- Helper Metotlar ---
        private ClaimsPrincipal GetClaimsPrincipalFromToken(string jwt)
        {
            try
            {
                var claims = ParseClaimsFromJwt(jwt);
                var identity = new ClaimsIdentity(claims, "jwtAuthType");
                return new ClaimsPrincipal(identity);
            }
            catch
            {
                return new ClaimsPrincipal(new ClaimsIdentity());
            }
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                // Gönderdiğiniz Payload'a göre anahtarları ("nameid", "unique_name", "email")
                // doğrudan burada kullanıyoruz.

                // 1. Kullanıcı ID'si için "nameid" anahtarını ara
                keyValuePairs.TryGetValue("nameid", out object? id);
                if (id != null)
                {
                    // Bu claim, context.User.Identity.FindFirst(ClaimTypes.NameIdentifier) ile ID'yi almanızı sağlar
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, id.ToString()!));
                }

                // 2. Kullanıcı Adı için "unique_name" anahtarını ara
                // (Payload'unuzda "name" yok, "unique_name" var)
                keyValuePairs.TryGetValue("unique_name", out object? name);
                if (name != null)
                {
                    // Bu claim, context.User.Identity.Name ile kullanıcı adını almanızı sağlar
                    claims.Add(new Claim(ClaimTypes.Name, name.ToString()!));
                }

                // 3. E-posta için "email" anahtarını ara
                keyValuePairs.TryGetValue("email", out object? email);
                if (email != null)
                {
                    // Bu claim, context.User.Identity.FindFirst(ClaimTypes.Email) ile e-postayı almanızı sağlar
                    claims.Add(new Claim(ClaimTypes.Email, email.ToString()!));
                }
            }
            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}