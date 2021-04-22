using System;
using System.Collections.Generic;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class Game
    {
        public int GameId { get; set; }

        public int HomeTeamId { get; set; } // inverce navigation prop -> point to class Team
        public Team HomeTeam { get; set; }

        public int AwayTeamId { get; set; } // inverce navigation prop -> point to class Team
        public Team AwayTeam { get; set; }

        public int HomeTeamGoals { get; set; } // ot better to use byte
        public int AwayTeamGoals { get; set; } // ot better to use byte
        public DateTime DateTime { get; set; }
        public double HomeTeamBetRate { get; set; }
        public double AwayTeamBetRate { get; set; }
        public double DrawBetRate { get; set; }
        public string Result { get; set; } // or int

        // mapping collection -> point to PlayerStatistic class/table 
        // In PlayerStatistic have two foreign keys GameId/PlayerId each with navigational properties Game Game/ Player Player.
        public virtual ICollection<PlayerStatistic> PlayerStatistics { get; set; } = new HashSet<PlayerStatistic>(); 

        public virtual ICollection<Bet> Bets { get; set; } = new HashSet<Bet>();
    }
}
//GameId, HomeTeamId, AwayTeamId, HomeTeamGoals, AwayTeamGoals, 
//DateTime, HomeTeamBetRate, AwayTeamBetRate, DrawBetRate, Result