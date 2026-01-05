using System.Threading.Tasks;
using Microsoft.JSInterop;
using System; 

namespace Unı_Proje.Web.Services
{
    public class AuthenticationStateService
    {
        private readonly IJSRuntime _jsRuntime;
        private bool _isLoggedIn = false;
        public event Action? OnChange;

        public AuthenticationStateService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            Console.WriteLine("AuthenticationStateService: Oluşturuldu.");
        }

        public bool IsLoggedIn => _isLoggedIn;

        public async Task InitializeAsync()
        {
            Console.WriteLine("AuthenticationStateService: InitializeAsync Başladı.");
            var token = await GetTokenAsync();
            _isLoggedIn = !string.IsNullOrEmpty(token);
            Console.WriteLine($"AuthenticationStateService: InitializeAsync Bitti. IsLoggedIn = {_isLoggedIn}");
            NotifyStateChanged();
        }

        public async Task LoginAsync(string token)
        {
            Console.WriteLine("AuthenticationStateService: LoginAsync Başladı.");
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
            _isLoggedIn = true;
            Console.WriteLine($"AuthenticationStateService: LoginAsync Bitti. IsLoggedIn = {_isLoggedIn}");
            NotifyStateChanged();
        }

        public async Task LogoutAsync()
        {
            Console.WriteLine("AuthenticationStateService: LogoutAsync Başladı.");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            _isLoggedIn = false;
            Console.WriteLine($"AuthenticationStateService: LogoutAsync Bitti. IsLoggedIn = {_isLoggedIn}");
            NotifyStateChanged();
        }

       
        private async Task<string?> GetTokenAsync()
        {
            Console.WriteLine("AuthenticationStateService: GetTokenAsync Çağrıldı."); // Log ekleyelim
            try
            {
                return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"AuthenticationStateService: GetTokenAsync Hata: {ex.Message}"); 
                
                return null;
            }
        }

        private void NotifyStateChanged()
        {
            Console.WriteLine($"AuthenticationStateService: NotifyStateChanged Çağrıldı. IsLoggedIn = {_isLoggedIn}");
            OnChange?.Invoke();
        }
    }
}