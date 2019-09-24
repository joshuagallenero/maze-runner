namespace MazeSolverQueue
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
                case 'e':
                    IsVisited = true;
                    break;
                case '@': IsVisited = true;
                    break;
            }
        }
        public MazeCell(int xCoordinate, int yCoordinate, char blockStatus, MazeCell parent)
        {
            Character = blockStatus;
            X = xCoordinate;
            Y = yCoordinate;
            Parent = parent;
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