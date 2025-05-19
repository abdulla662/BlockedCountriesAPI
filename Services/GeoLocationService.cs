using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BlockedCountriesAPI.Services
{
    public class GeoLocationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public GeoLocationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeolocationApi:ApiKey"]!;
            _baseUrl = configuration["GeolocationApi:BaseUrl"]!;
        }

        public async Task<(string CountryCode, string CountryName)?> GetCountryFromIPAsync(string ip)
        {
            var url = $"{_baseUrl}?apiKey={_apiKey}&ip={ip}";

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("========= API RESPONSE =========");
            Console.WriteLine("URL: " + url);
            Console.WriteLine("Status: " + response.StatusCode);
            Console.WriteLine("Content:");
            Console.WriteLine(content);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = JObject.Parse(content);

            string code = json["country_code2"]?.ToString() ?? "";
            string name = json["country_name"]?.ToString() ?? "";

            if (string.IsNullOrEmpty(code))
                return null;

            return (code, name);
        }
    }
}
