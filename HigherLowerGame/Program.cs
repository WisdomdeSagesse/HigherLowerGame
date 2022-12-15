using System;
using System.Collections.Generic;
using System.IO;

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
                }
            }
            PlayGame();

        }

        static int GenerateRandomNumber()
        {
            int randomNumber = random.Next(numberOfMovies + 1);
            // the total number of movies is increased by 1 to capture the all the data when using the random module
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


    }
}
