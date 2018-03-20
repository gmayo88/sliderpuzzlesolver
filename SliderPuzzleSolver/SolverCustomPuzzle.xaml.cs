/*Gabriel Mayo w860898
 CSC 412 - Intro to AI
 Ins. Bryant Walley
 Slider Puzzle Solver
 10-31-2017*/
 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SliderPuzzleSolver
{
    /// <summary>
    /// Interaction logic for SolverCustomPuzzle.xaml
    /// </summary>
    public partial class SolverCustomPuzzle : Page
    {
        public SolverCustomPuzzle()
        {
            InitializeComponent();
        }

        //This function is called when the user click the Generate 
        //button on the custom puzzle page
        public void Input_Puzzle(object sender, RoutedEventArgs e)
        {
            //Input type checking
            int[,] tempMatrix = new int[3, 3];  //Temp matrix for type checking

            if (!(Int32.TryParse(Custom00.Text, out tempMatrix[0, 0])))


            //First copy the inut to the temp matrix
            tempMatrix[0, 0] = int.Parse(Custom00.Text);
            tempMatrix[0, 1] = int.Parse(Custom01.Text);
            tempMatrix[0, 2] = int.Parse(Custom02.Text);
            tempMatrix[1, 0] = int.Parse(Custom10.Text);
            tempMatrix[1, 1] = int.Parse(Custom11.Text);
            tempMatrix[1, 2] = int.Parse(Custom12.Text);
            tempMatrix[2, 0] = int.Parse(Custom20.Text);
            tempMatrix[2, 1] = int.Parse(Custom21.Text);
            tempMatrix[2, 2] = int.Parse(Custom22.Text);


        }
    }
}
