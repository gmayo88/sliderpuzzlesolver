/*Gabriel Mayo w860898
 CSC 412 - Intro to AI
 Ins. Bryant Walley
 Slider Puzzle Solver
 10-31-2017, 11-7-2017, 11-14-2017*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

/*Priority_Queue class by Daniel "BlueRaja" Pflughoeft
 https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp */
using Priority_Queue;

namespace SliderPuzzleSolver
{
    //This is the class for the program's  home page
    public partial class SolverHome : Page
    {
        public SolverHome()
        {
            InitializeComponent();
        }

        //Event handler for when the page is loaded
        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            //Update the puzzle display, changing 0 to an empty space
            if (MainWindow.puzzleMatrix[0, 0] == 0)
                this.Space00.Content = " ";
            else
                this.Space00.Content = MainWindow.puzzleMatrix[0, 0];

            if (MainWindow.puzzleMatrix[0, 1] == 0)
                this.Space01.Content = " ";
            else
                this.Space01.Content = MainWindow.puzzleMatrix[0, 1];

            if (MainWindow.puzzleMatrix[0, 2] == 0)
                this.Space02.Content = " ";
            else
                this.Space02.Content = MainWindow.puzzleMatrix[0, 2];

            if (MainWindow.puzzleMatrix[1, 0] == 0)
                this.Space10.Content = " ";
            else
                this.Space10.Content = MainWindow.puzzleMatrix[1, 0];

            if (MainWindow.puzzleMatrix[1, 1] == 0)
                this.Space11.Content = " ";
            else
                this.Space11.Content = MainWindow.puzzleMatrix[1, 1];

            if (MainWindow.puzzleMatrix[1, 2] == 0)
                this.Space12.Content = " ";
            else
                this.Space12.Content = MainWindow.puzzleMatrix[1, 2];

            if (MainWindow.puzzleMatrix[2, 0] == 0)
                this.Space20.Content = " ";
            else
                this.Space20.Content = MainWindow.puzzleMatrix[2, 0];

            if (MainWindow.puzzleMatrix[2, 1] == 0)
                this.Space21.Content = " ";
            else
                this.Space21.Content = MainWindow.puzzleMatrix[2, 1];

            if (MainWindow.puzzleMatrix[2, 2] == 0)
                this.Space22.Content = " ";
            else
                this.Space22.Content = MainWindow.puzzleMatrix[2, 2];
        }

        //Event handler for when the "Generate a Puzzle" button is clicked
        private void Generate_Puzzle(object sender, RoutedEventArgs e)
        {
            MainWindow.RandomPuzzle();

            //Update the puzzle display
            if (MainWindow.puzzleMatrix[0, 0] == 0)
                this.Space00.Content = " ";
            else
                this.Space00.Content = MainWindow.puzzleMatrix[0, 0];

            if (MainWindow.puzzleMatrix[0, 1] == 0)
                this.Space01.Content = " ";
            else
                this.Space01.Content = MainWindow.puzzleMatrix[0, 1];

            if (MainWindow.puzzleMatrix[0, 2] == 0)
                this.Space02.Content = " ";
            else
                this.Space02.Content = MainWindow.puzzleMatrix[0, 2];

            if (MainWindow.puzzleMatrix[1, 0] == 0)
                this.Space10.Content = " ";
            else
                this.Space10.Content = MainWindow.puzzleMatrix[1, 0];

            if (MainWindow.puzzleMatrix[1, 1] == 0)
                this.Space11.Content = " ";
            else
                this.Space11.Content = MainWindow.puzzleMatrix[1, 1];

            if (MainWindow.puzzleMatrix[1, 2] == 0)
                this.Space12.Content = " ";
            else
                this.Space12.Content = MainWindow.puzzleMatrix[1, 2];

            if (MainWindow.puzzleMatrix[2, 0] == 0)
                this.Space20.Content = " ";
            else
                this.Space20.Content = MainWindow.puzzleMatrix[2, 0];

            if (MainWindow.puzzleMatrix[2, 1] == 0)
                this.Space21.Content = " ";
            else
                this.Space21.Content = MainWindow.puzzleMatrix[2, 1];

            if (MainWindow.puzzleMatrix[2, 2] == 0)
                this.Space22.Content = " ";
            else
                this.Space22.Content = MainWindow.puzzleMatrix[2, 2];
        }

        //Event handler for when the "Create your Own" button is clicked
        private void Custom_Puzzle(object sender, RoutedEventArgs e)
        {
            //Opens a dialog box where the user can enter a custom puzzle
            SolverCustomDialog inputDialog = new SolverCustomDialog();
            inputDialog.ShowDialog();
        }

        //Event handler for when the "Breadth-First Search" button is clicked
        //This is the Breadth-First Search
        private void BF_Search(object sender, RoutedEventArgs e)
        {
            //This is the working state
            int[,] workingState = new int[3, 3];
            //This is the temporary array for creating new states
            int[,] tempState = new int[3, 3];
            //Counters for the numbers of moves and nodes
            int moveCount = 0;
            int totalNodes = 0;
            Stopwatch searchTime = new Stopwatch(); //To track the search time
            //Dictionary to track visited states
            Dictionary<string, int[,]> visitedStates = new Dictionary<string, int[,]>();

            //An object for writing solutions to a data file
            System.IO.StreamWriter searchOutput = new System.IO.StreamWriter(@"results.csv");

            //Write the puzzle to the file
            for(int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    searchOutput.Write(MainWindow.puzzleMatrix[i, j] + " ");
                }
                searchOutput.WriteLine();
            }
            searchOutput.WriteLine();

            //Start the search timer
            searchTime.Start();

            //The fringe for the search, implemented as a queue for BFS
            Queue<StateNode> nodeQueue = new Queue<StateNode>();

            //Prime the working state with the initial state
            Array.Copy(MainWindow.puzzleMatrix, workingState, MainWindow.puzzleMatrix.Length);
            //The beginning of our path
            List<string> firstMove = new List<string>();
            firstMove.Add("Start, ");

            //Initial state node
            StateNode startState = new StateNode(workingState, moveCount, firstMove);
            //Temporary state node to hold our node to be expanded
            StateNode tempNode;

            //Push the initial state to the queue
            nodeQueue.Enqueue(startState);
            totalNodes++;

            //Navigate to results page
            MainWindow.results = new ResultsPage();
            this.NavigationService.Navigate(MainWindow.results);

            //Check to see if the puzzle has a solution
            bool solvable;
            int inversions = 0;
            //First write the values into a list
            List<int> arrayList = new List<int>(); 
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (MainWindow.puzzleMatrix[i, j] != 0)
                    {
                        arrayList.Add(MainWindow.puzzleMatrix[i, j]);
                    }
                }
            }

            //To check solvability we count the number of inversions,
            //i.e. the number of tiles that precede another tile with 
            //a lower number. If the number is even, the puzzle is
            //solvable. If the number is odd the puzzle is not solvable.
            for (int i = 0; i < arrayList.Count; i++)
            {
                for (int j = i + 1; j < arrayList.Count; j++)
                {
                    if (arrayList[j] < arrayList[i])
                    {
                        inversions++;
                    }
                }
            }
            if (inversions % 2 == 1)
            {
                solvable = false;
            }
            else
            {
                solvable = true;
            }

            //Search loop
            while(nodeQueue.Count() != 0)
            {
                //Get the first node from the queue for expansion
                tempNode = nodeQueue.Dequeue();

                //Copy the state in the first node in the Queue
                Array.Copy(tempNode.nodeState, workingState, tempNode.nodeState.Length);

                
                    
                    //Get the move counter from the node and increment it
                    moveCount = tempNode.nodeMoveCount;
                    moveCount++;

                    //Check for possible moves, in clockwise order (U,R,D,L)
                    //Locations inside the puzzle are checked individually
                    //First location check at [0,0]. 2 possible moves
                    if (workingState[0, 0] == 0)
                    {
                        //First move is [0,0] to [0,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [0,0] with [0,1]
                        int tempVal = new int();
                        tempVal = tempState[0, 0];
                        tempState[0, 0] = tempState[0, 1];
                        tempState[0, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path);
                        string nextMove = "00 to 01,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0,0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                            nodeQueue.Clear();
                            moveCount = newNode.nodeMoveCount;
                            break;
                        }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [0,0] to [1,0]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [0,0] with [0,1]
                        tempVal = tempState[0, 0];
                        tempState[0, 0] = tempState[1, 0];
                        tempState[1, 0] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        nextMove = "00 to 10,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                            nodeQueue.Clear();
                            moveCount = newNode.nodeMoveCount;
                            break;
                        }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Second location check at [0,1]. 3 possible moves
                    else if (workingState[0, 1] == 0)
                    {
                        //First move is [0,1] to [0,2]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [0,1] with [0,2]
                        int tempVal = new int();
                        tempVal = tempState[0, 1];
                        tempState[0, 1] = tempState[0, 2];
                        tempState[0, 2] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path);
                        string nextMove = "01 to 02,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                            nodeQueue.Clear();
                            moveCount = newNode.nodeMoveCount;
                            break;
                        }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [0,1] to [1,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [0,1] with [1,1]
                        tempVal = tempState[0, 1];
                        tempState[0, 1] = tempState[1, 1];
                        tempState[1, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        nextMove = "01 to 11,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                            nodeQueue.Clear();
                            moveCount = newNode.nodeMoveCount;
                            break;
                        }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Third move is [0,1] to [0,0]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [0,1] with [0,0]
                        tempVal = tempState[0, 1];
                        tempState[0, 1] = tempState[0, 0];
                        tempState[0, 0] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        nextMove = "01 to 00,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                            nodeQueue.Clear();
                            moveCount = newNode.nodeMoveCount;
                            break;
                        }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Third location check at [0,2]. 2 possible moves
                    else if (workingState[0, 2] == 0)
                    {
                        //First move is [0,2] to [1,2]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [0,2] with [1,2]
                        int tempVal = new int();
                        tempVal = tempState[0, 2];
                        tempState[0, 2] = tempState[1, 2];
                        tempState[1, 2] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path); ;
                        string nextMove = "02 to 12,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                            nodeQueue.Clear();
                            moveCount = newNode.nodeMoveCount;
                            break;
                        }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [0,2] to [0,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [0,2] with [0,1]
                        tempVal = tempState[0, 2];
                        tempState[0, 2] = tempState[0, 1];
                        tempState[0, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        nextMove = "02 to 01,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                            nodeQueue.Clear();
                            moveCount = newNode.nodeMoveCount;
                            break;
                        }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Fourth location check at [1,0]. 3 possible moves
                    else if (workingState[1, 0] == 0)
                    {
                        //First move is [1,0] to [0,0]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,0] with [0,0]
                        int tempVal = new int();
                        tempVal = tempState[1, 0];
                        tempState[1, 0] = tempState[0, 0];
                        tempState[0, 0] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path);
                        string nextMove = "10 to 00,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [1,0] to [1,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,0] with [1,1]
                        tempVal = tempState[1, 0];
                        tempState[1, 0] = tempState[1, 1];
                        tempState[1, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        nextMove = "10 to 11,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                            nodeQueue.Clear();
                            moveCount = newNode.nodeMoveCount;
                            break;
                        }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Third move is [1,0] to [2,0]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,0] with [2,0]
                        tempVal = tempState[1, 0];
                        tempState[1, 0] = tempState[2, 0];
                        tempState[2, 0] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        nextMove = "10 to 20,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Fifth location check at [1,1]. 4 possible moves
                    else if (workingState[1, 1] == 0)
                    {
                        //First move is [1,1] to [0,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,1] with [0,1]
                        int tempVal = new int();
                        tempVal = tempState[1, 1];
                        tempState[1, 1] = tempState[0, 1];
                        tempState[0, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path);
                        string nextMove = "11 to 01,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [1,1] to [1,2]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,1] with [1,2]
                        tempVal = tempState[1, 1];
                        tempState[1, 1] = tempState[1, 2];
                        tempState[1, 2] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        nextMove = "11 to 12,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Third move is [1,1] to [2,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,1] with [2,1]
                        tempVal = tempState[1, 1];
                        tempState[1, 1] = tempState[2, 1];
                        tempState[2, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        nextMove = "11 to 21,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Fourth move is [1,1] to [1,0]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,1] with [1,0]
                        tempVal = tempState[1, 1];
                        tempState[1, 1] = tempState[1, 0];
                        tempState[1, 0] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        nextMove = "11 to 10,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Sixth location check at [1,2]. 3 possible moves
                    else if (workingState[1, 2] == 0)
                    {
                        //First move is [1,2] to [0,2]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,2] with [0,2]
                        int tempVal = new int();
                        tempVal = tempState[1, 2];
                        tempState[1, 2] = tempState[0, 2];
                        tempState[0, 2] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path);
                        string nextMove = "12 to 02,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [1,2] to [2,2]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,2] with [2,2]
                        tempVal = tempState[1, 2];
                        tempState[1, 2] = tempState[2, 2];
                        tempState[2, 2] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        nextMove = "12 to 22,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Third move is [1,2] to [1,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,2] with [1,1]
                        tempVal = tempState[1, 2];
                        tempState[1, 2] = tempState[1, 1];
                        tempState[1, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        nextMove = "12 to 11,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Seventh location check at [2,0]. 2 possible moves
                    else if (workingState[2, 0] == 0)
                    {
                        //First move is [2,0] to [1,0]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [2,0] with [1,0]
                        int tempVal = new int();
                        tempVal = tempState[2, 0];
                        tempState[2, 0] = tempState[1, 0];
                        tempState[1, 0] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path);
                        string nextMove = "20 to 10,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [2,0] to [2,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [2,0] with [2,1]
                        tempVal = tempState[2, 0];
                        tempState[2, 0] = tempState[2, 1];
                        tempState[2, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        nextMove = "20 to 21,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Eighth location check at [2,1]. 3 possible moves
                    else if (workingState[2, 1] == 0)
                    {
                        //First move is [2,1] to [1,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [2,1] with [1,1]
                        int tempVal = new int();
                        tempVal = tempState[2, 1];
                        tempState[2, 1] = tempState[1, 1];
                        tempState[1, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path);
                        string nextMove = "21 to 11,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [2,1] to [2,2]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [2,1] with [2,2]
                        tempVal = tempState[2, 1];
                        tempState[2, 1] = tempState[2, 2];
                        tempState[2, 2] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        nextMove = "21 to 22,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Third move is [2,1] to [2,0]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [2,1] with [2,0]
                        tempVal = tempState[2, 1];
                        tempState[2, 1] = tempState[2, 0];
                        tempState[2, 0] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        nextMove = "21 to 20,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Ninth location check at [2,2]. 2 possible moves
                    else if (workingState[2, 2] == 0)
                    {
                        //First move is [2,2] to [1,2]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [2,2] with [1,2]
                        int tempVal = new int();
                        tempVal = tempState[2, 2];
                        tempState[2, 2] = tempState[1, 2];
                        tempState[1, 2] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path);
                        string nextMove = "22 to 12,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [2,2] to [2,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [2,2] with [2,1]
                        tempVal = tempState[2, 2];
                        tempState[2, 2] = tempState[2, 1];
                        tempState[2, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        nextMove = "22 to 21,";
                        tempList.Add(nextMove);

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeQueue.Enqueue(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Add the state to the list of visited states
                    if (!(visitedStates.ContainsKey(ArrayConvert(workingState))))
                    {
                        visitedStates.Add(ArrayConvert(workingState), workingState);
                    }
                
            }

            //Stop the search timer
            searchTime.Stop();

            //Close the file
            searchOutput.Close();

            if (solvable == true)
            {
                MainWindow.results.SolutionLabel.Content = "Solution found at depth " + moveCount.ToString() + " in " +
                    searchTime.Elapsed.ToString() + " seconds.";
                MainWindow.results.FileOutputLabel.Content = "Solution written to results.csv.";
            }
            else
            {
                MainWindow.results.SolutionLabel.Content = "No solution exists for this puzzle.";
                MainWindow.results.FileOutputLabel.Content = "Search took " + searchTime.Elapsed.ToString() + " seconds.";
            }

            MainWindow.results.NodesLabel.Content = totalNodes.ToString() + " nodes were generated.";
        }

        //Event handler for when the "Depth-First Search" button is clicked
        //This is the Depth-First Search
        private void DF_Search(object sender, RoutedEventArgs e)
        {
            //This is the working state
            int[,] workingState = new int[3, 3];
            //This is the temporary array for creating new states
            int[,] tempState = new int[3, 3];
            //Counters for the numbers of moves and nodes
            int moveCount = 0;
            int totalNodes = 0;
            Stopwatch searchTime = new Stopwatch(); //To track the search time
            //Dictionary to track visited states
            Dictionary<string, int[,]> visitedStates = new Dictionary<string, int[,]>();

            //An object for writing solutions to a data file
            System.IO.StreamWriter searchOutput = new System.IO.StreamWriter(@"results.csv");

            //Write the puzzle to the file
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    searchOutput.Write(MainWindow.puzzleMatrix[i, j] + " ");
                }
                searchOutput.WriteLine();
            }
            searchOutput.WriteLine();

            //Start the search timer
            searchTime.Start();

            //The fringe for the search, implemented as a stack for DFS
            Stack<StateNode> nodeStack = new Stack<StateNode>();

            //Prime the working state with the initial state
            Array.Copy(MainWindow.puzzleMatrix, workingState, MainWindow.puzzleMatrix.Length);
            //The beginning of our path
            List<string> firstMove = new List<string>();
            firstMove.Add("Start, ");

            //Initial state node
            StateNode startState = new StateNode(workingState, moveCount, firstMove);
            //Temporary state node to hold our node to be expanded
            StateNode tempNode;

            //Push the initial state to the queue
            nodeStack.Push(startState);
            totalNodes++;

            //Navigate to results page
            MainWindow.results = new ResultsPage();
            this.NavigationService.Navigate(MainWindow.results);

            //Check to see if the puzzle has a solution
            bool solvable;
            int inversions = 0;
            //First write the values into a list
            List<int> arrayList = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (MainWindow.puzzleMatrix[i, j] != 0)
                    {
                        arrayList.Add(MainWindow.puzzleMatrix[i, j]);
                    }
                }
            }

            //To check solvability we count the number of inversions,
            //i.e. the number of tiles that precede another tile with 
            //a lower number. If the number is even, the puzzle is
            //solvable. If the number is odd the puzzle is not solvable.
            for (int i = 0; i < arrayList.Count; i++)
            {
                for (int j = i + 1; j < arrayList.Count; j++)
                {
                    if (arrayList[j] < arrayList[i])
                    {
                        inversions++;
                    }
                }
            }
            if (inversions % 2 == 1)
            {
                solvable = false;
            }
            else
            {
                solvable = true;
            }

            //Search loop
            while (nodeStack.Count() != 0)
            {
                //Pop the first node from the stack for expansion
                tempNode = nodeStack.Pop();

                //Copy the state in expansion node
                Array.Copy(tempNode.nodeState, workingState, tempNode.nodeState.Length);

                                  
                    //Get the move counter from the node and increment it
                    moveCount = tempNode.nodeMoveCount;
                    moveCount++;

                    //Check for possible moves, in clockwise order (U,R,D,L)
                    //Locations inside the puzzle are checked individually
                    //First location check at [0,0]. 2 possible moves
                    if (workingState[0, 0] == 0)
                    {
                        //First move is [0,0] to [0,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [0,0] with [0,1]
                        int tempVal = new int();
                        tempVal = tempState[0, 0];
                        tempState[0, 0] = tempState[0, 1];
                        tempState[0, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path);
                        /*string nextMove = "00 to 01,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [0,0] to [1,0]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [0,0] with [0,1]
                        tempVal = tempState[0, 0];
                        tempState[0, 0] = tempState[1, 0];
                        tempState[1, 0] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        /*nextMove = "00 to 10,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Second location check at [0,1]. 3 possible moves
                    else if (workingState[0, 1] == 0)
                    {
                        //First move is [0,1] to [0,2]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [0,1] with [0,2]
                        int tempVal = new int();
                        tempVal = tempState[0, 1];
                        tempState[0, 1] = tempState[0, 2];
                        tempState[0, 2] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path);
                        /*string nextMove = "01 to 02,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [0,1] to [1,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [0,1] with [1,1]
                        tempVal = tempState[0, 1];
                        tempState[0, 1] = tempState[1, 1];
                        tempState[1, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        /*nextMove = "01 to 11,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Third move is [0,1] to [0,0]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [0,1] with [0,0]
                        tempVal = tempState[0, 1];
                        tempState[0, 1] = tempState[0, 0];
                        tempState[0, 0] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        /*nextMove = "01 to 00,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Third location check at [0,2]. 2 possible moves
                    else if (workingState[0, 2] == 0)
                    {
                        //First move is [0,2] to [1,2]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [0,2] with [1,2]
                        int tempVal = new int();
                        tempVal = tempState[0, 2];
                        tempState[0, 2] = tempState[1, 2];
                        tempState[1, 2] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path); ;
                        /*string nextMove = "02 to 12,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [0,2] to [0,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [0,2] with [0,1]
                        tempVal = tempState[0, 2];
                        tempState[0, 2] = tempState[0, 1];
                        tempState[0, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        /*nextMove = "02 to 01,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Fourth location check at [1,0]. 3 possible moves
                    else if (workingState[1, 0] == 0)
                    {
                        //First move is [1,0] to [0,0]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,0] with [0,0]
                        int tempVal = new int();
                        tempVal = tempState[1, 0];
                        tempState[1, 0] = tempState[0, 0];
                        tempState[0, 0] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path);
                        /*string nextMove = "10 to 00,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [1,0] to [1,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,0] with [1,1]
                        tempVal = tempState[1, 0];
                        tempState[1, 0] = tempState[1, 1];
                        tempState[1, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        /*nextMove = "10 to 11,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Third move is [1,0] to [2,0]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,0] with [2,0]
                        tempVal = tempState[1, 0];
                        tempState[1, 0] = tempState[2, 0];
                        tempState[2, 0] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        /*nextMove = "10 to 20,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Fifth location check at [1,1]. 4 possible moves
                    else if (workingState[1, 1] == 0)
                    {
                        //First move is [1,1] to [0,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,1] with [0,1]
                        int tempVal = new int();
                        tempVal = tempState[1, 1];
                        tempState[1, 1] = tempState[0, 1];
                        tempState[0, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path);
                        /*string nextMove = "11 to 01,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        };*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [1,1] to [1,2]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,1] with [1,2]
                        tempVal = tempState[1, 1];
                        tempState[1, 1] = tempState[1, 2];
                        tempState[1, 2] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        /*nextMove = "11 to 12,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Third move is [1,1] to [2,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,1] with [2,1]
                        tempVal = tempState[1, 1];
                        tempState[1, 1] = tempState[2, 1];
                        tempState[2, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        /*nextMove = "11 to 21,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Fourth move is [1,1] to [1,0]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,1] with [1,0]
                        tempVal = tempState[1, 1];
                        tempState[1, 1] = tempState[1, 0];
                        tempState[1, 0] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        /*nextMove = "11 to 10,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Sixth location check at [1,2]. 3 possible moves
                    else if (workingState[1, 2] == 0)
                    {
                        //First move is [1,2] to [0,2]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,2] with [0,2]
                        int tempVal = new int();
                        tempVal = tempState[1, 2];
                        tempState[1, 2] = tempState[0, 2];
                        tempState[0, 2] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path);
                        /*string nextMove = "12 to 02,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [1,2] to [2,2]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,2] with [2,2]
                        tempVal = tempState[1, 2];
                        tempState[1, 2] = tempState[2, 2];
                        tempState[2, 2] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        /*nextMove = "12 to 22,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        };*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Third move is [1,2] to [1,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [1,2] with [1,1]
                        tempVal = tempState[1, 2];
                        tempState[1, 2] = tempState[1, 1];
                        tempState[1, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        /*nextMove = "12 to 11,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Seventh location check at [2,0]. 2 possible moves
                    else if (workingState[2, 0] == 0)
                    {
                        //First move is [2,0] to [1,0]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [2,0] with [1,0]
                        int tempVal = new int();
                        tempVal = tempState[2, 0];
                        tempState[2, 0] = tempState[1, 0];
                        tempState[1, 0] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path);
                        /*string nextMove = "20 to 10,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [2,0] to [2,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [2,0] with [2,1]
                        tempVal = tempState[2, 0];
                        tempState[2, 0] = tempState[2, 1];
                        tempState[2, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        /*nextMove = "20 to 21,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                            //Write the path to the file
                            for (int i = 0; i < tempList.Count; i++)
                            {
                                searchOutput.Write(tempList[i]);
                            }
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Eighth location check at [2,1]. 3 possible moves
                    else if (workingState[2, 1] == 0)
                    {
                        //First move is [2,1] to [1,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [2,1] with [1,1]
                        int tempVal = new int();
                        tempVal = tempState[2, 1];
                        tempState[2, 1] = tempState[1, 1];
                        tempState[1, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(tempNode.path);
                        /*string nextMove = "21 to 11,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [2,1] to [2,2]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [2,1] with [2,2]
                        tempVal = tempState[2, 1];
                        tempState[2, 1] = tempState[2, 2];
                        tempState[2, 2] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        /*nextMove = "21 to 22,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Third move is [2,1] to [2,0]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [2,1] with [2,0]
                        tempVal = tempState[2, 1];
                        tempState[2, 1] = tempState[2, 0];
                        tempState[2, 0] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        /*nextMove = "21 to 20,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }

                    //Ninth location check at [2,2]. 2 possible moves
                    else if (workingState[2, 2] == 0)
                    {
                        //First move is [2,2] to [1,2]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [2,2] with [1,2]
                        int tempVal = new int();
                        tempVal = tempState[2, 2];
                        tempState[2, 2] = tempState[1, 2];
                        tempState[1, 2] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        List<string> tempList = new List<string>(nodeStack.Peek().path);
                        /*string nextMove = "22 to 12,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        StateNode newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }

                        //Second move is [2,2] to [2,1]
                        //Copy the array to the tempState to generate new potential states
                        Array.Copy(workingState, tempState, workingState.Length);

                        //Swap [2,2] with [2,1]
                        tempVal = tempState[2, 2];
                        tempState[2, 2] = tempState[2, 1];
                        tempState[2, 1] = tempVal;

                        //Copy the previous moves into the vector then add the next move
                        tempList = new List<string>(tempNode.path);
                        /*nextMove = "22 to 21,";
                        tempList.Add(nextMove);*/

                        //Generate a new node
                        newNode = new StateNode(tempState, moveCount, tempList);

                        //Test for the goal state
                        if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                            tempState[0, 1] == MainWindow.goalState[0, 1] &&
                            tempState[0, 2] == MainWindow.goalState[0, 2] &&
                            tempState[1, 0] == MainWindow.goalState[1, 0] &&
                            tempState[1, 1] == MainWindow.goalState[1, 1] &&
                            tempState[1, 2] == MainWindow.goalState[1, 2] &&
                            tempState[2, 0] == MainWindow.goalState[2, 0] &&
                            tempState[2, 1] == MainWindow.goalState[2, 1] &&
                            tempState[2, 2] == MainWindow.goalState[2, 2])
                        {
                        //Write the path to the file
                        /*for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }*/
                        nodeStack.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                        else
                        {
                            if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                            {
                                nodeStack.Push(newNode);
                                visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                                totalNodes++;
                            }
                        }
                    }                    
                
                //Add the state to the list of visited states
                if (!(visitedStates.ContainsKey(ArrayConvert(workingState))))
                {
                    visitedStates.Add(ArrayConvert(workingState), workingState);
                }
            }

            //Stop the search timer
            searchTime.Stop();

            //Close the file
            searchOutput.Close();

            if (solvable == true)
            {
                MainWindow.results.SolutionLabel.Content = "Solution found at depth " + moveCount.ToString() + " in " +
                    searchTime.Elapsed.ToString() + " seconds.";
            }
            else
            {
                MainWindow.results.SolutionLabel.Content = "No solution exists for this puzzle.";
                MainWindow.results.FileOutputLabel.Content = "Search took " + searchTime.Elapsed.ToString() + " seconds.";
            }

            MainWindow.results.NodesLabel.Content = totalNodes.ToString() + " nodes were generated.";
        }

        //Event handler for when the "A-Star Search with Displacement 
        //Heuristic" button is clicked
        //This is the A-Star Search using the displacement heuristic
        private void AStar_Displacement(object sender, RoutedEventArgs e)
        {
            //This is the working state
            int[,] workingState = new int[3, 3];
            //This is the temporary array for creating new states
            int[,] tempState = new int[3, 3];
            //Counters for the numbers of moves and nodes
            int moveCount = 0;
            int totalNodes = 0;
            Stopwatch searchTime = new Stopwatch(); //To track the search time
            //Dictionary to track visited states
            Dictionary<string, int[,]> visitedStates = new Dictionary<string, int[,]>();

            //An object for writing solutions to a data file
            System.IO.StreamWriter searchOutput = new System.IO.StreamWriter(@"results.csv");

            //Write the puzzle to the file
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    searchOutput.Write(MainWindow.puzzleMatrix[i, j] + " ");
                }
                searchOutput.WriteLine();
            }
            searchOutput.WriteLine();

            //Start the search timer
            searchTime.Start();

            //The fringe for the search, implemented as a priority queue for A*
            SimplePriorityQueue<StateNode> nodeQueue = new SimplePriorityQueue<StateNode>();

            //Prime the working state with the initial state
            Array.Copy(MainWindow.puzzleMatrix, workingState, MainWindow.puzzleMatrix.Length);
            //The beginning of our path
            List<string> firstMove = new List<string>();
            firstMove.Add("Start, ");

            //Initial state node
            StateNode startState = new StateNode(workingState, moveCount, firstMove);
            //Temporary state node to hold our node to be expanded
            StateNode tempNode;

            //Push the initial state to the queue
            nodeQueue.Enqueue(startState, (startState.nodeMoveCount + DisplacementCalc(startState.nodeState)));
            totalNodes++;

            //Navigate to results page
            MainWindow.results = new ResultsPage();
            this.NavigationService.Navigate(MainWindow.results);

            //Check to see if the puzzle has a solution
            bool solvable;
            int inversions = 0;
            //First write the values into a list
            List<int> arrayList = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (MainWindow.puzzleMatrix[i, j] != 0)
                    {
                        arrayList.Add(MainWindow.puzzleMatrix[i, j]);
                    }
                }
            }

            //To check solvability we count the number of inversions,
            //i.e. the number of tiles that precede another tile with 
            //a lower number. If the number is even, the puzzle is
            //solvable. If the number is odd the puzzle is not solvable.
            for (int i = 0; i < arrayList.Count; i++)
            {
                for (int j = i + 1; j < arrayList.Count; j++)
                {
                    if (arrayList[j] < arrayList[i])
                    {
                        inversions++;
                    }
                }
            }
            if (inversions % 2 == 1)
            {
                solvable = false;
            }
            else
            {
                solvable = true;
            }

            //Search loop
            while (nodeQueue.Count() != 0)
            {
                //Get the first node from the queue for expansion
                tempNode = nodeQueue.Dequeue();

                //Copy the state in the first node in the Queue
                Array.Copy(tempNode.nodeState, workingState, tempNode.nodeState.Length);



                //Get the move counter from the node and increment it
                moveCount = tempNode.nodeMoveCount;
                moveCount++;

                //Check for possible moves, in clockwise order (U,R,D,L)
                //Locations inside the puzzle are checked individually
                //First location check at [0,0]. 2 possible moves
                if (workingState[0, 0] == 0)
                {
                    //First move is [0,0] to [0,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [0,0] with [0,1]
                    int tempVal = new int();
                    tempVal = tempState[0, 0];
                    tempState[0, 0] = tempState[0, 1];
                    tempState[0, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "00 to 01,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [0,0] to [1,0]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [0,0] with [0,1]
                    tempVal = tempState[0, 0];
                    tempState[0, 0] = tempState[1, 0];
                    tempState[1, 0] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "00 to 10,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Second location check at [0,1]. 3 possible moves
                else if (workingState[0, 1] == 0)
                {
                    //First move is [0,1] to [0,2]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [0,1] with [0,2]
                    int tempVal = new int();
                    tempVal = tempState[0, 1];
                    tempState[0, 1] = tempState[0, 2];
                    tempState[0, 2] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "01 to 02,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [0,1] to [1,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [0,1] with [1,1]
                    tempVal = tempState[0, 1];
                    tempState[0, 1] = tempState[1, 1];
                    tempState[1, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "01 to 11,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Third move is [0,1] to [0,0]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [0,1] with [0,0]
                    tempVal = tempState[0, 1];
                    tempState[0, 1] = tempState[0, 0];
                    tempState[0, 0] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "01 to 00,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Third location check at [0,2]. 2 possible moves
                else if (workingState[0, 2] == 0)
                {
                    //First move is [0,2] to [1,2]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [0,2] with [1,2]
                    int tempVal = new int();
                    tempVal = tempState[0, 2];
                    tempState[0, 2] = tempState[1, 2];
                    tempState[1, 2] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path); ;
                    string nextMove = "02 to 12,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [0,2] to [0,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [0,2] with [0,1]
                    tempVal = tempState[0, 2];
                    tempState[0, 2] = tempState[0, 1];
                    tempState[0, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "02 to 01,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Fourth location check at [1,0]. 3 possible moves
                else if (workingState[1, 0] == 0)
                {
                    //First move is [1,0] to [0,0]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,0] with [0,0]
                    int tempVal = new int();
                    tempVal = tempState[1, 0];
                    tempState[1, 0] = tempState[0, 0];
                    tempState[0, 0] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "10 to 00,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [1,0] to [1,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,0] with [1,1]
                    tempVal = tempState[1, 0];
                    tempState[1, 0] = tempState[1, 1];
                    tempState[1, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "10 to 11,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Third move is [1,0] to [2,0]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,0] with [2,0]
                    tempVal = tempState[1, 0];
                    tempState[1, 0] = tempState[2, 0];
                    tempState[2, 0] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "10 to 20,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Fifth location check at [1,1]. 4 possible moves
                else if (workingState[1, 1] == 0)
                {
                    //First move is [1,1] to [0,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,1] with [0,1]
                    int tempVal = new int();
                    tempVal = tempState[1, 1];
                    tempState[1, 1] = tempState[0, 1];
                    tempState[0, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "11 to 01,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        };
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [1,1] to [1,2]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,1] with [1,2]
                    tempVal = tempState[1, 1];
                    tempState[1, 1] = tempState[1, 2];
                    tempState[1, 2] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "11 to 12,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Third move is [1,1] to [2,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,1] with [2,1]
                    tempVal = tempState[1, 1];
                    tempState[1, 1] = tempState[2, 1];
                    tempState[2, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "11 to 21,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Fourth move is [1,1] to [1,0]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,1] with [1,0]
                    tempVal = tempState[1, 1];
                    tempState[1, 1] = tempState[1, 0];
                    tempState[1, 0] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "11 to 10,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Sixth location check at [1,2]. 3 possible moves
                else if (workingState[1, 2] == 0)
                {
                    //First move is [1,2] to [0,2]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,2] with [0,2]
                    int tempVal = new int();
                    tempVal = tempState[1, 2];
                    tempState[1, 2] = tempState[0, 2];
                    tempState[0, 2] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "12 to 02,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [1,2] to [2,2]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,2] with [2,2]
                    tempVal = tempState[1, 2];
                    tempState[1, 2] = tempState[2, 2];
                    tempState[2, 2] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "12 to 22,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Third move is [1,2] to [1,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,2] with [1,1]
                    tempVal = tempState[1, 2];
                    tempState[1, 2] = tempState[1, 1];
                    tempState[1, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "12 to 11,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Seventh location check at [2,0]. 2 possible moves
                else if (workingState[2, 0] == 0)
                {
                    //First move is [2,0] to [1,0]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [2,0] with [1,0]
                    int tempVal = new int();
                    tempVal = tempState[2, 0];
                    tempState[2, 0] = tempState[1, 0];
                    tempState[1, 0] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "20 to 10,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [2,0] to [2,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [2,0] with [2,1]
                    tempVal = tempState[2, 0];
                    tempState[2, 0] = tempState[2, 1];
                    tempState[2, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "20 to 21,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Eighth location check at [2,1]. 3 possible moves
                else if (workingState[2, 1] == 0)
                {
                    //First move is [2,1] to [1,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [2,1] with [1,1]
                    int tempVal = new int();
                    tempVal = tempState[2, 1];
                    tempState[2, 1] = tempState[1, 1];
                    tempState[1, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "21 to 11,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [2,1] to [2,2]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [2,1] with [2,2]
                    tempVal = tempState[2, 1];
                    tempState[2, 1] = tempState[2, 2];
                    tempState[2, 2] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "21 to 22,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Third move is [2,1] to [2,0]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [2,1] with [2,0]
                    tempVal = tempState[2, 1];
                    tempState[2, 1] = tempState[2, 0];
                    tempState[2, 0] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "21 to 20,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Ninth location check at [2,2]. 2 possible moves
                else if (workingState[2, 2] == 0)
                {
                    //First move is [2,2] to [1,2]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [2,2] with [1,2]
                    int tempVal = new int();
                    tempVal = tempState[2, 2];
                    tempState[2, 2] = tempState[1, 2];
                    tempState[1, 2] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "22 to 12,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [2,2] to [2,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [2,2] with [2,1]
                    tempVal = tempState[2, 2];
                    tempState[2, 2] = tempState[2, 1];
                    tempState[2, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "22 to 21,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + DisplacementCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Add the state to the list of visited states
                if (!(visitedStates.ContainsKey(ArrayConvert(workingState))))
                {
                    visitedStates.Add(ArrayConvert(workingState), workingState);
                }

            }

            //Stop the search timer
            searchTime.Stop();

            //Close the file
            searchOutput.Close();

            if (solvable == true)
            {
                MainWindow.results.SolutionLabel.Content = "Solution found at depth " + moveCount.ToString() + " in " +
                    searchTime.Elapsed.ToString() + " seconds.";
                MainWindow.results.FileOutputLabel.Content = "Solution written to results.csv.";
            }
            else
            {
                MainWindow.results.SolutionLabel.Content = "No solution exists for this puzzle.";
                MainWindow.results.FileOutputLabel.Content = "Search took " + searchTime.Elapsed.ToString() + " seconds.";
            }

            MainWindow.results.NodesLabel.Content = totalNodes.ToString() + " nodes were generated.";
        }

        //Event handler for when the "A-Star Search with Manhattan 
        //Distance Heuristic" button is clicked
        //This is the A-Star Search using the Manhattan distance heuristic
        private void AStar_Manhattan(object sender, RoutedEventArgs e)
        {
            //This is the working state
            int[,] workingState = new int[3, 3];
            //This is the temporary array for creating new states
            int[,] tempState = new int[3, 3];
            //Counters for the numbers of moves and nodes
            int moveCount = 0;
            int totalNodes = 0;
            Stopwatch searchTime = new Stopwatch(); //To track the search time
            //Dictionary to track visited states
            Dictionary<string, int[,]> visitedStates = new Dictionary<string, int[,]>();

            //An object for writing solutions to a data file
            System.IO.StreamWriter searchOutput = new System.IO.StreamWriter(@"results.csv");

            //Write the puzzle to the file
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    searchOutput.Write(MainWindow.puzzleMatrix[i, j] + " ");
                }
                searchOutput.WriteLine();
            }
            searchOutput.WriteLine();

            //Start the search timer
            searchTime.Start();

            //The fringe for the search, implemented as a priority queue for A*
            SimplePriorityQueue<StateNode> nodeQueue = new SimplePriorityQueue<StateNode>();

            //Prime the working state with the initial state
            Array.Copy(MainWindow.puzzleMatrix, workingState, MainWindow.puzzleMatrix.Length);
            //The beginning of our path
            List<string> firstMove = new List<string>();
            firstMove.Add("Start, ");

            //Initial state node
            StateNode startState = new StateNode(workingState, moveCount, firstMove);
            //Temporary state node to hold our node to be expanded
            StateNode tempNode;

            //Push the initial state to the queue
            nodeQueue.Enqueue(startState, (startState.nodeMoveCount + ManhattanCalc(startState.nodeState)));
            totalNodes++;

            //Navigate to results page
            MainWindow.results = new ResultsPage();
            this.NavigationService.Navigate(MainWindow.results);

            //Check to see if the puzzle has a solution
            bool solvable;
            int inversions = 0;
            //First write the values into a list
            List<int> arrayList = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (MainWindow.puzzleMatrix[i, j] != 0)
                    {
                        arrayList.Add(MainWindow.puzzleMatrix[i, j]);
                    }
                }
            }

            //To check solvability we count the number of inversions,
            //i.e. the number of tiles that precede another tile with 
            //a lower number. If the number is even, the puzzle is
            //solvable. If the number is odd the puzzle is not solvable.
            for (int i = 0; i < arrayList.Count; i++)
            {
                for (int j = i + 1; j < arrayList.Count; j++)
                {
                    if (arrayList[j] < arrayList[i])
                    {
                        inversions++;
                    }
                }
            }
            if (inversions % 2 == 1)
            {
                solvable = false;
            }
            else
            {
                solvable = true;
            }

            //Search loop
            while (nodeQueue.Count() != 0)
            {
                //Get the first node from the queue for expansion
                tempNode = nodeQueue.Dequeue();

                //Copy the state in the first node in the Queue
                Array.Copy(tempNode.nodeState, workingState, tempNode.nodeState.Length);



                //Get the move counter from the node and increment it
                moveCount = tempNode.nodeMoveCount;
                moveCount++;

                //Check for possible moves, in clockwise order (U,R,D,L)
                //Locations inside the puzzle are checked individually
                //First location check at [0,0]. 2 possible moves
                if (workingState[0, 0] == 0)
                {
                    //First move is [0,0] to [0,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [0,0] with [0,1]
                    int tempVal = new int();
                    tempVal = tempState[0, 0];
                    tempState[0, 0] = tempState[0, 1];
                    tempState[0, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "00 to 01,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [0,0] to [1,0]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [0,0] with [0,1]
                    tempVal = tempState[0, 0];
                    tempState[0, 0] = tempState[1, 0];
                    tempState[1, 0] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "00 to 10,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Second location check at [0,1]. 3 possible moves
                else if (workingState[0, 1] == 0)
                {
                    //First move is [0,1] to [0,2]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [0,1] with [0,2]
                    int tempVal = new int();
                    tempVal = tempState[0, 1];
                    tempState[0, 1] = tempState[0, 2];
                    tempState[0, 2] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "01 to 02,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [0,1] to [1,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [0,1] with [1,1]
                    tempVal = tempState[0, 1];
                    tempState[0, 1] = tempState[1, 1];
                    tempState[1, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "01 to 11,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Third move is [0,1] to [0,0]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [0,1] with [0,0]
                    tempVal = tempState[0, 1];
                    tempState[0, 1] = tempState[0, 0];
                    tempState[0, 0] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "01 to 00,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Third location check at [0,2]. 2 possible moves
                else if (workingState[0, 2] == 0)
                {
                    //First move is [0,2] to [1,2]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [0,2] with [1,2]
                    int tempVal = new int();
                    tempVal = tempState[0, 2];
                    tempState[0, 2] = tempState[1, 2];
                    tempState[1, 2] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path); ;
                    string nextMove = "02 to 12,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [0,2] to [0,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [0,2] with [0,1]
                    tempVal = tempState[0, 2];
                    tempState[0, 2] = tempState[0, 1];
                    tempState[0, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "02 to 01,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Fourth location check at [1,0]. 3 possible moves
                else if (workingState[1, 0] == 0)
                {
                    //First move is [1,0] to [0,0]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,0] with [0,0]
                    int tempVal = new int();
                    tempVal = tempState[1, 0];
                    tempState[1, 0] = tempState[0, 0];
                    tempState[0, 0] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "10 to 00,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [1,0] to [1,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,0] with [1,1]
                    tempVal = tempState[1, 0];
                    tempState[1, 0] = tempState[1, 1];
                    tempState[1, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "10 to 11,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Third move is [1,0] to [2,0]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,0] with [2,0]
                    tempVal = tempState[1, 0];
                    tempState[1, 0] = tempState[2, 0];
                    tempState[2, 0] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "10 to 20,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Fifth location check at [1,1]. 4 possible moves
                else if (workingState[1, 1] == 0)
                {
                    //First move is [1,1] to [0,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,1] with [0,1]
                    int tempVal = new int();
                    tempVal = tempState[1, 1];
                    tempState[1, 1] = tempState[0, 1];
                    tempState[0, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "11 to 01,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        };
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [1,1] to [1,2]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,1] with [1,2]
                    tempVal = tempState[1, 1];
                    tempState[1, 1] = tempState[1, 2];
                    tempState[1, 2] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "11 to 12,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Third move is [1,1] to [2,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,1] with [2,1]
                    tempVal = tempState[1, 1];
                    tempState[1, 1] = tempState[2, 1];
                    tempState[2, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "11 to 21,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Fourth move is [1,1] to [1,0]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,1] with [1,0]
                    tempVal = tempState[1, 1];
                    tempState[1, 1] = tempState[1, 0];
                    tempState[1, 0] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "11 to 10,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Sixth location check at [1,2]. 3 possible moves
                else if (workingState[1, 2] == 0)
                {
                    //First move is [1,2] to [0,2]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,2] with [0,2]
                    int tempVal = new int();
                    tempVal = tempState[1, 2];
                    tempState[1, 2] = tempState[0, 2];
                    tempState[0, 2] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "12 to 02,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [1,2] to [2,2]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,2] with [2,2]
                    tempVal = tempState[1, 2];
                    tempState[1, 2] = tempState[2, 2];
                    tempState[2, 2] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "12 to 22,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        };
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Third move is [1,2] to [1,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [1,2] with [1,1]
                    tempVal = tempState[1, 2];
                    tempState[1, 2] = tempState[1, 1];
                    tempState[1, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "12 to 11,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Seventh location check at [2,0]. 2 possible moves
                else if (workingState[2, 0] == 0)
                {
                    //First move is [2,0] to [1,0]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [2,0] with [1,0]
                    int tempVal = new int();
                    tempVal = tempState[2, 0];
                    tempState[2, 0] = tempState[1, 0];
                    tempState[1, 0] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "20 to 10,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [2,0] to [2,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [2,0] with [2,1]
                    tempVal = tempState[2, 0];
                    tempState[2, 0] = tempState[2, 1];
                    tempState[2, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "20 to 21,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Eighth location check at [2,1]. 3 possible moves
                else if (workingState[2, 1] == 0)
                {
                    //First move is [2,1] to [1,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [2,1] with [1,1]
                    int tempVal = new int();
                    tempVal = tempState[2, 1];
                    tempState[2, 1] = tempState[1, 1];
                    tempState[1, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "21 to 11,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [2,1] to [2,2]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [2,1] with [2,2]
                    tempVal = tempState[2, 1];
                    tempState[2, 1] = tempState[2, 2];
                    tempState[2, 2] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "21 to 22,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Third move is [2,1] to [2,0]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [2,1] with [2,0]
                    tempVal = tempState[2, 1];
                    tempState[2, 1] = tempState[2, 0];
                    tempState[2, 0] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "21 to 20,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Ninth location check at [2,2]. 2 possible moves
                else if (workingState[2, 2] == 0)
                {
                    //First move is [2,2] to [1,2]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [2,2] with [1,2]
                    int tempVal = new int();
                    tempVal = tempState[2, 2];
                    tempState[2, 2] = tempState[1, 2];
                    tempState[1, 2] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    List<string> tempList = new List<string>(tempNode.path);
                    string nextMove = "22 to 12,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    StateNode newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }

                    //Second move is [2,2] to [2,1]
                    //Copy the array to the tempState to generate new potential states
                    Array.Copy(workingState, tempState, workingState.Length);

                    //Swap [2,2] with [2,1]
                    tempVal = tempState[2, 2];
                    tempState[2, 2] = tempState[2, 1];
                    tempState[2, 1] = tempVal;

                    //Copy the previous moves into the vector then add the next move
                    tempList = new List<string>(tempNode.path);
                    nextMove = "22 to 21,";
                    tempList.Add(nextMove);

                    //Generate a new node
                    newNode = new StateNode(tempState, moveCount, tempList);

                    //Test for the goal state
                    if (tempState[0, 0] == MainWindow.goalState[0, 0] &&
                        tempState[0, 1] == MainWindow.goalState[0, 1] &&
                        tempState[0, 2] == MainWindow.goalState[0, 2] &&
                        tempState[1, 0] == MainWindow.goalState[1, 0] &&
                        tempState[1, 1] == MainWindow.goalState[1, 1] &&
                        tempState[1, 2] == MainWindow.goalState[1, 2] &&
                        tempState[2, 0] == MainWindow.goalState[2, 0] &&
                        tempState[2, 1] == MainWindow.goalState[2, 1] &&
                        tempState[2, 2] == MainWindow.goalState[2, 2])
                    {
                        //Write the path to the file
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            searchOutput.Write(tempList[i]);
                        }
                        nodeQueue.Clear();
                        moveCount = newNode.nodeMoveCount;
                        break;
                    }
                    else
                    {
                        if (!(visitedStates.ContainsKey(ArrayConvert(newNode.nodeState))))
                        {
                            nodeQueue.Enqueue(newNode, (newNode.nodeMoveCount + ManhattanCalc(newNode.nodeState)));
                            visitedStates.Add(ArrayConvert(newNode.nodeState), newNode.nodeState);
                            totalNodes++;
                        }
                    }
                }

                //Add the state to the list of visited states
                if (!(visitedStates.ContainsKey(ArrayConvert(workingState))))
                {
                    visitedStates.Add(ArrayConvert(workingState), workingState);
                }

            }

            //Stop the search timer
            searchTime.Stop();

            //Close the file
            searchOutput.Close();

            if (solvable == true)
            {
                MainWindow.results.SolutionLabel.Content = "Solution found at depth " + moveCount.ToString() + " in " +
                    searchTime.Elapsed.ToString() + " seconds.";
                MainWindow.results.FileOutputLabel.Content = "Solution written to results.csv.";
            }
            else
            {
                MainWindow.results.SolutionLabel.Content = "No solution exists for this puzzle.";
                MainWindow.results.FileOutputLabel.Content = "Search took " + searchTime.Elapsed.ToString() + " seconds.";
            }

            MainWindow.results.NodesLabel.Content = totalNodes.ToString() + " nodes were generated.";
        }

        //Function for converting our 2d array to a string
        public string ArrayConvert(int[,] toBeConverted)
        {
            string converted = new string('\0', 0);

            for(int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    converted += toBeConverted[i, j].ToString();
                }
            }
            return converted;
        }

        //Function for calculating the displacement heuristic
        public int DisplacementCalc(int[,] puzzle)
        {
            int numDisplaced = 0;

            if (puzzle[0, 0] != 1)
                numDisplaced++;
            if (puzzle[0, 1] != 2)
                numDisplaced++;
            if (puzzle[0, 2] != 3)
                numDisplaced++;
            if (puzzle[1, 0] != 4)
                numDisplaced++;
            if (puzzle[1, 1] != 5)
                numDisplaced++;
            if (puzzle[1, 2] != 6)
                numDisplaced++;
            if (puzzle[2, 0] != 7)
                numDisplaced++;
            if (puzzle[2, 1] != 8)
                numDisplaced++;

            return numDisplaced;
        }

        //Function for calculating the Manhattan distance heuristic
        public int ManhattanCalc(int [,] puzzle)
        {
            int manhattanDist = 0;

            for (int i = 0; i < puzzle.GetLength(0); i++)
            {
                for (int j = 0; j < puzzle.GetLength(1); j++)
                {
                    int elem = puzzle[i, j];
                    //Ignore the empty space
                    if (elem == 0) continue;

                    bool found = false;
                    for (int h = 0; h < puzzle.GetLength(0); h++)
                    {
                        for (int k = 0; k < puzzle.GetLength(1); k++)
                        {
                            if (MainWindow.goalState[h, k].Equals(elem))
                            {
                                manhattanDist += Math.Abs(h - i) + Math.Abs(j - k);
                                found = true;
                                break;
                            }
                        }
                        if (found) break;
                    }
                }
            }
            return manhattanDist;
        }

        //A struct for state information
        public struct StateNode
        {
            public int[,] nodeState;
            public int nodeMoveCount;
            public List<string> path;

            //Constructor
            public StateNode(int[,] newState, int newCount, List<string> nextMove)
            {
                //Initialize our variables
                nodeState = new int[3, 3];
                nodeMoveCount = new int();
                path = new List<string>();

                //Add the new information
                Array.Copy(newState, nodeState, newState.Length);
                nodeMoveCount = newCount;
                path = nextMove;
            }
        }

        //A second struct for use with the A* Searches
        public struct AStarNode
        {
            public int[,] nodeState;
            public int nodeMoveCount;
            public int heuristic;
            public List<string> path;

            //Constructor
            public AStarNode(int[,] newState, int newCount, int newHeur, List<string> nextMove)
            {
                //Initialize our variables
                nodeState = new int[3, 3];
                nodeMoveCount = new int();
                heuristic = new int();
                path = new List<string>();

                //Add the new information
                Array.Copy(newState, nodeState, newState.Length);
                nodeMoveCount = newCount;
                heuristic = newHeur;
                path = nextMove;
            }
        }
    }
}
