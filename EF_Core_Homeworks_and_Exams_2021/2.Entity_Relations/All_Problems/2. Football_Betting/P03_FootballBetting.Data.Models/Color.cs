using P03_FootballBetting.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P03_FootballBetting.Data
{
    public class Color
    {
        public int ColorId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Team> PrimaryKitTeams { get; set; } = new HashSet<Team>();

        public virtual ICollection<Team> SecondaryKitTeams { get; set; } = new HashSet<Team>();
    }
}
//ColorId, Name
