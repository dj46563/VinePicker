using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VinePicker.Models
{
    // Model for a single row in the leaderboard
    public class LeaderboardVine
    {
        public string ThumbnailUrl { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public string Permalink { get; set; }
        public int Rank { get; set; }
    }
}
