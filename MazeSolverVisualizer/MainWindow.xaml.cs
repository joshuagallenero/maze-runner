using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MazeSolverQueue;
using Path = System.IO.Path;
using Timer = System.Timers.Timer;

namespace MazeSolverVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        public double CurrentCanvasWidth = 1;
        public double CurrentCanvasHeight = 1;
        private bool _drag;
        private bool _firstPass = true;
        public void InitializeMaze(int mazeRows, int mazeColumns)
        {
            CurrentCanvasHeight = MazeCanvas.ActualHeight;
            CurrentCanvasWidth = MazeCanvas.ActualWidth;

            MazeCanvas.Children.Clear();

            double canvasWidth = MazeCanvas.ActualWidth;
            double canvasHeight = MazeCanvas.ActualHeight;

            double cellWidth = canvasWidth / mazeColumns;
            double cellHeight = canvasHeight / mazeRows;

            for (int x = 0; x < mazeRows; x++)
            {
                for (int y = 0; y < mazeColumns; y++)
                {
                    var mazeCell = CreateMazeCell(cellWidth, cellHeight);
                    AddCellToMaze(mazeCell, cellHeight * x ,cellWidth * y);
                }
            }
        }

        public Rectangle CreateMazeCell(double width, double height)
        {
            Rectangle baseRectangle = new Rectangle();
            baseRectangle.Stroke = new SolidColorBrush(Colors.Black);
            baseRectangle.StrokeThickness = 2;
            baseRectangle.Fill = new SolidColorBrush(Colors.DarkOrange);
            baseRectangle.Width = width;
            baseRectangle.Height = height;
            return baseRectangle;
        }

        public void MazeCellDimensionAdjust(UIElement cell, double width, double height)
        {
            ((Rectangle) cell).Width = width;
            ((Rectangle) cell).Height = height;
        }

        public void MazeCellPositionAdjust(UIElement cell, double top, double left)
        {
            Canvas.SetTop(cell, top);
            Canvas.SetLeft(cell, left);
        }

        public void AddCellToMaze(UIElement cell, double top, double left)
        {
            Canvas.SetTop(cell, top);
            Canvas.SetLeft(cell, left);
            MazeCanvas.Children.Add(cell);
        }

        public void AdjustCellsInMazeToRelativeSize()
        {
            double canvasWidth = MazeCanvas.ActualWidth;
            double canvasHeight = MazeCanvas.ActualHeight;

            double cellWidth = canvasWidth / ColumnSlider.Value;
            double cellHeight = canvasHeight / RowSlider.Value;

            int counter = 0;
            for (int y = 0; y < RowSlider.Value; y++)
            {
                for (int x = 0; x < ColumnSlider.Value; x++)
                {
                    MazeCellDimensionAdjust(MazeCanvas.Children[counter], cellWidth, cellHeight);
                    MazeCellPositionAdjust(MazeCanvas.Children[counter], cellHeight * y, cellWidth * x);
                    counter++;
                }
            }
        }

        public char[,] CreateMazeArrayFromMazeCanvas(UIElementCollection canvasMaze)
        {
            var mazeArray = new char[(int) RowSlider.Value, (int) ColumnSlider.Value];
            bool mouseChecker = false;
            int rectangleIndex = 0;
            for (int y = 0; y < RowSlider.Value; y++)
            {
                for (int x = 0; x < ColumnSlider.Value; x++)
                {
                    if ((MazeCanvas.Children[rectangleIndex] as Rectangle)?.Fill.ToString() == Brushes.DarkOrange.ToString()) mazeArray[y,x] = '1';
                    else if ((MazeCanvas.Children[rectangleIndex] as Rectangle)?.Fill.ToString() == Brushes.BlanchedAlmond.ToString()) mazeArray[y,x] = '0';
                    else if ((MazeCanvas.Children[rectangleIndex] as Rectangle)?.Fill.ToString() == Brushes.DimGray.ToString()) mazeArray[y,x] = 'm';
                    else if ((MazeCanvas.Children[rectangleIndex] as Rectangle)?.Fill.ToString() == Brushes.Black.ToString()) mazeArray[y,x] = 'e';
                    else if ((MazeCanvas.Children[rectangleIndex] as Rectangle)?.Fill.ToString() == Brushes.DarkViolet.ToString()) mazeArray[y,x] = '@';

                    rectangleIndex++;
                }
            }

            return mazeArray;
        }

        public void RevertSolvedMazeToCanvas(MazeCell[,] solvedArray)
        {
            int canvasIndex = 0;
            for (int y = 0; y < solvedArray.GetLength(0); y++)
            {
                for (int x = 0; x < solvedArray.GetLength(1); x++)
                {
                    if (solvedArray[y, x].Character == '1') ((Rectangle) MazeCanvas.Children[canvasIndex]).Fill = new SolidColorBrush(Colors.DarkOrange);

                    else if (solvedArray[y, x].Character == '0') ((Rectangle) (MazeCanvas.Children[canvasIndex])).Fill = new SolidColorBrush(Colors.BlanchedAlmond);

                    else if (solvedArray[y, x].Character == 'm') ((Rectangle) (MazeCanvas.Children[canvasIndex])).Fill = new SolidColorBrush(Colors.DimGray);

                    else if (solvedArray[y, x].Character == 'e') ((Rectangle) (MazeCanvas.Children[canvasIndex])).Fill = new SolidColorBrush(Colors.Black);

                    else if (solvedArray[y, x].Character == '.') ((Rectangle) (MazeCanvas.Children[canvasIndex])).Fill = new SolidColorBrush(Colors.LightGreen);

                    canvasIndex++;
                }
            }

        }

        private void BlinkExit(MazeCell exitCell, MazeCell[,] mazeToBeShowed, int exitCellCount)
        {
            foreach (var mazeCell in mazeToBeShowed)
            {
                if (mazeCell.Character == '@' || mazeCell.Character == 'x') exitCell = mazeCell;
            }

            var exitIndex = exitCell.X + mazeToBeShowed.GetLength(1) * exitCell.Y;
            var timer = new DispatcherTimer();
            var counter = 5;
            bool flag = false;
            timer.Tick += delegate
            {
                if (flag == false)
                {
                    if (exitCell.Y == 0 || exitCell.X == 0)
                        ((Rectangle) MazeCanvas.Children[exitIndex]).Fill = new SolidColorBrush(Colors.LightGreen);
                    else
                    {
                        ((Rectangle)MazeCanvas.Children[exitIndex]).Fill = new SolidColorBrush(Colors.LightGreen);
                        ((Rectangle)MazeCanvas.Children[exitIndex]).StrokeThickness = 7.5;
                    }
                }
                else
                {
                    if (exitCell.Y == 0 || exitCell.X == 0) ((Rectangle)MazeCanvas.Children[exitIndex]).Fill = new SolidColorBrush(Colors.Black);
                    else ((Rectangle)MazeCanvas.Children[exitIndex]).Fill = new SolidColorBrush(Colors.Black);
                }
                counter--;
                flag = !flag;
                if (counter == 0)
                {
                    timer.Stop();
                    ((Rectangle) MazeCanvas.Children[exitIndex]).Fill = new SolidColorBrush(Colors.DarkViolet);
                    ((Rectangle) MazeCanvas.Children[exitIndex]).StrokeThickness = 7.5;
                    if (exitCellCount > 0) NextExit.Visibility = Visibility.Visible;
                }
            };
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Start();
        }

        private void ReinitializeMaze()
        {
            foreach (Rectangle mazeCanvasChild in MazeCanvas.Children)
            {
                if (mazeCanvasChild.Fill.ToString() == Brushes.LightGreen.ToString())
                {
                    mazeCanvasChild.Fill = new SolidColorBrush(Colors.BlanchedAlmond);
                    mazeCanvasChild.StrokeThickness = 2;
                }
            }
        }

        private void ResetMaze()
        {
            foreach (Rectangle mazeCanvasChild in MazeCanvas.Children)
            {
                if (mazeCanvasChild.Fill.ToString() == Brushes.LightGreen.ToString())
                {
                    mazeCanvasChild.Fill = new SolidColorBrush(Colors.BlanchedAlmond);
                    mazeCanvasChild.StrokeThickness = 2;
                }

                if (mazeCanvasChild.Fill.ToString() == Brushes.DarkViolet.ToString())
                {
                    mazeCanvasChild.Fill = new SolidColorBrush(Colors.Black);
                    mazeCanvasChild.StrokeThickness = 2;
                }
            }
        }

        private void DisableMazeControl()
        {
            LongestPathToggle.IsEnabled = false;
            ShortestPathToggle.IsEnabled = false;
            RowSlider.IsEnabled = false;
            ColumnSlider.IsEnabled = false;
            MouseToggle.IsEnabled = false;
            WallToggle.IsEnabled = false;
            PathToggle.IsEnabled = false;
            ExitToggle.IsEnabled = false;
        }

        private void EnableMazeControl()
        {
            LongestPathToggle.IsEnabled = true;
            ShortestPathToggle.IsEnabled = true;
            RowSlider.IsEnabled = true;
            ColumnSlider.IsEnabled = true;
            MouseToggle.IsEnabled = true;
            WallToggle.IsEnabled = true;
            PathToggle.IsEnabled = true;
            ExitToggle.IsEnabled = true;
            SolveButton.IsEnabled = true;
        }

        private void RowSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                InitializeMaze((int)RowSlider.Value, (int)ColumnSlider.Value);
            }
            catch (Exception)
            {
                
            }
        }

        private void ColumnSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                InitializeMaze((int)RowSlider.Value,(int)ColumnSlider.Value);
            }
            catch (Exception)
            {
                
            }
        }

        private void MazeCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MazeCanvas.Children.Count != 0) AdjustCellsInMazeToRelativeSize();
        }

        private void WallToggle_OnChecked(object sender, RoutedEventArgs e)
        {
            ExitToggle.IsChecked = false;
            MouseToggle.IsChecked = false;
            PathToggle.IsChecked = false;
        }

        private void PathToggle_OnChecked(object sender, RoutedEventArgs e)
        {
            ExitToggle.IsChecked = false;
            MouseToggle.IsChecked = false;
            WallToggle.IsChecked = false;
        }

        private void MouseToggle_OnChecked(object sender, RoutedEventArgs e)
        {
            ExitToggle.IsChecked = false;
            WallToggle.IsChecked = false;
            PathToggle.IsChecked = false;
        }

        private void ExitToggle_OnChecked(object sender, RoutedEventArgs e)
        {
            WallToggle.IsChecked = false;
            MouseToggle.IsChecked = false;
            PathToggle.IsChecked = false;
        }

        private void MazeCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle currentCell = new Rectangle();
            var cellClicked = e.Source as FrameworkElement;
            if (cellClicked != null) currentCell = cellClicked as Rectangle;

            if (WallToggle.IsChecked == true)
            {
                if (currentCell != null) currentCell.Fill = new SolidColorBrush(Colors.DarkOrange);
            }
            else if (PathToggle.IsChecked == true)
            {
                if (currentCell != null) currentCell.Fill = new SolidColorBrush(Colors.BlanchedAlmond);
            }
            else if (MouseToggle.IsChecked == true)
            {
                Rectangle mouseFinder = new Rectangle();
                foreach (Rectangle mazeCell in MazeCanvas.Children)
                {
                    if (mazeCell.Fill.ToString() == Brushes.DimGray.ToString())
                    {
                        mazeCell.Fill = new SolidColorBrush(Colors.DarkOrange);
                        mouseFinder = mazeCell;
                    }
                }
                if (mouseFinder != currentCell)
                    if (currentCell != null) currentCell.Fill = new SolidColorBrush(Colors.DimGray);
            }
            else if (ExitToggle.IsChecked == true)
            {
                if (currentCell != null) currentCell.Fill = new SolidColorBrush(Colors.Black);
            }
            _drag = true;
        }

        private void MazeCanvas_OnMouseEnter(object sender, MouseEventArgs e)
        {
            
            if (_drag)
            {
                Rectangle currentCell = new Rectangle();
                var cellClicked = e.Source as FrameworkElement;
                if (cellClicked != null) currentCell = cellClicked as Rectangle;

                if (WallToggle.IsChecked == true)
                {
                    if (currentCell != null) currentCell.Fill = new SolidColorBrush(Colors.DarkOrange);
                }
                else if (PathToggle.IsChecked == true)
                {
                    if (currentCell != null) currentCell.Fill = new SolidColorBrush(Colors.BlanchedAlmond);
                }

                else if (ExitToggle.IsChecked == true)
                {
                    if (currentCell != null) currentCell.Fill = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void MazeCanvas_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _drag = false;
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer player = new MediaPlayer();

            #region LongestPath

            if (LongestPathToggle.IsChecked == true)
            {
                DisableMazeControl();

                if (_firstPass)
                {
                    MazeSolverLongestPath.CreateMazeArray(CreateMazeArrayFromMazeCanvas(MazeCanvas.Children));
                    if (MazeSolverLongestPath.MouseChecker == false || MazeSolverLongestPath.ExitChecker == false)
                    {
                        EnableMazeControl();
                        return;
                    }
                    var solvedPath = MazeSolverLongestPath.GetLongestPath(MazeSolverLongestPath.MazeArray);

                    if (solvedPath != null)
                    {
                        MazeSolverLongestPath.GetPathForVisualization(solvedPath);
                        var solveTimer = new DispatcherTimer();
                        var counter = 0;
                        solveTimer.Tick += delegate
                        {
                            counter++;
                            MazeSolverLongestPath.StepMazeSolution(solvedPath);
                            RevertSolvedMazeToCanvas(MazeSolverLongestPath.MazeToBeShowed);

                            if (counter == MazeSolverLongestPath.CellsInMazePath)
                            {
                                solveTimer.Stop();
                                if (MazeSolverLongestPath.ExitCells.Count > 0)
                                {
                                    BlinkExit(MazeSolverLongestPath.ExitCell, MazeSolverLongestPath.MazeToBeShowed, MazeSolverLongestPath.ExitCells.Count);
                                    MessageBox.Show("An exit has been found!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                                    SolveButton.IsEnabled = false;
                                    _firstPass = false;
                                }
                                else
                                {
                                    
                                    player.Stop();
                                    MediaPlayer soundBite = new MediaPlayer();
                                    soundBite.Open(new Uri(@"..\..\SoundBit\Sound.mp3", UriKind.Relative));
                                    soundBite.Play();
                                    BlinkExit(MazeSolverLongestPath.ExitCell, MazeSolverLongestPath.MazeToBeShowed, 0);
                                    MessageBox.Show("All exits found!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                                    SolveButton.Visibility = Visibility.Collapsed;
                                    ResetButton.Visibility = Visibility.Visible;
                                    SolveButton.IsEnabled = false;
                                    _firstPass = true;

                                }
                            }

                        };
                        
                        if (MazeSolverLongestPath.ExitCells.Count == 0)
                        {
                            player.Open(new Uri(@"..\..\SoundBit\Sound 3.wav", UriKind.Relative));
                            player.Play();
                        }
                        solveTimer.Interval = TimeSpan.FromMilliseconds(100);
                        solveTimer.Start();
                        SolveButton.IsEnabled = false;
                    }

                    else
                    {
                        MazeSolverLongestPath.GetPathForVisualization(solvedPath);
                        RevertSolvedMazeToCanvas(MazeSolverLongestPath.MazeToBeShowed);
                        if (MazeSolverLongestPath.ExitCells.Count > 0)
                        {
                            MediaPlayer soundBite = new MediaPlayer();
                            soundBite.Open(new Uri(@"..\..\SoundBit\Sound 4.wav", UriKind.Relative));
                            soundBite.Play();
                            BlinkExit(MazeSolverLongestPath.ExitCell, MazeSolverLongestPath.MazeToBeShowed, MazeSolverLongestPath.ExitCells.Count);
                            MessageBox.Show($"There is no path to {MazeSolverLongestPath.ExitCell.Y - 1},{MazeSolverLongestPath.ExitCell.X - 1}.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                            SolveButton.IsEnabled = false;
                            _firstPass = false;
                        }
                        else
                        {
                            MediaPlayer soundBite = new MediaPlayer();
                            soundBite.Open(new Uri(@"..\..\SoundBit\Sound 2.mp3", UriKind.Relative));
                            soundBite.Play();
                            BlinkExit(MazeSolverLongestPath.ExitCell, MazeSolverLongestPath.MazeToBeShowed, 0);
                            MessageBox.Show("Last exit unreachable.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                            SolveButton.Visibility = Visibility.Collapsed;
                            ResetButton.Visibility = Visibility.Visible;
                            SolveButton.IsEnabled = false;
                            _firstPass = true;
                        }
                    }
                }


                else
                {
                    var solvedPath = MazeSolverLongestPath.GetLongestPath(MazeSolverLongestPath.MazeArray);
                    
                    if (solvedPath != null)
                    {
                        MazeSolverLongestPath.GetPathForVisualization(solvedPath);
                        var solveTimer = new DispatcherTimer();
                        var counter = 0;

                        solveTimer.Tick += delegate
                        {
                            counter++;
                            MazeSolverLongestPath.StepMazeSolution(solvedPath);
                            RevertSolvedMazeToCanvas(MazeSolverLongestPath.MazeToBeShowed);

                            if (counter == MazeSolverLongestPath.CellsInMazePath)
                            {
                                solveTimer.Stop();
                                if (MazeSolverLongestPath.ExitCells.Count > 0)
                                {
                                    BlinkExit(MazeSolverLongestPath.ExitCell, MazeSolverLongestPath.MazeToBeShowed, MazeSolverLongestPath.ExitCells.Count);
                                    MessageBox.Show("An exit has been found!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                                    _firstPass = false;
                                }
                                else
                                {
                                    player.Stop();
                                    MediaPlayer soundBite = new MediaPlayer();
                                    soundBite.Open(new Uri(@"..\..\SoundBit\Sound.mp3", UriKind.Relative));
                                    soundBite.Play();
                                    BlinkExit(MazeSolverLongestPath.ExitCell, MazeSolverLongestPath.MazeToBeShowed, 0);
                                    MessageBox.Show("All exits found!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                                    SolveButton.Visibility = Visibility.Collapsed;
                                    ResetButton.Visibility = Visibility.Visible;
                                    SolveButton.IsEnabled = true;
                                    _firstPass = true;
                                }
                            }

                        };

                        if (MazeSolverLongestPath.ExitCells.Count == 0)
                        {
                            player.Open(new Uri(@"..\..\SoundBit\Sound 3.wav", UriKind.Relative));
                            player.Play();
                        }
                        solveTimer.Interval = TimeSpan.FromMilliseconds(100);
                        solveTimer.Start();
                        SolveButton.IsEnabled = false;
                    }

                    else
                    {
                        MazeSolverLongestPath.GetPathForVisualization(solvedPath);
                        RevertSolvedMazeToCanvas(MazeSolverLongestPath.MazeToBeShowed);
                        if (MazeSolverLongestPath.ExitCells.Count > 0)
                        {
                            MediaPlayer soundBite = new MediaPlayer();
                            soundBite.Open(new Uri(@"..\..\SoundBit\Sound 4.wav", UriKind.Relative));
                            soundBite.Play();
                            BlinkExit(MazeSolverLongestPath.ExitCell, MazeSolverLongestPath.MazeToBeShowed, MazeSolverLongestPath.ExitCells.Count);
                            MessageBox.Show($"There is no path to {MazeSolverLongestPath.ExitCell.Y - 1},{MazeSolverLongestPath.ExitCell.X - 1}.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                            SolveButton.IsEnabled = false;
                            _firstPass = false;
                        }
                        else
                        {
                            MediaPlayer soundBite = new MediaPlayer();
                            soundBite.Open(new Uri(@"..\..\SoundBit\Sound 2.mp3", UriKind.Relative));
                            soundBite.Play();
                            BlinkExit(MazeSolverLongestPath.ExitCell, MazeSolverLongestPath.MazeToBeShowed, 0);
                            MessageBox.Show("Last exit unreachable.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                            SolveButton.Visibility = Visibility.Collapsed;
                            ResetButton.Visibility = Visibility.Visible;
                            SolveButton.IsEnabled = false;
                            _firstPass = true;
                        }
                    }
                }
            }

            #endregion

            #region ShortestPath

            else if (ShortestPathToggle.IsChecked == true)
            {
                DisableMazeControl();

                if (_firstPass)
                {
                    MazeSolverShortestPath.CreateMazeArray(CreateMazeArrayFromMazeCanvas(MazeCanvas.Children));
                    if (MazeSolverShortestPath.MouseChecker == false || MazeSolverShortestPath.ExitChecker == false)
                    {
                        EnableMazeControl();
                        return;
                    }
                    var solvedPath = MazeSolverShortestPath.GetShortestPath(MazeSolverShortestPath.MazeArray);

                    if (solvedPath != null)
                    {
                        MazeSolverShortestPath.GetPathForVisualization(solvedPath);
                        var solveTimer = new DispatcherTimer();
                        var counter = 0;
                        solveTimer.Tick += delegate
                        {
                            counter++;
                            MazeSolverShortestPath.StepMazeSolution(solvedPath);
                            RevertSolvedMazeToCanvas(MazeSolverShortestPath.MazeToBeShowed);

                            if (counter == MazeSolverShortestPath.CellsInMazePath)
                            {
                                solveTimer.Stop();
                                if (MazeSolverShortestPath.ExitCells.Count > 0)
                                {
                                    BlinkExit(MazeSolverShortestPath.ExitCell, MazeSolverShortestPath.MazeToBeShowed, MazeSolverShortestPath.ExitCells.Count);
                                    MessageBox.Show("An exit has been found!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                                    SolveButton.IsEnabled = false;
                                    _firstPass = false;
                                }
                                else
                                {

                                    player.Stop();
                                    MediaPlayer soundBite = new MediaPlayer();
                                    soundBite.Open(new Uri(@"..\..\SoundBit\Sound.mp3", UriKind.Relative));
                                    soundBite.Play();
                                    BlinkExit(MazeSolverShortestPath.ExitCell, MazeSolverShortestPath.MazeToBeShowed, 0);
                                    MessageBox.Show("All exits found!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                                    SolveButton.Visibility = Visibility.Collapsed;
                                    ResetButton.Visibility = Visibility.Visible;
                                    SolveButton.IsEnabled = false;
                                    _firstPass = true;

                                }
                            }
                        };
                        if (MazeSolverShortestPath.ExitCells.Count == 0)
                        {
                            player.Open(new Uri(@"..\..\SoundBit\Sound 3.wav", UriKind.Relative));
                            player.Play();
                        }
                        solveTimer.Interval = TimeSpan.FromMilliseconds(100);
                        solveTimer.Start();
                        SolveButton.IsEnabled = false;
                    }

                    else
                    {
                        MazeSolverShortestPath.GetPathForVisualization(solvedPath);
                        RevertSolvedMazeToCanvas(MazeSolverShortestPath.MazeToBeShowed);
                        if (MazeSolverShortestPath.ExitCells.Count > 0)
                        {
                            MediaPlayer soundBite = new MediaPlayer();
                            soundBite.Open(new Uri(@"..\..\SoundBit\Sound 4.wav", UriKind.Relative));
                            soundBite.Play();
                            BlinkExit(MazeSolverShortestPath.ExitCell, MazeSolverShortestPath.MazeToBeShowed, MazeSolverShortestPath.ExitCells.Count);
                            MessageBox.Show($"There is no path to {MazeSolverShortestPath.ExitCell.Y - 1},{MazeSolverShortestPath.ExitCell.X - 1}.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                            SolveButton.IsEnabled = false;
                            _firstPass = false;
                        }
                        else
                        {
                            MediaPlayer soundBite = new MediaPlayer();
                            soundBite.Open(new Uri(@"..\..\SoundBit\Sound 2.mp3", UriKind.Relative));
                            soundBite.Play();
                            BlinkExit(MazeSolverShortestPath.ExitCell, MazeSolverShortestPath.MazeToBeShowed, 0);
                            MessageBox.Show("Last exit unreachable.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                            SolveButton.Visibility = Visibility.Collapsed;
                            ResetButton.Visibility = Visibility.Visible;
                            SolveButton.IsEnabled = false;
                            _firstPass = true;
                        }
                    }
                }
                else
                {
                    var solvedPath = MazeSolverShortestPath.GetShortestPath(MazeSolverShortestPath.MazeArray);

                    if (solvedPath != null)
                    {
                        MazeSolverShortestPath.GetPathForVisualization(solvedPath);
                        var solveTimer = new DispatcherTimer();
                        var counter = 0;
                        solveTimer.Tick += delegate
                        {
                            counter++;
                            MazeSolverShortestPath.StepMazeSolution(solvedPath);
                            RevertSolvedMazeToCanvas(MazeSolverShortestPath.MazeToBeShowed);

                            if (counter == MazeSolverShortestPath.CellsInMazePath)
                            {
                                solveTimer.Stop();
                                if (MazeSolverShortestPath.ExitCells.Count > 0)
                                {
                                    BlinkExit(MazeSolverShortestPath.ExitCell, MazeSolverShortestPath.MazeToBeShowed, MazeSolverShortestPath.ExitCells.Count);
                                    MessageBox.Show("An exit has been found!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                                    SolveButton.IsEnabled = false;
                                    _firstPass = false;
                                }
                                else
                                {

                                    player.Stop();
                                    MediaPlayer soundBite = new MediaPlayer();
                                    soundBite.Open(new Uri(@"..\..\SoundBit\Sound.mp3", UriKind.Relative));
                                    soundBite.Play();
                                    BlinkExit(MazeSolverShortestPath.ExitCell, MazeSolverShortestPath.MazeToBeShowed, 0);
                                    MessageBox.Show("All exits found!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                                    SolveButton.Visibility = Visibility.Collapsed;
                                    ResetButton.Visibility = Visibility.Visible;
                                    SolveButton.IsEnabled = false;
                                    _firstPass = true;

                                }
                            }
                        };
                        if (MazeSolverShortestPath.ExitCells.Count == 0)
                        {
                            player.Open(new Uri(@"..\..\SoundBit\Sound 3.wav", UriKind.Relative));
                            player.Play();
                        }
                        solveTimer.Interval = TimeSpan.FromMilliseconds(100);
                        solveTimer.Start();
                        SolveButton.IsEnabled = false;
                    }
                    else
                    {
                        MazeSolverShortestPath.GetPathForVisualization(solvedPath);
                        RevertSolvedMazeToCanvas(MazeSolverShortestPath.MazeToBeShowed);
                        if (MazeSolverShortestPath.ExitCells.Count > 0)
                        {
                            MediaPlayer soundBite = new MediaPlayer();
                            soundBite.Open(new Uri(@"..\..\SoundBit\Sound 4.wav", UriKind.Relative));
                            soundBite.Play();
                            BlinkExit(MazeSolverShortestPath.ExitCell, MazeSolverShortestPath.MazeToBeShowed, MazeSolverShortestPath.ExitCells.Count);
                            MessageBox.Show($"There is no path to {MazeSolverShortestPath.ExitCell.Y - 1},{MazeSolverShortestPath.ExitCell.X - 1}.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                            SolveButton.IsEnabled = false;
                            _firstPass = false;
                        }
                        else
                        {
                            MediaPlayer soundBite = new MediaPlayer();
                            soundBite.Open(new Uri(@"..\..\SoundBit\Sound 2.mp3", UriKind.Relative));
                            soundBite.Play();
                            BlinkExit(MazeSolverShortestPath.ExitCell, MazeSolverShortestPath.MazeToBeShowed, 0);
                            MessageBox.Show("Last exit unreachable.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                            SolveButton.Visibility = Visibility.Collapsed;
                            ResetButton.Visibility = Visibility.Visible;
                            SolveButton.IsEnabled = false;
                            _firstPass = true;
                        }
                    }
                }
                
            }

            #endregion

        }

        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            ResetMaze();
            EnableMazeControl();
            SolveButton.Visibility = Visibility.Visible;
            ResetButton.Visibility = Visibility.Collapsed;
        }

        private void NextExit_OnClick(object sender, RoutedEventArgs e)
        {
            if (LongestPathToggle.IsChecked == true) MazeSolverLongestPath.RefreshMazeToBeShowed();
            else if (ShortestPathToggle.IsChecked == true) MazeSolverShortestPath.RefreshMazeToBeShowed();
            ReinitializeMaze();
            NextExit.Visibility = Visibility.Collapsed;
            SolveButton.IsEnabled = true;
        }
    }
}
