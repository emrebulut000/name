using Microsoft.Extensions.Configuration;

namespace Uný_Proje.Web.Client.Services
{
    public class ApiConfigService
    {
        private readonly IConfiguration _configuration;

        public ApiConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string LegacyApiUrl => _configuration["ApiEndpoints:LegacyApi"] ?? "https://localhost:7130";
        public string CurrentApiUrl => _configuration["ApiEndpoints:CurrentApi"] ?? "https://localhost:7000";

        // Helper metodlar
        public string GetAuthApiUrl(string endpoint) => $"{LegacyApiUrl}{endpoint}";
        public string GetWebApiUrl(string endpoint) => $"{CurrentApiUrl}{endpoint}";
    }
}
