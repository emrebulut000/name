using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Unı_Proje.Web.Services
{
    // Bu servis SADECE sunucu tarafında hata almamak için var.
    // Sunucuda localStorage olmadığı için "Hiç kimse giriş yapmamış" (Anonymous) gibi davranmalı.
    public class DummyAuthStateProvider : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // İçi boş bir ClaimsIdentity oluşturuyoruz.
            // Bu, "Kullanıcı Giriş Yapmadı" anlamına gelir.
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());

            return Task.FromResult(new AuthenticationState(anonymous));
        }
    }
}