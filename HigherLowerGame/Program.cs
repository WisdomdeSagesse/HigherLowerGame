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


        /*
         This proceedure reads the contents of a txt file containing the game art and dispalys it to the console.
         */
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


        /*
         This proceedure displays the game introduction and instructions to the console.
         */
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


        /*
         This function generates a random integer from the total number of movies.
         Returns:
         A random integer between 0 and 249
         */
        static int GenerateRandomNumber()
        {
            int randomNumber = random.Next(numberOfMovies);
            return randomNumber;
        }


        /*
         This function parses a CSV file containing the list of IMDB's top 250 movies and returns lists containing the ranks,
         titles and release years of the movies.
         Parameters: 
         string filePath - The CSV filepath.
         Returns:
         Three lists containing the ranks titles and release years of the movies.
         */

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


        /*
         This function passes the random number returned from the GenerateRandomNumber() function as the index number into the Lists,
         returned from the ParseCSVFile() function to create a new instance of the Movie class object.
         titles and release years of the movies.
         Parameters: 
         List<int> rank - A list conataining the movie ranks.
         List<string> title - A list containing the movie titles.
         List<int> year - A list containing the movie release years.
         Returns:
         A new instance of the Movie class object.
         */
        static Movie PickMovie(List<int> rank, List<string> title, List<int> year)
        {
            int randomNumber = GenerateRandomNumber();
            Movie movie = new Movie(rank[randomNumber], title[randomNumber], year[randomNumber]);
            return movie;
        }


        /*
         This function compares the player's input against a movie ranks and returns a true or false boolean value
         Parameters: 
         int movieARank - Rank of movieA
         int movieBRank - Rank of movieB
         Returns:
         A true or false value.
         */
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


        /*
         This function creates a new instance of the Player class object
         Parameters: 
         int score - The player's final score
         DateTime dateTime - The datetime at the end of the current iteration of gameplay.
         Returns:
         A new instance of the Player class object.
         */
        static Player PlayerRecord(int score, DateTime dateTime)
        {
            string display = "Enter your name: ";
            Console.Write(display);
            string name = Console.ReadLine();
            Player player = new Player(name, score, dateTime);
            displayPlayerScore(player);
            return player;
        }

        /*
         This proceedure dispalys the player name score and datetime to the console
         Parameters: 
         Player player - Current instance of the Player class object.
         */
        static void displayPlayerScore (Player player)
        {
            Console.WriteLine();
            string header = "Player Name     Player Score       Date/Time";
            Console.WriteLine(header);
            Console.WriteLine($"Player Score: " +
            $"\n{player.PlayerName}            " +
            $"{player.PlayerScore}              " +
            $"{player.Date}");
            Console.WriteLine();
        }


        /*
         This proceedure serializes the record of the player to a bin file. New records are appended to the end of the file.
         Parameters: 
         Player playerRecord - Current instance of the Player class object.
         string filePath - File path of the bin file.
         */
        static void addRecord(Player playerRecord, string filePath)
        {
            FileStream playerScoresData = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(playerScoresData, playerRecord); 
            playerScoresData.Close();
        }


        /*
         This function deserializes the player records conatined in a bin file and passes it into a list.
         Parameters: 
         string filePath - File path of the bin file.
         Returns:
         A list containing the player records.
         */
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


        /*
         This proceedure parses the player records contained in the list returened from the DeserializePlayersScores() funtion,
         and passes them into lists containing player names, scores and datetimes. The list of players scores in evaluated to
         find the max score and record of the player with the max score is displayed to the console.
         Parameters: 
         List<Player> - playerScores - List containing the player records.
         */
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


        /*
         This function validates the players input and returns a valid input
         Parameters: 
         string display - A prompt asking the player to enter an input
         Returns:
         A vaild player input.
         */
        static char VaidateAnswerInput(string displayAction)
        {
            char playerInput;
            char[] validResponses = {'h', 'l'};
            do
            {
                Console.Write(displayAction);
                playerInput = Char.ToLower(Console.ReadKey().KeyChar);
                Console.WriteLine();
            }
            while (!validResponses.Contains(playerInput));
           
            return playerInput;
        }

        /*
        This function validates the players input and returns a valid input
         Parameters: 
         string display - A prompt asking the player to enter an input
         Returns:
         A vaild player input.
        */
        static char VaidateReplayGameInput(string displayAction)
        {
            char playerInput;
            char[] validResponses = {'y', 'x', 'n'};
            do
            {
                Console.Write(displayAction);
                playerInput = Char.ToLower(Console.ReadKey().KeyChar);
                Console.WriteLine();
            }
            while (!validResponses.Contains(playerInput));

            return playerInput;
        }

        /*
         This proceedure serializes the score of the player to a bin file.
         Parameters: 
         int score - The player's final score at the current gamplay instance.
         string filePath - File path of the bin file.
         */
        static void autoGameSave(int score, string filePath)
        {
            FileStream autoSaveFile = new FileStream(filePath, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(autoSaveFile, score);
            autoSaveFile.Close();
        }


        /*
         This function deserializes the player's scores conatined in a bin file.
         Parameters: 
         string filePath - File path of the bin file.
         Returns:
         An integer - the player's final score at the current gameplay instance.
         */
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
