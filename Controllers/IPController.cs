using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories;
using BlockedCountriesAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BlockedCountriesAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class IPController : ControllerBase
    {
        private readonly GeoLocationService _geoService;
        private readonly InMemoryRepository _repo;

        public IPController(GeoLocationService geoService, InMemoryRepository repo)
        {
            _geoService = geoService;
            _repo = repo;
        }

        // 1. GET your current ip address api
        [HttpGet("ip/lookup")]
        public async Task<IActionResult> LookupCountry([FromQuery] string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                ipAddress = "8.8.8.8";

            var result = await _geoService.GetCountryFromIPAsync(ipAddress);

            if (result == null)
                return NotFound("Country information could not be retrieved.");

            return Ok(new
            {
                IP = ipAddress,
                CountryCode = result.Value.CountryCode,
                CountryName = result.Value.CountryName
            });
        }

        // 2. GET to check if the ip address is blocked or not
        [HttpGet("ip/check-block")]
        public async Task<IActionResult> CheckBlock([FromQuery] string? ipAddress, [FromQuery] string? countryCode)
        {
            string code = "";
            string name = "";
            string ip = "";

            if (!string.IsNullOrWhiteSpace(countryCode))
            {
                code = countryCode.ToUpper();
                name = "Unknown"; 
                ip = "ManualInput";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(ipAddress))
                {
                    ip = ipAddress;
                }
                else
                {
                    ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "8.8.8.8";

                    if (ip == "::1") ip = "8.8.8.8";
                }

                var result = await _geoService.GetCountryFromIPAsync(ip);
                if (result == null)
                    return NotFound("Could not determine location.");

                code = result.Value.CountryCode.ToUpper();
                name = result.Value.CountryName;
            }

            var isBlocked = _repo.BlockedCountries.ContainsKey(code);

            _repo.BlockedLogs.Add(new BlockedAttemptLog
            {
                IPAddress = ip,
                CountryCode = code,
                IsBlocked = isBlocked,
                Timestamp = DateTime.UtcNow,
                UserAgent = Request.Headers["User-Agent"].ToString()
            });

            return Ok(new
            {
                IP = ip,
                CountryCode = code,
                CountryName = name,
                IsBlocked = isBlocked
            });
        }
    }
}
