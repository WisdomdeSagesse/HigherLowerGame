using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Schema;

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
                string display = "Do you wish to play again? Enter 'Y' for yes and 'N' for NO: ";
                char response = VaidateReplayGameInput(display);
                if (response == 'y')
                {
                    Console.Clear();
                    PlayGame();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine($"Game Over. Final Score: {score}");
                    if (score > 0)
                    {
                        filePath = @"C:\Users\User\OneDrive\NTU Documents\Software Engineering Coursework\SoftwareEngCoursework\HigherLowerGame\HigherLowerGame\bin\Debug\PlayerScores.bin";
                        addRecord(PlayerRecord(score, Player.GetDateTime()), filePath);
                        DeserailizePlayerScores(filePath);
                        displayHighScore(DeserailizePlayerScores(filePath));
                    }
                    
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
            string display = "Which movie has a higher IMDB rank. Type A or B: ";
            char playerInput = VaidateAnswerInput(display);
            Console.WriteLine();
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
        
        static Player PlayerRecord(int score, DateTime dateTime)
        {
            string display = "Enter your name: ";
            Console.Write(display);
            string name = Console.ReadLine();
            Player player = new Player(name, score, dateTime);
            displayPlayerScore(player);
            return player;
        }

        
        static void displayPlayerScore (Player player)
        {
            Console.WriteLine();
            string header = "Player Name     Player Score       Date/Time";
            Console.WriteLine(header);
            Console.WriteLine($"Player Score: " +
            $"\n{player.PlayerName}            " +
            $"{player.PlayerScore}              " +
            $"{player.Date}");
        }
   

        static void addRecord(Player playerRecord, string filePath)
        {
            FileStream playerScoresData = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(playerScoresData, playerRecord); 
            playerScoresData.Close();
        }

        static List<Player> DeserailizePlayerScores(string filePath)
        {
            List<Player> players = new List<Player>();
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream playerScoresData = new FileStream(filePath, FileMode.Open);
            // Code to read all objects in the binary file
            // ChatGPT
            // https://chat.openai.com/chat
            // [Accessed 31/12/2022]
            while (playerScoresData.Position < playerScoresData.Length)
            {
                Player playersScores = (Player)formatter.Deserialize(playerScoresData);
                players.Add(playersScores);
            }
            playerScoresData.Close();
            return players;  
        }

        
        static void displayHighScore(List<Player> playersScores)
        {
            List<string> playerName = new List<string>();
            List<int> playerScore = new List<int>();
            List<DateTime> dateTime = new List<DateTime>();
            
            foreach(Player player in playersScores)
            {
                playerName.Add(player.PlayerName);
                playerScore.Add(player.PlayerScore);
                dateTime.Add(player.Date);
            }
            
            int maxScore = playerScore.Max();
            int indexNumberOfMaxScore = playerScore.IndexOf(maxScore);
            Console.WriteLine($"High Score: " +
                $"\n{playerName[indexNumberOfMaxScore]}        " +
                $"{playerScore[indexNumberOfMaxScore]}         " +
                $"{dateTime[indexNumberOfMaxScore]}");
        }
        
        static char VaidateAnswerInput(string displayAction)
        {
            char playerInput;
            char[] validResponses = {'a', 'b'};
            do
            {
                Console.WriteLine(displayAction);
                playerInput = Char.ToLower(Console.ReadKey().KeyChar);
                Console.WriteLine();
            }
            while (!validResponses.Contains(playerInput));
           
            return playerInput;
        }
        static char VaidateReplayGameInput(string displayAction)
        {
            char playerInput;
            char[] validResponses = {'y', 'n'};
            do
            {
                Console.WriteLine(displayAction);
                playerInput = Char.ToLower(Console.ReadKey().KeyChar);
                Console.WriteLine();
            }
            while (!validResponses.Contains(playerInput));

            return playerInput;
        }
    }
}
