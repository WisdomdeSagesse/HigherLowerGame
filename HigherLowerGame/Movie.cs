using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherLowerGame
{
    public class Movie
    {
        public int MovieRank { get; set; }
        public string MovieTitle { get; set; }
        public int ReleaseYear { get; set; }

        public Movie(int movieRank, string movieTitle, int releaseYear)
        {
            this.MovieRank = movieRank;
            this.MovieTitle = movieTitle;
            this.ReleaseYear = releaseYear;
        }   
    }
}
