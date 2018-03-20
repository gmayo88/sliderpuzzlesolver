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
    //This class handles the dialog box where the user inputs
    //custom puzzles
    public partial class SolverCustomDialog : Window
    {
        public SolverCustomDialog()
        {
            InitializeComponent();
        }

        //Called when the user clicks Ok to submit their puzzle
        private void BtnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            //Input type checking
            int[,] tempMatrix = new int[3, 3];  //Temp matrix for type checking
            HashSet<int> check = new HashSet<int>(); //HashSet for puzzle validation

            //Make sure the input is numerical
            if ((!(Int32.TryParse(Custom00.Text, out tempMatrix[0, 0]))) ||
                (!(Int32.TryParse(Custom01.Text, out tempMatrix[0, 1]))) ||
                (!(Int32.TryParse(Custom01.Text, out tempMatrix[0, 1]))) ||
                (!(Int32.TryParse(Custom02.Text, out tempMatrix[0, 2]))) ||
                (!(Int32.TryParse(Custom10.Text, out tempMatrix[1, 0]))) ||
                (!(Int32.TryParse(Custom11.Text, out tempMatrix[1, 1]))) ||
                (!(Int32.TryParse(Custom12.Text, out tempMatrix[1, 2]))) ||
                (!(Int32.TryParse(Custom20.Text, out tempMatrix[2, 0]))) ||
                (!(Int32.TryParse(Custom21.Text, out tempMatrix[2, 1]))) ||
                (!(Int32.TryParse(Custom22.Text, out tempMatrix[2, 2]))))
            {
                MessageBox.Show("You must enter a number between 0 and 8.");
                return;
            }

            //Next we validate the puzzle
            //Copy the input to the temp matrix
            tempMatrix[0, 0] = int.Parse(Custom00.Text);
            tempMatrix[0, 1] = int.Parse(Custom01.Text);
            tempMatrix[0, 2] = int.Parse(Custom02.Text);
            tempMatrix[1, 0] = int.Parse(Custom10.Text);
            tempMatrix[1, 1] = int.Parse(Custom11.Text);
            tempMatrix[1, 2] = int.Parse(Custom12.Text);
            tempMatrix[2, 0] = int.Parse(Custom20.Text);
            tempMatrix[2, 1] = int.Parse(Custom21.Text);
            tempMatrix[2, 2] = int.Parse(Custom22.Text);

            //Check the puzzle validity
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    //First check to make sure the numbers are valid for the puzzle
                    if (tempMatrix[i,j] < 0 || tempMatrix[i,j] > 8)
                    {
                        MessageBox.Show("Values must be between 0 and 8.");
                        return;
                    }
                    //Next, check for repeated values
                    if (check.Contains(tempMatrix[i,j]))
                    {
                        MessageBox.Show("Repeat values are not allowed.");
                        return;
                    }
                    check.Add(tempMatrix[i, j]);
                }

            //Once the input has been validated, we copy the puzzle
            //into the initial state and update the display on the homepage.

            Array.Copy(tempMatrix, MainWindow.puzzleMatrix, tempMatrix.Length);

            if (MainWindow.puzzleMatrix[0, 0] == 0)
                MainWindow.homePage.Space00.Content = " ";
            else
                MainWindow.homePage.Space00.Content = MainWindow.puzzleMatrix[0, 0];

            if (MainWindow.puzzleMatrix[0, 1] == 0)
                MainWindow.homePage.Space01.Content = " ";
            else
                MainWindow.homePage.Space01.Content = MainWindow.puzzleMatrix[0, 1];

            if (MainWindow.puzzleMatrix[0, 2] == 0)
                MainWindow.homePage.Space02.Content = " ";
            else
                MainWindow.homePage.Space02.Content = MainWindow.puzzleMatrix[0, 2];

            if (MainWindow.puzzleMatrix[1, 0] == 0)
                MainWindow.homePage.Space10.Content = " ";
            else
                MainWindow.homePage.Space10.Content = MainWindow.puzzleMatrix[1, 0];

            if (MainWindow.puzzleMatrix[1, 1] == 0)
                MainWindow.homePage.Space11.Content = " ";
            else
                MainWindow.homePage.Space11.Content = MainWindow.puzzleMatrix[1, 1];

            if (MainWindow.puzzleMatrix[1, 2] == 0)
                MainWindow.homePage.Space12.Content = " ";
            else
                MainWindow.homePage.Space12.Content = MainWindow.puzzleMatrix[1, 2];

            if (MainWindow.puzzleMatrix[2, 0] == 0)
                MainWindow.homePage.Space20.Content = " ";
            else
                MainWindow.homePage.Space20.Content = MainWindow.puzzleMatrix[2, 0];

            if (MainWindow.puzzleMatrix[2, 1] == 0)
                MainWindow.homePage.Space21.Content = " ";
            else
                MainWindow.homePage.Space21.Content = MainWindow.puzzleMatrix[2, 1];

            if (MainWindow.puzzleMatrix[2, 2] == 0)
                MainWindow.homePage.Space22.Content = " ";
            else
                MainWindow.homePage.Space22.Content = MainWindow.puzzleMatrix[2, 2];

            //Finally, close the window
            this.Close();
        }

        //Called when the window appears
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Custom00.Focus();
        }
    }
}
