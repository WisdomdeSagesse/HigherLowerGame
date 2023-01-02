using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace HigherLowerGame
{
    [Serializable]
    public class Player
    {
        public string PlayerName { get; set; }
        public int PlayerScore { get; set; }
        public DateTime Date { get; set; }

        public Player(string name, int score, DateTime date)
        {
            this.PlayerName = name;
            this.PlayerScore = score;
            this.Date = date;
        }
        
        static public DateTime GetDateTime()
        {
            DateTime now = DateTime.Now;
            return now;
        }
    }
}
