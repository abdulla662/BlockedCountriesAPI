using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories;
using BlockedCountriesAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static BlockedCountriesAPI.Controllers.IPController;

namespace BlockedCountriesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly GeoLocationService _geoService;
        private readonly InMemoryRepository _repo;
        public CountriesController(GeoLocationService geoService, InMemoryRepository repo)
        {
            _geoService = geoService;
            _repo = repo;
        }
        [HttpPost("countries/block")]
        public IActionResult BlockCountry([FromBody] string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return BadRequest("Country code is required.");

            countryCode = countryCode.ToUpper();

            if (_repo.BlockedCountries.ContainsKey(countryCode))
                return Conflict("Country already blocked.");

            _repo.BlockedCountries[countryCode] = new BlockedCountry { CountryCode = countryCode };

            return Ok($"Country '{countryCode}' has been blocked.");
        }
        // 1. DELETE blocked country api 
        [HttpDelete("countries/block/{countryCode}")]
        public IActionResult UnblockCountry(string countryCode)
        {
            countryCode = countryCode.ToUpper();

            if (!_repo.BlockedCountries.ContainsKey(countryCode))
                return NotFound("Country is not blocked.");

            _repo.BlockedCountries.TryRemove(countryCode, out _);
            return Ok($"Country '{countryCode}' has been unblocked.");
        }
        // 2. GET blocked countries api
        [HttpGet("countries/blocked")]
        public IActionResult GetBlockedCountries([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? filter = null)
        {
            var query = _repo.BlockedCountries.Values.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
                query = query.Where(c =>
                    c.CountryCode.Contains(filter.ToUpper()) ||
                    (c.CountryName != null && c.CountryName.Contains(filter, StringComparison.OrdinalIgnoreCase))
                );
            var result = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(result);
        }
        // 3. POST add temporary block to a country api 
        [HttpPost("countries/temporal-block")]
        public IActionResult TemporalBlock([FromBody] TemporalBlockRequest request)
        {
            var code = request.CountryCode.ToUpper();

            if (string.IsNullOrWhiteSpace(code) || request.DurationMinutes < 1 || request.DurationMinutes > 1440)
                return BadRequest("Invalid input.");

            if (_repo.BlockedCountries.ContainsKey(code) && _repo.BlockedCountries[code].IsTemporary)
                return Conflict("Country is already temporarily blocked.");

            _repo.BlockedCountries[code] = new BlockedCountry
            {
                CountryCode = code,
                IsTemporary = true,
                ExpiresAt = DateTime.UtcNow.AddMinutes(request.DurationMinutes)
            };

            return Ok($"Country '{code}' temporarily blocked for {request.DurationMinutes} minutes.");
        }
        public class TemporalBlockRequest
        {
            public string CountryCode { get; set; }
            public int DurationMinutes { get; set; }
        }
    }
}
