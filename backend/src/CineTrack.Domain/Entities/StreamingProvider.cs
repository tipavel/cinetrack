using System;

namespace CineTrack.Domain.Entities
{
    public class StreamingProvider
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string ProviderName { get; set; } = string.Empty; // Netflix, Prime, etc.
        public string? StreamingUrl { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableUntil { get; set; }
        public string? Regions { get; set; } // Comma-separated
        
        public Movie? Movie { get; set; }
    }
}