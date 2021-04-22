using System;
using System.Collections.Generic;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class Team
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string Initials { get; set; }
        public decimal Budget { get; set; }

        public int PrimaryKitColorId { get; set; } // FK
        public Color PrimaryKitColor { get; set; } // Nav prop

        public int SecondaryKitColorId { get; set; } // FK
        public Color SecondaryKitColor { get; set; } // Nav prop

        public int TownId { get; set; } // FK
        public Town Town { get; set; } // Nav prop

        public virtual ICollection<Game> HomeGames { get; set; } = new HashSet<Game>();
        public virtual ICollection<Game> AwayGames { get; set; } = new HashSet<Game>();

        public virtual ICollection<Player> Players { get; set; } = new HashSet<Player>();
    }
}
//TeamId, Name, LogoUrl, Initials (JUV, LIV, ARS…), Budget, PrimaryKitColorId, SecondaryKitColorId, TownId
