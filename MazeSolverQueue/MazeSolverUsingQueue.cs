using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MazeSolverQueue
{
    public class MazeSolverUsingQueue
    {
        public MazeSolverUsingQueue()
        {
            MazeRows = new Stack<string>();
            MazeQueue = new Queue<MazeCell>();
            ExitCells = new Queue<MazeCell>();
            //Predecessors = new Queue<MazeCell>();
        }
        //public Queue<MazeCell> Predecessors { get; set; }
        public Stack<string> MazeRows { get; set; }
        public Queue<MazeCell> MazeQueue { get; set; }
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
                    Console.Write(mazeArray[y, x].Character);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        
        public void SolveMazeOneExit(MazeCell[,] mazeArray)
        {
            InitializeMaze(mazeArray);
            ExitCell = ExitCells.Dequeue();
            var mazePath = FindShortestPath(EntryCell, ExitCell, CurrentCell, MazeQueue, mazeArray);
            if (mazePath == null)
            {
                Console.WriteLine("Maze is unsolvable.");
            }
            else
            {
                foreach (var mazeCell in mazePath)
                {
                    if (mazeCell != ExitCell || mazeCell == EntryCell) mazeArray[mazeCell.Y, mazeCell.X].Character = '.';
                    else mazeArray[mazeCell.Y, mazeCell.X].Character = '@';
                    DisplayMaze(mazeArray);
                }
                
                Console.WriteLine($"Shortest path to {ExitCell} from {EntryCell} found.");
            }
            
        }
        public void SolveMazeMultipleExits(MazeCell[,] mazeArray)
        {
            InitializeMaze(mazeArray);
            var storedMazeArray = new MazeCell[mazeArray.GetLength(0), mazeArray.GetLength(1)];
            CopyMaze(storedMazeArray, mazeArray);

            while (ExitCells.Count != 0)
            {
                ExitCell = ExitCells.Dequeue();
                mazeArray[ExitCell.Y, ExitCell.X].IsVisited = false;
                CurrentCell = EntryCell;
                var mazePath = FindShortestPath(EntryCell, ExitCell, CurrentCell, MazeQueue, mazeArray);
                if (mazePath == null)
                {
                    Console.WriteLine($"Could not find path to {ExitCell}.");
                    mazeArray[ExitCell.Y, ExitCell.X].Character = 'x';
                    mazeArray[ExitCell.Y, ExitCell.X].IsVisited = true;
                    MazeQueue.Clear();
                }
                else
                {
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
                    Console.WriteLine($"Shortest path from {EntryCell} to {ExitCell} found.");
                    Console.WriteLine();
                    MazeQueue.Clear();
                    storedMazeArray[ExitCell.Y, ExitCell.X] = ExitCell;
                    CopyMaze(mazeArray, storedMazeArray);
                }
            }

        }
        private List<MazeCell> FindShortestPath(MazeCell entryCell, MazeCell exitCell, MazeCell currentCell, Queue<MazeCell> mazeQueue, MazeCell[,] mazeArray)
        {
            currentCell = entryCell;
            mazeQueue.Enqueue(currentCell);
            while (mazeQueue.Count > 0)
            {
                currentCell = MazeQueue.Dequeue();
                mazeArray[currentCell.Y, currentCell.X].IsVisited = true;
                if (currentCell == exitCell)
                {
                    break;
                }

                if (mazeArray[currentCell.Y - 1, currentCell.X].IsVisited == false)
                {
                    mazeArray[currentCell.Y - 1, currentCell.X].Parent = mazeArray[currentCell.Y, currentCell.X];
                    mazeQueue.Enqueue(mazeArray[currentCell.Y - 1, currentCell.X]);
                }
                if (mazeArray[currentCell.Y + 1, currentCell.X].IsVisited == false)
                {
                    mazeArray[currentCell.Y + 1, currentCell.X].Parent = mazeArray[currentCell.Y, currentCell.X];
                    mazeQueue.Enqueue(mazeArray[currentCell.Y + 1, currentCell.X]);
                }
                if (mazeArray[currentCell.Y, currentCell.X - 1].IsVisited == false)
                {
                    mazeArray[currentCell.Y, currentCell.X - 1].Parent = mazeArray[currentCell.Y, currentCell.X];
                    mazeQueue.Enqueue(mazeArray[currentCell.Y, currentCell.X - 1]);

                }
                if (mazeArray[currentCell.Y, currentCell.X + 1].IsVisited == false)
                {
                    mazeArray[currentCell.Y, currentCell.X + 1].Parent = mazeArray[currentCell.Y, currentCell.X];
                    mazeQueue.Enqueue(mazeArray[currentCell.Y, currentCell.X + 1]);
                }

            }
            var mazePath = new List<MazeCell>();
            var start = mazeArray[entryCell.Y, entryCell.X];
            var exit = mazeArray[exitCell.Y, exitCell.X];
            mazePath.Add(exit);
            while (exit != start)
            {
                if (exit.Parent == null) return null;
                mazePath.Add(exit.Parent);
                exit = exit.Parent;
            }
            mazePath.Reverse();
            mazePath.RemoveAt(0);
            return mazePath;
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
                MazeQueue.Clear();
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
                    mazeArray[y, x] = new MazeCell(mazeToBeCopied[y, x].X, mazeToBeCopied[y, x].Y, mazeToBeCopied[y,x].Character);
                    mazeArray[y, x].IsVisited = mazeToBeCopied[y, x].IsVisited;
                }
            }
        }
    }
}