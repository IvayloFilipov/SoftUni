using P03_FootballBetting.Data.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class Bet
    {
        public int BetId { get; set; }
        public decimal Amount { get; set; }

        public Prediction Prediction { get; set; } // or -> enum in folder Enumerations
        public DateTime DateTime { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }

        //private enum Prediction
        //{
        //    Win = 1,
        //    Lose = 2,
        //    Standoff = 3
        //}

    }
}
//BetId, Amount, Prediction, DateTime, UserId, GameId