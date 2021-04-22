using System;
using System.Collections.Generic;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public int SquadNumber { get; set; } // or byte

        public int TeamId { get; set; }
        public Team Team { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; }

        public bool IsInjured { get; set; }


        // mapping collection -> point to PlayerStatistic class/table 
        // In PlayerStatistic have two foreign keys GameId/PlayerId each with navigational properties Game Game/ Player Player.
        public virtual ICollection<PlayerStatistic> PlayerStatistics { get; set; } = new HashSet<PlayerStatistic>();
    }
}
//PlayerId, Name, SquadNumber, TeamId, PositionId, IsInjured