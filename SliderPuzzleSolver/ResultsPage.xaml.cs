/*Gabriel Mayo w860898
 CSC 412 - Intro to AI
 Ins. Bryant Walley
 Slider Puzzle Solver
 10-31-2017, 11-7-2017, 11-14-2017*/
using System.Windows;
using System.Windows.Controls;

namespace SliderPuzzleSolver
{
    //This class controls the page where the results 
    //of the search are displayed.
    public partial class ResultsPage : Page
    {
        public ResultsPage()
        {
            InitializeComponent();
        }

        private void Return_Home(object sender, RoutedEventArgs e)
        {
            MainWindow.homePage = new SolverHome();
            this.NavigationService.Navigate(MainWindow.homePage);
        }
    }
}
