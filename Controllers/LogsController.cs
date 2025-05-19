using BlockedCountriesAPI.Repositories;
using BlockedCountriesAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountriesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly GeoLocationService _geoService;
        private readonly InMemoryRepository _repo;

        public LogsController(GeoLocationService geoService, InMemoryRepository repo)
        {
            _geoService = geoService;
            _repo = repo;
        }
        //. GET the bkocked attempts from blocked ips
        [HttpGet("logs/blocked-attempts")]
        public IActionResult GetBlockedLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var logs = _repo.BlockedLogs
                .OrderByDescending(log => log.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(logs);
        }

    }
}
