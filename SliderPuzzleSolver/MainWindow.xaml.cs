/*Gabriel Mayo w860898
 CSC 412 - Intro to AI
 Ins. Bryant Walley
 Slider Puzzle Solver
 10-31-2017, 11-7-2017, 11-14-2017*/
using System;
using System.Collections.Generic;
using System.Windows;

namespace SliderPuzzleSolver
{
    //This class controls the main window of the program
    //and creates the initial state.
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Generate a random puzzle at the start
            RandomPuzzle();
            //Load the home page
            homePage = new SolverHome();
            Frame1.Navigate(homePage);
        }

        //This function generates a random puzzle
        public static void RandomPuzzle()
        {
            Random rand = new Random();     //RNG
            //Hash set and int var to make sure all values are unique 
            HashSet<int> check = new HashSet<int>();    
            int curValue = new int();
            

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    curValue = rand.Next(0, 9);
                    while (check.Contains(curValue))
                    {
                        curValue = rand.Next(0, 9);
                    }
                    puzzleMatrix[i, j] = curValue;

                    check.Add(curValue);
                }

        }

        //The initial state of the puzzle
        public static int[,] puzzleMatrix = new int[3,3];

        //Goal state of the puzzle
        public static int[,] goalState = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 0 } };

        //Page objects
        public static SolverHome homePage;
        public static ResultsPage results;
    }

}
