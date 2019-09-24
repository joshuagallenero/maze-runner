using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolverQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            string wall = "";
            string input = "";
            var MazeSolver = new MazeSolverUsingQueue(); bool done = false;
            Console.WriteLine("It is assumed that the user knows each row has a fixed number of characters and only 1 mouse can be in the maze.");
            Console.WriteLine("Create a maze by inputting a string of walls, paths, exits, and 1 mouse.");
            Console.WriteLine();
            Console.ReadLine();
            Console.Clear();
            while (done != true)
            {
                Console.WriteLine("Legend: e - exit");
                Console.WriteLine("Legend: n - mouse");
                Console.WriteLine("Legend: 0 - path");
                Console.WriteLine("Legend: 1 - wall");
                Console.WriteLine("Enter Row: ");
                input = Console.ReadLine();
                input = "1" + input + "1";
                if (MazeSolver.MazeRows.Count == 0)
                {
                    var mazerowstacklength = input.Length;
                    for (int x = 0; x < mazerowstacklength; x++)
                    {
                        wall += "1";
                    }
                    MazeSolver.MazeRows.Push(wall);
                }
                MazeSolver.MazeRows.Push(input);
                Console.WriteLine("Would you like to enter another row? (Y or N)");
                var answer = Console.ReadLine().ToLowerInvariant();
                if (answer == "n")
                {
                    done = true;
                }
                Console.WriteLine();

            }
            Console.Clear();
            MazeSolver.MazeRows.Push(wall);

            MazeCell[,] mazeArray = new MazeCell[MazeSolver.MazeRows.Count, input.Length];

            for (int y = MazeSolver.MazeRows.Count - 1; y >= 0; y--)
            {
                var currentRow = MazeSolver.MazeRows.Pop();
                for (int x = 0; x < input.Length; x++)
                {
                    mazeArray[y, x] = new MazeCell(x, y, currentRow[x]);
                }
            }

            MazeSolver.DisplayMaze(mazeArray);
            MazeSolver.SolveMazeMultipleExits(mazeArray);



            Console.ReadLine();
        }
    }
}
