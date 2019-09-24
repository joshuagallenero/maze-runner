using System.Collections.Generic;
using System.Windows;
using MazeSolverQueue;

namespace MazeSolverVisualizer
{
    public class MazeSolverLongestPath
    {
        public static MazeCell ExitCell { get; set; }
        public static MazeCell EntryCell { get; set; }
        public static MazeCell CurrentCell { get; set; }
        public static Queue<MazeCell> ExitCells { get; set; } = new Queue<MazeCell>();
        public static MazeCell[,] MazeToBeShowed { get; set; }
        public static MazeCell[,] StoredMazeToBeShowed { get; set; }
        public static MazeCell[,] MazeArray { get; set; }
        public static MazeCell[,] StoredMazeArray { get; set; }
        public static List<MazeCell> MazePath { get; set; }
        public static int CellsInMazePath { get; set; }
        public static bool MouseChecker { get; set; }
        public static bool ExitChecker { get; set; }
        public static void CreateMazeArray(char[,] canvasMazeArray)
        {
            MouseChecker = false;
            ExitChecker = false;
            MazeArray = new MazeCell[canvasMazeArray.GetLength(0) + 2, canvasMazeArray.GetLength(1) + 2];
            MazeToBeShowed = new MazeCell[canvasMazeArray.GetLength(0), canvasMazeArray.GetLength(1)];
            for (int y = 1; y < MazeArray.GetLength(0) - 1; y++)
            {
                for (int x = 1; x < MazeArray.GetLength(1) - 1; x++)
                {
                    MazeArray[y, x] = new MazeCell(x, y, canvasMazeArray[y - 1, x - 1]);
                    MazeToBeShowed[y - 1, x - 1] = new MazeCell(x - 1, y - 1, canvasMazeArray[y - 1, x - 1]);
                }
            }

            for (int x = 0; x < MazeArray.GetLength(1); x++)
            {
                MazeArray[0,x] = new MazeCell(x, 0, '1'); 
            }

            for (int x = 0; x < MazeArray.GetLength(1); x++)
            {
                MazeArray[MazeArray.GetLength(0) - 1,x] = new MazeCell(MazeArray.GetLength(0) - 1, x, '1');
            }

            for (int y = 1; y < MazeArray.GetLength(0) - 1; y++)
            {
                MazeArray[y, 0] = new MazeCell(0, y, '1');
                MazeArray[y, MazeArray.GetLength(1) - 1] = new MazeCell(MazeArray.GetLength(0) - 1, y, '1');
            }

            foreach (var mazeCell in MazeArray)
            {
                if (mazeCell.Character == 'm') MouseChecker = true;
                if (mazeCell.Character == 'e') ExitChecker = true;
            }

            if (MouseChecker && ExitChecker) InitializeMaze(MazeArray);
            else
            {
                if (!MouseChecker) MessageBox.Show("No entrance in maze.", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                if (!ExitChecker) MessageBox.Show("No exit in maze.", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        public static List<MazeCell> GetLongestPath(MazeCell[,] mazeArray)
        {
            StoredMazeArray = new MazeCell[mazeArray.GetLength(0), mazeArray.GetLength(1)];
            StoredMazeToBeShowed = new MazeCell[MazeToBeShowed.GetLength(0), MazeToBeShowed.GetLength(1)];
            CopyMaze(StoredMazeArray, mazeArray);
            ExitCell = ExitCells.Dequeue();
            mazeArray[ExitCell.Y, ExitCell.X].IsVisited = false;
            CurrentCell = EntryCell;
            MazePath = FindLongestPath(mazeArray);
            return MazePath;
        }
        private static List<MazeCell> FindLongestPath(MazeCell[,] mazeArray)
        {
            var mazePath = FindLongestPath(CurrentCell, ExitCell, mazeArray);
            return mazePath;
        }
        private static List<MazeCell> FindLongestPath(MazeCell currentCell, MazeCell exitCell, MazeCell[,] mazeArray)
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
        public static void GetPathForVisualization(List<MazeCell> mazePath)
        {
            if (mazePath == null)
            {
                MazeArray[ExitCell.Y, ExitCell.X].Character = 'x';
                MazeArray[ExitCell.Y, ExitCell.X].IsVisited = true;
                MazeToBeShowed[ExitCell.Y - 1, ExitCell.X - 1] = new MazeCell(ExitCell.X - 1, ExitCell.Y - 1, MazeArray[ExitCell.Y, ExitCell.X].Character);
                StoredMazeArray[ExitCell.Y, ExitCell.X] = MazeArray[ExitCell.Y, ExitCell.X];
                CopyMaze(MazeArray, StoredMazeArray);
                CopyMaze(StoredMazeToBeShowed, MazeToBeShowed);
                
            }
            else
            {
                mazePath[0].IsVisited = true;
                mazePath.Reverse();
                mazePath.RemoveAt(0);
                foreach (var mazeCell in mazePath)
                {
                    if (mazeCell.Character != ExitCell.Character) MazeArray[mazeCell.Y, mazeCell.X].Character = '.';
                    else
                    {
                        MazeArray[mazeCell.Y, mazeCell.X].Character = '@';
                        ExitCell.Character = MazeArray[mazeCell.Y, mazeCell.X].Character;
                    }
                }
                var canvasPath = new List<MazeCell>();
                foreach (MazeCell mazeCell in mazePath)
                {
                    canvasPath.Add(new MazeCell(mazeCell.X - 1, mazeCell.Y -1, mazeCell.Character));
                    canvasPath[0].IsVisited = mazeCell.IsVisited;
                }

                mazePath.Clear();
                mazePath.AddRange(canvasPath);
                StoredMazeArray[ExitCell.Y, ExitCell.X] = ExitCell;
                CopyMaze(MazeArray, StoredMazeArray);
                CopyMaze(StoredMazeToBeShowed, MazeToBeShowed);
            }
            if (mazePath != null) CellsInMazePath = mazePath.Count;
        }
        public static void StepMazeSolution(List<MazeCell> solvedPath)
        {
            var cellStep = solvedPath[0];
            solvedPath.RemoveAt(0);
            MazeToBeShowed[cellStep.Y, cellStep.X] = cellStep;

        }
        public static void RefreshMazeToBeShowed()
        {
            foreach (var mazeCell in MazeToBeShowed)
            {
                if (mazeCell.Character == '@') StoredMazeToBeShowed[mazeCell.Y, mazeCell.X] = mazeCell;
                if (mazeCell.Character == 'x') StoredMazeToBeShowed[mazeCell.Y, mazeCell.X] = mazeCell;
            }
            CopyMaze(MazeToBeShowed, StoredMazeToBeShowed);
        }
        private static void InitializeMaze(MazeCell[,] mazeArray)
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
                    if (ExitCells.Contains(mazeCell) == false) ExitCells.Enqueue(mazeCell);
                }
                if (mazeCell.Character == '0' || mazeCell.Character == 'm') mazeCell.IsVisited = false;
                else mazeCell.IsVisited = true;
            }
        }
        private static void CopyMaze(MazeCell[,] mazeArray, MazeCell[,] mazeToBeCopied)
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