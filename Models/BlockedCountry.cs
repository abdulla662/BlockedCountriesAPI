namespace BlockedCountriesAPI.Models
{
    public class BlockedCountry
    {

        public string CountryCode { get; set; } = string.Empty;
        public string? CountryName { get; set; }
        public bool IsTemporary { get; set; } = false;
        public DateTime? ExpiresAt { get; set; }
    }
}
