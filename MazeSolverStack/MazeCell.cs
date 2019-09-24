namespace MazeSolverStack
{
    public class MazeCell
    {
        public MazeCell()
        {
            
        }

        public MazeCell(int xCoordinate, int yCoordinate, char blockStatus)
        {
            Character = blockStatus;
            X = xCoordinate;
            Y = yCoordinate;
            switch (Character)
            {
                case '1':
                    IsVisited = true;
                    break;
                case '0':
                    IsVisited = false;
                    break;
                case 'e':
                    IsVisited = true;
                    break;
                case '@':
                    IsVisited = true;
                    break;
            }
        }

        public MazeCell(char character, int x, int y, MazeCell parent)
        {
            Character = character;
            X = x;
            Y = y;
            Parent = parent;
        }

        public MazeCell(int y, int x)
        {
            Y = y;
            X = x;
        }
        public char Character { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsVisited { get; set; }
        public MazeCell Parent { get; set; }

        public override string ToString()
        {
            return $"{Y},{X}";
        }
    }
}