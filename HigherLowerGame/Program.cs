using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace HigherLowerGame
{
    internal class Program
    {
        static Random random = new Random();
        const int numberOfMovies = 250;
        const int defaultScore = 0;
        static void Main(string[] args)
        {
            gameIntro();
            void PlayGame(int score)
            {
                string filePath = @"C:\Users\User\OneDrive\NTU Documents\Software Engineering Coursework\IMDB Top 250 Movies.csv";
                List<int> rank = ParseCSVFile(filePath).movieRanks;
                List<string> title = ParseCSVFile(filePath).movieTitles;
                List<int> year = ParseCSVFile(filePath).releaseYears;
                Movie optionB = PickMovie(rank, title, year);
                bool endGame = false;
                while (!endGame)
                {

                    Movie optionA = optionB;
                    do
                    {
                        optionB = PickMovie(rank, title, year);
                    }
                    while (optionA.MovieRank == optionB.MovieRank);

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
                string display = "Do you wish to play again? " +
                    "\nEnter 'Y' to play a new game" +
                    "\nEnter 'X' to continue from last score" +
                    "\nEnter 'N' to end game: ";
                char response = VaidateReplayGameInput(display);
                if (response == 'y')
                {
                    Console.Clear();
                    PlayGame(defaultScore); 
                }
                else if(response == 'x')
                {
                    filePath = @"C:\Users\User\OneDrive\NTU Documents\Software Engineering Coursework\SoftwareEngCoursework\HigherLowerGame\HigherLowerGame\bin\Debug\SavedScore.bin";
                    Console.Clear();
                    autoGameSave(score, filePath);
                    int savedScore = LoadAutoGameSave(filePath);
                    PlayGame(savedScore);
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
            PlayGame(defaultScore);

        }

        static void displayGameArt()
        {
            string filePath = @"C:\Users\User\OneDrive\NTU Documents\Software Engineering Coursework\GameArt.txt";
            if(!File.Exists(filePath))
            {
                throw new Exception("Game art does not exist");
            }
            else 
            {
                StreamReader gameArt = new StreamReader(filePath);
                string display = gameArt.ReadToEnd();
                Console.WriteLine(display);
                gameArt.Close();
            }
        }

        static void gameIntro()
        {
            displayGameArt();   
            Console.WriteLine("Welcome to the IMDB Top 250 Higher/Lower Game" +
                "\nTwo movie options will be displayed to you" +
                "\nSelect if the second movie option has a higher or lower rating compared to the first movie option");

            //Code to time the dsiplay of the game introductory message :
            //"Console.Clear() after a timeout", Ofiris, 19/04/2013
            //https://stackoverflow.com/questions/16109055/console-clear-after-a-timeout
            //[Accessed 02/01/2023]
            System.Threading.Thread.Sleep(5000);
            Console.Clear();
        }

        static int GenerateRandomNumber()
        {
            int randomNumber = random.Next(numberOfMovies);
            return randomNumber;
        }


        //Code to return mutliple values in a method taken from:
        //"Return multiple values to a method caller", Francisco Noriega, 03/08/2018
        //https://stackoverflow.com/questions/748062/return-multiple-values-to-a-method-caller/36436255#36436255
        //[Accessed 13/12/2022]
        static (List<int> movieRanks, List<string> movieTitles, List<int> releaseYears) ParseCSVFile(string filePath)
        {
            List<int> movieRanks = new List<int>();
            List<string> movieTitles = new List<string>();
            List<int> releaseYears = new List<int>();
            if (!File.Exists(filePath))
            {
                throw new Exception("Movie File does not exist");
            }
            else 
            {
                string[] movieList = File.ReadAllLines(filePath);
                for (int i = 1; i < movieList.Length; i++)
                {
                    char delimiter = ',';
                    string[] rowData = movieList[i].Split(delimiter); // Split the data contained in each row by the comma delimiter
                    movieRanks.Add(int.Parse(rowData[0]));
                    movieTitles.Add(rowData[1]);
                    releaseYears.Add(int.Parse(rowData[2]));
                }
            }
            return (movieRanks, movieTitles, releaseYears); 
            
        }
        

    static Movie PickMovie(List<int> rank, List<string> title, List<int> year)
        {
            int randomNumber = GenerateRandomNumber();
            Movie movie = new Movie(rank[randomNumber], title[randomNumber], year[randomNumber]);
            return movie;
        }


        static bool CompareAnswer(int movieARank, int movieBRank)
        {
            string display = "Does 'Movie B' have a Higher or Hower rating compared to 'Movie A'. " +
                             "Type H for Higher or L for Lower: ";
            char playerInput = VaidateAnswerInput(display);
            Console.WriteLine();
            Console.WriteLine();
            bool output;
            if (playerInput == 'h')
            {
                output = movieBRank < movieARank;
            }
            else
            {
                output = movieARank < movieBRank;
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
            char[] validResponses = {'h', 'l'};
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
            char[] validResponses = {'y', 'x', 'n'};
            do
            {
                Console.WriteLine(displayAction);
                playerInput = Char.ToLower(Console.ReadKey().KeyChar);
                Console.WriteLine();
            }
            while (!validResponses.Contains(playerInput));

            return playerInput;
        }

        static void autoGameSave(int score, string filePath)
        {
            FileStream autoSaveFile = new FileStream(filePath, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(autoSaveFile, score);
            autoSaveFile.Close();
        }

        static int LoadAutoGameSave(string filePath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream autoSaveFile = new FileStream(filePath, FileMode.Open);
            int savedScore = (int)formatter.Deserialize(autoSaveFile);
            autoSaveFile.Close();
            Console.WriteLine(savedScore);
            return savedScore;
        }
    }
}
