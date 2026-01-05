using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace Uný_Proje.Web.Client.Handlers
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly IJSRuntime _jsRuntime;

        public AuthTokenHandler(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Her istekten önce token'ý localStorage'dan al
                var token = await _jsRuntime.InvokeAsync<string>(
                    "localStorage.getItem",
                    cancellationToken,
                    "authToken");

                if (!string.IsNullOrEmpty(token))
                {
                    // Ýsteðe Authorization header'ý ekle
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (JSDisconnectedException)
            {
                // Prerender sýrasýnda veya baðlantý koptuðunda sessizce devam et
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token injection failed: {ex.Message}");
            }

            // Ýsteði gönder
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
