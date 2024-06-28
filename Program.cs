using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Prototype
{
    public class Program
    {

        // TODO make a quick UI we use a while loop and then when the user has had enough we quit
        static void Main(string[] args)
        {
            bool loop = true;
            while(loop != false) { UserUI(ref loop); }
        }
        
        private static void UserUI(ref bool loop)
        {
            Console.WriteLine("Would you like to?\n[1] Create another maze\n[2] Quit the program");
            try
            {
                int Userinput = int.Parse(Console.ReadLine());
                if (Userinput == 2)
                {
                    loop = false;
                }
                else if (Userinput == 1) 
                {
                    Console.WriteLine();
                    Console.WriteLine("Please enter the width of the maze you would like to generate");
                    int x = int.Parse(Console.ReadLine());
                    Console.WriteLine("Please enter the height of the maze you would like to generate");
                    int y = int.Parse(Console.ReadLine());
                    CreateMazeAndVisualiseIt(x,y);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Please try again.");
                Console.WriteLine();
            }
        }

        private static void CreateMazeAndVisualiseIt(int x, int y )
        {
            // We make it an odd number to stop the mazes from breaking
            if (x % 2 == 0) x++;
            if (y % 2 == 0) y++;
            Console.Clear();    
            DFSMazeGeneration maze = new DFSMazeGeneration(x, y); 
            maze.GenerateMaze();
            VisualizeMaze(maze.GetGrid());
        }

        private static void VisualizeMaze(Cell[,] maze)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                for (int x = 0; x < maze.GetLength(0); x++)
                {
                    Console.ForegroundColor = ConsoleColor.White; // Fixes a weird bug where the top row would be gray for some reason.
                    if (maze[x, y].IsFirstCell == true) { Console.ForegroundColor = ConsoleColor.Green; Console.Write("██"); } // Prints the starting point
                    else if (maze[x, y].IsLastCell == true) { Console.ForegroundColor = ConsoleColor.Red; Console.Write("██"); } // Prints the ending point
                    else if (maze[x, y].IsWall == true) { Console.Write("██"); }
                    else { Console.Write("  "); }

                }
                Console.WriteLine();
            }
        }
    }

    public class Cell
    {
        public readonly int x, y;
        public bool IsWall, IsVisited, IsFirstCell, IsLastCell;
        public Cell(int inputX, int inputY)
        {
            x = inputX;
            y = inputY;
            IsWall = true;
            IsVisited = false;
            IsFirstCell = false;
            IsLastCell = false;
        }
    }

    public class DFSMazeGeneration
    {
        private int width;
        private int height;
        private Cell[,] grid;
        private Stack<Cell> cellStack;
        private Random random;

        public DFSMazeGeneration(int inWidth, int inHeight)
        {
            width = inWidth;
            height = inHeight;
            grid = new Cell[inWidth, inHeight];
            cellStack = new Stack<Cell>();
            random = new Random();
            InitializeGrid();
        }

        public Cell[,] GetGrid()
        {
            return grid;
        }

        private void InitializeGrid()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = new Cell(x, y);
                }
            }
        }

        public void GenerateMaze()
        {
            Cell startCell = grid[1, 1]; // Start at (1, 1) else it breaks
            startCell.IsWall = false;
            startCell.IsVisited = true;
            grid[0, 1].IsFirstCell = true; // Generates the start cell 
            grid[width - 1, height - 2].IsLastCell = true; // Generates the end cell
            cellStack.Push(startCell);

            while (cellStack.Count > 0) // Keeps looping until all the cells have no neighbours that have not been visited
            {
                Cell currentCell = cellStack.Peek();
                List<Cell> unvisitedNeighbours = GetUnvisitedNeighbours(currentCell);

                if (unvisitedNeighbours.Count > 0)
                {
                    Cell nextCell = unvisitedNeighbours[random.Next(unvisitedNeighbours.Count)];
                    RemoveWall(currentCell, nextCell);
                    nextCell.IsVisited = true;
                    nextCell.IsWall = false;
                    cellStack.Push(nextCell);
                }
                else
                {
                    cellStack.Pop(); // Backtracks to previous cell to see if there are any neighbours 
                }
            }
        }

        private List<Cell> GetUnvisitedNeighbours(Cell cell)
        {
            List<Cell> unvisitedNeighbours = new List<Cell>(); // Use a List to keep track of unvisited Neighbours
            int x = cell.x;
            int y = cell.y;

            // Check two cells away in each direction and makes sure that we are in the bounds of the array, and that we haven't already been to that cell.
            if (y > 2 && grid[x, y - 2].IsVisited == false)
            {
                unvisitedNeighbours.Add(grid[x, y - 2]);
            }
            if (y < height - 3 && grid[x, y + 2].IsVisited == false)
            {
                unvisitedNeighbours.Add(grid[x, y + 2]);
            }
            if (x > 2 && grid[x - 2, y].IsVisited == false)
            {
                unvisitedNeighbours.Add(grid[x - 2, y]);
            }
            if (x < width - 3 && grid[x + 2, y].IsVisited == false)
            {
                unvisitedNeighbours.Add(grid[x + 2, y]);
            }

            return unvisitedNeighbours;
        }

        private void RemoveWall(Cell currentCell, Cell nextCell) // We check 2 spaces away and break the wall inbetween the two
        {
            // Check the position of our neighBour
            int deltaX = currentCell.x - nextCell.x;
            int deltaY = currentCell.y - nextCell.y;

            if (deltaX == 2) // Neighbour is to the left (We know this becuase the x coordinate of the next cell is less than the coordinate of the current cell)
            {
                grid[currentCell.x - 1, currentCell.y].IsWall = false;
            }
            else if (deltaX == -2) // Neighbour is to the right
            {
                grid[currentCell.x + 1, currentCell.y].IsWall = false;
            }
            else if (deltaY == 2) // Neighbour is above 
            {
                grid[currentCell.x, currentCell.y - 1].IsWall = false;
            }
            else if (deltaY == -2) // Neighbour is bellow
            {
                grid[currentCell.x, currentCell.y + 1].IsWall = false;
            }
        }
    }
}
