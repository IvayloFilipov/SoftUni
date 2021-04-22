using System;
using System.Collections.Generic;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class PlayerStatistic
    {
        public int GameId { get; set; } // FK
        public Game Game { get; set; } // Nav prop

        public int PlayerId { get; set; } // FK
        public Player Player { get; set; } // Nav prop

        public int ScoredGoals { get; set; } // or better to use byte
        public int Assists { get; set; } // or better to use byte
        public int MinutesPlayed { get; set; } // or better to use byte
    }
}
//mapping class - GameId, PlayerId, ScoredGoals, Assists, MinutesPlayed 