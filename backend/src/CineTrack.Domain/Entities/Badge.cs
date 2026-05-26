using System;
using System.Collections.Generic;

namespace CineTrack.Domain.Entities
{
    public class Badge
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public BadgeType Type { get; set; }
        
        public ICollection<User> Users { get; set; } = new List<User>();
    }

    public enum BadgeType
    {
        CinePhile,
        Reviewer,
        SocialButterfly,
        MovieMarathon,
        GenreMaster,
        TrendyWatcher,
        EarlyAdopter
    }
}