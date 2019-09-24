using System.Collections.Generic;
using MazeSolverQueue;

namespace MazeSolverVisualizer
{
    public class MazeSolver
    {
        private MazeCell _currentCell;
        private Stack<MazeCell> _currentMazeStackForGui;
        private Stack<MazeCell> _mazeStack;
        private char[,] _mazeArray;
        private char[,] _currentArray;

        public static MazeCell CurrentCell
        {
            get { return _currentCell; }
            set { _currentCell = value; }
        }

        public static Stack<MazeCell> CurrentMazeStackForGUI
        {
            get { return _currentMazeStackForGui; }
            set { _currentMazeStackForGui = value; }
        }

        public static Stack<MazeCell> MazeStack
        {
            get { return _mazeStack; }
            set { _mazeStack = value; }
        }

        public static char[,] MazeArray
        {
            get { return _mazeArray; }
            set { _mazeArray = value; }
        }

        public static char[,] CurrentArray
        {
            get { return _currentArray; }
            set { _currentArray = value; }
        }
        public static void CreateMazeArray(char[,] mazeArray)
        {
            MazeArray = new char[mazeArray.GetLength(0) +2, mazeArray.GetLength(1) + 2];
            CurrentArray = new char[mazeArray.GetLength(0), mazeArray.GetLength(1)];

            for (int x = 0; x < mazeArray.GetLength(1) + 2; x++)
            {
                MazeArray[0, x] = '1';
            }

            for (int x = 0; x < mazeArray.GetLength(1) + 2; x++)
            {
                MazeArray[mazeArray.GetLength(0) + 1, x] = '1';
            }

            for (int x = 1; x < mazeArray.GetLength(0) + 1; x++)
            {
                MazeArray[x, 0] = '1';
                MazeArray[x, mazeArray.GetLength(1) + 1] = '1';
            }

            for (int y = 1; y < mazeArray.GetLength(0) + 1; y++)
            {
                for (int x = 1; x < mazeArray.GetLength(1) + 1; x++)
                {
                    MazeArray[y, x] = mazeArray[y - 1, x - 1];
                }
            }

            for (int y = 0; y < CurrentArray.GetLength(0); y++)
            {
                for (int x = 0; x < CurrentArray.GetLength(1); x++)
                {
                    CurrentArray[y, x] = mazeArray[y, x];
                }
            }

        }
    }
}