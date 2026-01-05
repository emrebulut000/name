using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Uný_Proje.Services
{
    public class GeocodingService
    {
        private readonly HttpClient _httpClient;
        // ?? API KEY ÇALIÞMIYOR MU? Alternatif olarak Nominatim (OpenStreetMap) kullanýlabilir (ücretsiz)
        // VEYA Yeni Google API key alýn: https://console.cloud.google.com/
        private const string GOOGLE_GEOCODING_API_KEY = "AIzaSyBVVhXQXszvwYexO5c1kzLcHBEMjuDrxjU"; // Geçici test key

        public GeocodingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ?? Adres metninden koordinat al (Google + Nominatim fallback)
        public async Task<(double? Latitude, double? Longitude)> GetCoordinatesAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                Console.WriteLine("[GEOCODING] ?? Adres boþ");
                return (null, null);
            }

            // ÖNCE GOOGLE API'YÝ DENE
            var result = await TryGoogleGeocodingAsync(address);
            if (result.Latitude.HasValue && result.Longitude.HasValue)
            {
                return result;
            }

            // BAÞARISIZ OLURSA NOMINATIM DENE (ÜCRETSÝZ, API KEY YOK)
            Console.WriteLine("[GEOCODING] Google baþarýsýz, Nominatim deneniyor...");
            return await TryNominatimGeocodingAsync(address);
        }

        // Google Maps Geocoding
        private async Task<(double? Latitude, double? Longitude)> TryGoogleGeocodingAsync(string address)
        {
            try
            {
                var encodedAddress = Uri.EscapeDataString(address + ", Türkiye");
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={GOOGLE_GEOCODING_API_KEY}";

                Console.WriteLine($"[GOOGLE] ?? {address}");
                
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[GOOGLE] ? HTTP {response.StatusCode}");
                    return (null, null);
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GeocodingResponse>(json);

                if (result?.Status == "OK" && result.Results?.Length > 0)
                {
                    var location = result.Results[0].Geometry?.Location;
                    if (location != null)
                    {
                        Console.WriteLine($"[GOOGLE] ? ({location.Lat}, {location.Lng})");
                        return (location.Lat, location.Lng);
                    }
                }

                Console.WriteLine($"[GOOGLE] ?? Status: {result?.Status}");
                return (null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GOOGLE] ? {ex.Message}");
                return (null, null);
            }
        }

        // Nominatim (OpenStreetMap) - ÜCRETSIZ ALTERNATÝF
        private async Task<(double? Latitude, double? Longitude)> TryNominatimGeocodingAsync(string address)
        {
            try
            {
                var encodedAddress = Uri.EscapeDataString(address + ", Türkiye");
                var url = $"https://nominatim.openstreetmap.org/search?q={encodedAddress}&format=json&limit=1";

                Console.WriteLine($"[NOMINATIM] ?? {address}");
                
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent", "IkinciElProjesi/1.0");
                
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[NOMINATIM] ? HTTP {response.StatusCode}");
                    return (null, null);
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[NOMINATIM] ?? Response: {json.Substring(0, Math.Min(100, json.Length))}...");
                
                // JSON boþ mu kontrol et
                if (string.IsNullOrWhiteSpace(json) || json == "[]")
                {
                    Console.WriteLine($"[NOMINATIM] ?? Boþ sonuç");
                    return (null, null);
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var results = JsonSerializer.Deserialize<NominatimResult[]>(json, options);

                if (results != null && results.Length > 0)
                {
                    // String'i double'a parse et (InvariantCulture kullan)
                    if (double.TryParse(results[0].Lat, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
                        double.TryParse(results[0].Lon, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lon))
                    {
                        Console.WriteLine($"[NOMINATIM] ? ({lat}, {lon})");
                        return (lat, lon);
                    }
                    else
                    {
                        Console.WriteLine($"[NOMINATIM] ? Parse hatasý: Lat='{results[0].Lat}', Lon='{results[0].Lon}'");
                    }
                }

                Console.WriteLine($"[NOMINATIM] ?? Sonuç yok");
                return (null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NOMINATIM] ? {ex.GetType().Name}: {ex.Message}");
                return (null, null);
            }
        }

        // Response modelleri
        private class GeocodingResponse
        {
            public string Status { get; set; } = "";
            public GeocodingResult[]? Results { get; set; }
        }

        private class GeocodingResult
        {
            public Geometry? Geometry { get; set; }
        }

        private class Geometry
        {
            public Location? Location { get; set; }
        }

        private class Location
        {
            public double Lat { get; set; }
            public double Lng { get; set; }
        }

        // Nominatim response
        private class NominatimResult
        {
            public string Lat { get; set; } = "";
            public string Lon { get; set; } = "";
        }
    }
}

