using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MazeSolverStack
{
    public class MazeSolverUsingStack
    {
        public MazeSolverUsingStack()
        {
            MazeRows = new Stack<string>();
            MazeStack = new Stack<MazeCell>();
            ExitCells = new Queue<MazeCell>();
        }
        public Stack<string> MazeRows { get; set; }
        public Stack<MazeCell> MazeStack { get; set; }
        public MazeCell ExitCell { get; set; }
        public MazeCell EntryCell { get; set; }
        public MazeCell CurrentCell { get; set; }
        public Queue<MazeCell> ExitCells { get; set; }

        public void DisplayMaze(MazeCell[,] mazeArray)
        {
            for (int y = 0; y < mazeArray.GetLength(0); y++)
            {
                for (int x = 0; x < mazeArray.GetLength(1); x++)
                {
                    Console.Write(mazeArray[y,x].Character);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void SolveMazeOneExitOld(MazeCell[,] mazeArray)
        {
            for (int y = 0; y < mazeArray.GetLength(0); y++)
            {
                for (int x = 0; x < mazeArray.GetLength(1); x++)
                {
                    if (mazeArray[y, x].Character == 'm') EntryCell = mazeArray[y, x];
                    if (mazeArray[y, x].Character == 'e') ExitCell = mazeArray[y, x];
                }
            }

            CurrentCell = EntryCell;

            while (CurrentCell != ExitCell)
            {
                if (CurrentCell.Character == 'e') CurrentCell = ExitCell;
                else CurrentCell.Character = '.';

                if (mazeArray[CurrentCell.Y + 1, CurrentCell.X].Character == '0' || mazeArray[CurrentCell.Y + 1, CurrentCell.X].Character == 'e')
                {
                    MazeStack.Push(mazeArray[CurrentCell.Y + 1, CurrentCell.X]);
                }
                if (mazeArray[CurrentCell.Y - 1, CurrentCell.X].Character == '0' || mazeArray[CurrentCell.Y - 1, CurrentCell.X].Character == 'e')
                {
                    MazeStack.Push(mazeArray[CurrentCell.Y - 1, CurrentCell.X]);
                }
                if (mazeArray[CurrentCell.Y, CurrentCell.X - 1].Character == '0' || mazeArray[CurrentCell.Y, CurrentCell.X - 1].Character == 'e')
                {
                    MazeStack.Push(mazeArray[CurrentCell.Y, CurrentCell.X - 1]);
                }
                if (mazeArray[CurrentCell.Y, CurrentCell.X + 1].Character == '0' || mazeArray[CurrentCell.Y, CurrentCell.X + 1].Character == 'e')
                {
                    MazeStack.Push(mazeArray[CurrentCell.Y, CurrentCell.X + 1]);
                }

                DisplayMaze(mazeArray);

                if (MazeStack.Count <= 0)
                {
                    Console.WriteLine("Unsolvable maze.");
                    return;
                }
                else
                {
                    CurrentCell = MazeStack.Pop();
                }
            }

            Console.WriteLine("Exit reached.");
        }

        public void SolveMazeMultipleExitsOld(MazeCell[,] mazeArray)
        {
            bool NotEntryCell = false;
            //ExitCellCount = 0;
            var foundExitCells = 0;

            InitializeMaze(mazeArray);
            bool ExitMethodFlag = false;
            if (ExitCells.Count > 1)
            {
                Console.WriteLine("Would you like to continue search from last exit found(1), or start from entrance(2)?");
                var answer = Console.ReadLine();
                if (answer == "1")
                {
                    ExitMethodFlag = true;
                }
            }

            CurrentCell = EntryCell;
            var storedMazeArray = new MazeCell[mazeArray.GetLength(0), mazeArray.GetLength(1)];
            for (int y = 0; y < mazeArray.GetLength(0); y++)
            {
                for (int x = 0; x < mazeArray.GetLength(1); x++)
                {
                    storedMazeArray[y, x] = new MazeCell(mazeArray[y, x].X, mazeArray[y, x].Y, mazeArray[y, x].Character);
                }
            }

            while (foundExitCells != ExitCells.Count)
            {
                if (NotEntryCell == false)
                {
                    NotEntryCell = true;
                }
                else
                {
                    if (CurrentCell.Character == 'e')
                    {
                        foundExitCells++;
                        storedMazeArray[CurrentCell.Y, CurrentCell.X].Character = '@';
                        mazeArray = new MazeCell[mazeArray.GetLength(0),mazeArray.GetLength(1)];

                        for (int y = 0; y < mazeArray.GetLength(0); y++)
                        {
                            for (int x = 0; x < mazeArray.GetLength(1); x++)
                            {
                                mazeArray[y,x] = new MazeCell(storedMazeArray[y,x].X, storedMazeArray[y,x].Y, storedMazeArray[y,x].Character);
                            }
                        }
                        Console.WriteLine("Refreshed array");

                        if (ExitMethodFlag == true)
                        {
                            CurrentCell = mazeArray[CurrentCell.Y, CurrentCell.X];
                            MazeStack = new Stack<MazeCell>();
                        }
                        else
                        {
                            CurrentCell = mazeArray[EntryCell.Y, EntryCell.X];
                            MazeStack = new Stack<MazeCell>();
                        }
                    }
                    else
                    {
                        CurrentCell.Character = '.';
                    }
                }

                if (mazeArray[CurrentCell.Y + 1, CurrentCell.X].Character == '0' || mazeArray[CurrentCell.Y + 1, CurrentCell.X].Character == 'e')
                {
                    MazeStack.Push(mazeArray[CurrentCell.Y + 1, CurrentCell.X]);
                }
                if (mazeArray[CurrentCell.Y - 1, CurrentCell.X].Character == '0' || mazeArray[CurrentCell.Y - 1, CurrentCell.X].Character == 'e')
                {
                    MazeStack.Push(mazeArray[CurrentCell.Y - 1, CurrentCell.X]);
                }
                if (mazeArray[CurrentCell.Y, CurrentCell.X - 1].Character == '0' || mazeArray[CurrentCell.Y, CurrentCell.X - 1].Character == 'e')
                {
                    MazeStack.Push(mazeArray[CurrentCell.Y, CurrentCell.X - 1]);
                }
                if (mazeArray[CurrentCell.Y, CurrentCell.X + 1].Character == '0' || mazeArray[CurrentCell.Y, CurrentCell.X + 1].Character == 'e')
                {
                    MazeStack.Push(mazeArray[CurrentCell.Y, CurrentCell.X + 1]);
                }

                DisplayMaze(mazeArray);

                if (MazeStack.Count <= 0)
                {
                    if (foundExitCells > 0)
                    {
                        Console.WriteLine("Only " + foundExitCells +
                                          " were/was found. Other exits are unreachable.");
                    }
                    else
                    {
                        Console.WriteLine("Unsolvable maze.");
                    }
                    break;
                }
                else
                {
                    CurrentCell = MazeStack.Pop();
                }
            }
        }

        public void SolveMazeMultipleExits(MazeCell[,] mazeArray)
        {
            InitializeMaze(mazeArray);
            var storedMazeArray = new MazeCell[mazeArray.GetLength(0), mazeArray.GetLength(1)];
            CopyMaze(storedMazeArray, mazeArray);

            while (ExitCells.Count > 0)
            {
                ExitCell = ExitCells.Dequeue();
                mazeArray[ExitCell.Y, ExitCell.X].IsVisited = false;
                CurrentCell = EntryCell;
                var mazePath = FindLongestPath(mazeArray);

                if (mazePath == null)
                {
                    Console.WriteLine($"Could not find path to {ExitCell}.");
                    Console.WriteLine();
                    mazeArray[ExitCell.Y, ExitCell.X].Character = 'x';
                    mazeArray[ExitCell.Y, ExitCell.X].IsVisited = true;
                    DisplayMaze(mazeArray);
                    MazeStack.Clear();
                }
                else
                {
                    mazePath[0].IsVisited = true;
                    mazePath.Reverse();
                    mazePath.RemoveAt(0);
                    foreach (var mazeCell in mazePath)
                    {
                        if (mazeCell.Character != ExitCell.Character) mazeArray[mazeCell.Y, mazeCell.X].Character = '.';
                        else
                        {
                            mazeArray[mazeCell.Y, mazeCell.X].Character = '@';
                            ExitCell.Character = mazeArray[mazeCell.Y, mazeCell.X].Character;
                        }
                        DisplayMaze(mazeArray);
                    }
                    Console.WriteLine($"Longest path from {EntryCell} to {ExitCell} found.");
                    Console.WriteLine();
                    MazeStack.Clear();
                    storedMazeArray[ExitCell.Y, ExitCell.X] = ExitCell;
                    CopyMaze(mazeArray, storedMazeArray);
                }
            }
        }

        private List<MazeCell> FindLongestPath(MazeCell[,] mazeArray)
        {
            var mazePath = FindLongestPath(CurrentCell, ExitCell, mazeArray);
            return mazePath;
        }
        private List<MazeCell> FindLongestPath(MazeCell currentCell, MazeCell exitCell, MazeCell[,] mazeArray)
        {
            List<MazeCell> longestPath = null;
            if (currentCell.Y < 0 || currentCell.X < 0) return null;
            if (currentCell.Y == mazeArray.GetLength(0) || currentCell.X == mazeArray.GetLength(1)) return null;
            if (mazeArray[currentCell.Y, currentCell.X].IsVisited == true) return null;
            if (currentCell.Character == exitCell.Character)
            {
                var longPath = new List<MazeCell>();     
                longPath.Add(currentCell);
                return longPath;
            }

            mazeArray[currentCell.Y, currentCell.X].IsVisited = true;
            var nextCells = new List<MazeCell>();
            if (mazeArray[currentCell.Y - 1, currentCell.X].IsVisited == false)
            {                                                                 
                nextCells.Add(mazeArray[currentCell.Y - 1, currentCell.X]);   
            }                                                                 
            if (mazeArray[currentCell.Y + 1, currentCell.X].IsVisited == false)
            {                                                                 
                nextCells.Add(mazeArray[currentCell.Y + 1, currentCell.X]);   
            }                                                                 
            if (mazeArray[currentCell.Y, currentCell.X - 1].IsVisited == false)
            {                                                                 
                nextCells.Add(mazeArray[currentCell.Y, currentCell.X - 1]);   
                                                                              
            }                                                                 
            if (mazeArray[currentCell.Y, currentCell.X + 1].IsVisited == false)
            {
                nextCells.Add(mazeArray[currentCell.Y, currentCell.X + 1]);
            }

            var maxLength = -1;
            foreach (var nextCell in nextCells)
            {
                var longPath = FindLongestPath(nextCell, exitCell, mazeArray);
                if (longPath != null && longPath.Count > maxLength)
                {
                    maxLength = longPath.Count;
                    longPath.Add(currentCell);
                    longestPath = longPath;
                }
            }
            mazeArray[currentCell.Y, currentCell.X].IsVisited = false;
            if (longestPath == null || longestPath.Count == 0) return null;
            return longestPath;
        }
        private void InitializeMaze(MazeCell[,] mazeArray)
        {
            foreach (var mazeCell in mazeArray)
            {
                if (mazeCell.Character == 'm')
                {
                    EntryCell = mazeCell;
                    CurrentCell = EntryCell;
                }
                if (mazeCell.Character == 'e')
                {
                    ExitCells.Enqueue(mazeCell);
                }
                if (mazeCell.Character == '0' || mazeCell.Character == 'm') mazeCell.IsVisited = false;
                else mazeCell.IsVisited = true;
                MazeStack.Clear();
            }
        }

        /// <summary>
        /// Copies a maze array.
        /// </summary>
        /// <param name="mazeArray">The resulting copy of the maze.</param>
        /// <param name="mazeToBeCopied">The maze array to be copied.</param>
        private void CopyMaze(MazeCell[,] mazeArray, MazeCell[,] mazeToBeCopied)
        {
            for (int y = 0; y < mazeArray.GetLength(0); y++)
            {
                for (int x = 0; x < mazeArray.GetLength(1); x++)
                {
                    mazeArray[y, x] = new MazeCell(mazeToBeCopied[y, x].X, mazeToBeCopied[y, x].Y, mazeToBeCopied[y, x].Character);
                    mazeArray[y, x].IsVisited = mazeToBeCopied[y, x].IsVisited;
                }
            }
        }
    }
}