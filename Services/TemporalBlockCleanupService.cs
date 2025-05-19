using BlockedCountriesAPI.Repositories;

namespace BlockedCountriesAPI.Services
{
    public class TemporalBlockCleanupService : BackgroundService
    {
        private readonly InMemoryRepository _repository;
        private readonly ILogger<TemporalBlockCleanupService> _logger;

        public TemporalBlockCleanupService(InMemoryRepository repository, ILogger<TemporalBlockCleanupService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("⏳ Temporal Block Cleanup Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;
                    var expired = _repository.BlockedCountries
                        .Where(c => c.Value.IsTemporary && c.Value.ExpiresAt < now)
                        .Select(c => c.Key)
                        .ToList();

                    foreach (var key in expired)
                    {
                        _repository.BlockedCountries.TryRemove(key, out _);
                        _logger.LogInformation($"✅ Unblocked expired temporary country: {key}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "⚠️ Error in TemporalBlockCleanupService");
                }

                // انتظر دقيقة
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("🛑 Temporal Block Cleanup Service stopped.");
        }
    }
}
