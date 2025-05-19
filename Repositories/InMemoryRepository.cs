using BlockedCountriesAPI.Models;
using System.Collections.Concurrent;

namespace BlockedCountriesAPI.Repositories
{
    public class InMemoryRepository
    {
        public ConcurrentDictionary<string, BlockedCountry> BlockedCountries { get; } = new();
        public List<BlockedAttemptLog> BlockedLogs { get; } = new();
    }
}
