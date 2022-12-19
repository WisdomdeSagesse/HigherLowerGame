using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace HigherLowerGame
{
    internal class Program
    {
        static Random random = new Random();
        const int numberOfMovies = 250;
        static void Main(string[] args)
        {
            string filePath = @"C:\Users\User\OneDrive\NTU Documents\Software Engineering Coursework\IMDB Top 250 Movies.csv";
            string[] movieList = File.ReadAllLines(filePath);
            List<int> rank = new List<int>();
            List<string> title = new List<string>();
            List<int> year = new List<int>();

            for (int i = 1; i < movieList.Length; i++)
            {
                string[] rowData = movieList[i].Split(','); // Split the data contained in each row by the comma delimiter
                rank.Add(int.Parse(rowData[0]));
                title.Add(rowData[1]);
                year.Add(int.Parse(rowData[2]));
            }
            void PlayGame()
            {
                Movie optionB = PickMovie(rank, title, year);
                int score = 0;
                bool endGame = false;
                while (!endGame)
                {

                    Movie optionA = optionB;
                    do
                    {
                        optionB = PickMovie(rank, title, year);
                    }
                    while (optionA == optionB);

                    Console.WriteLine($"Compare A: {optionA.MovieTitle} released in {optionA.ReleaseYear}" +
                        "\nVS" +
                        $"\nAgainst B: {optionB.MovieTitle} released in {optionB.ReleaseYear}");

                    if (CompareAnswer(optionA.MovieRank, optionB.MovieRank) == true)
                    {
                        Console.Clear();
                        score++;
                        Console.WriteLine($"You are Correct. Current score : {score}");
                    }
                    else
                    {
                        Console.WriteLine($"You are wrong. Final score: {score}");
                        endGame = true;
                    }
                }
                Console.Write("Do you wish to play again? Enter 'Y' for yes and 'N' for NO: ");
                char response = Char.ToLower(Console.ReadKey().KeyChar);
                if (response == 'y')
                {
                    Console.Clear();
                    PlayGame();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine($"Game Over. Final Score: {score}");
                    Player player = PlayerDetails(score, Player.GetDateTime());
                    string databaseFilePath = @"C:\Users\User\OneDrive\NTU Documents\Software Engineering Coursework\SoftwareEngCoursework\HigherLowerGame\HigherLowerGame\bin\Debug\PlayerScores.txt";
                    addRecord(player.PlayerName, player.PlayerScore, player.Date, databaseFilePath);
                    dispalyHighScore(databaseFilePath);
                }
            }
            PlayGame();

        }

        static int GenerateRandomNumber()
        {
            int randomNumber = random.Next(numberOfMovies);
            return randomNumber;
        }

        static Movie PickMovie(List<int> rank, List<string> title, List<int> year)
        {
            int randomNumber = GenerateRandomNumber();
            Movie movie = new Movie(rank[randomNumber], title[randomNumber], year[randomNumber]);
            return movie;
        }

        static bool CompareAnswer(int movieARank, int movieBRank)
        {
            Console.Write("Which movie has a higher IMDB rank. Type A or B: ");
            char playerInput = Char.ToLower(Console.ReadKey().KeyChar);
            Console.WriteLine();
            bool output;
            if (playerInput == 'a')
            {
                output = movieARank < movieBRank;
            }
            else
            {
                output = movieBRank < movieARank;
            }
            return output;
        }

        static Player PlayerDetails(int score, DateTime dateTime)
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Player player = new Player(name, score, dateTime);
            Console.WriteLine();
            string header = "Player Name     Player Score       Date/Time";
            Console.WriteLine(header);
            Console.WriteLine($"Player Score: " +
                $"\n{player.PlayerName}            " +
                $"{player.PlayerScore}              " +
                $"{player.Date}");
            return player;
        }

        static void addRecord(string playerName, int playerScore, DateTime dateTime, string filePath)
        {
            StreamWriter newRecord = new StreamWriter(filePath, true);
            newRecord.WriteLine($"{playerName}, {playerScore}, {dateTime}");
            newRecord.Close();
        }

        static void dispalyHighScore(string filepath)
        {
            string[] playerScores = File.ReadAllLines(filepath);
            List<string> playerName = new List<string>();
            List<int> playerScore = new List<int>();
            List<DateTime> dateTime = new List<DateTime>();
            //Dictionary<int, Player> playerScoreDatabase = new Dictionary<int, Player>();
            for (int i = 2; i < playerScores.Length; i++)
            {
                string[] rowData = playerScores[i].Split(',');
                playerName.Add(rowData[0]);
                playerScore.Add(Convert.ToInt32(rowData[1]));
                dateTime.Add(Convert.ToDateTime(rowData[2]));
            }
            int indexNumberOfMaxScore = playerScore.IndexOf(playerScore.Max());
            Console.WriteLine($"High Score: " +
                $"\n{playerName[indexNumberOfMaxScore]}        " +
                $"{playerScore[indexNumberOfMaxScore]}         " +
                $"{dateTime[indexNumberOfMaxScore]}");
        }
    }
}
