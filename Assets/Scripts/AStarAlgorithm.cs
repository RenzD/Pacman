using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarAlgorithm : MonoBehaviour
{
    const int ROWS = 29;
    const int COLS = 26;

    //Compares and sorts by the F value first, then row and col
    class AstarComparer : IComparer<Astar>
    {
        public int Compare(Astar x, Astar y)
        {
            int result = x.Fn.CompareTo(y.Fn);

            if (result == 0)
                result = x.row.CompareTo(y.row);

            if (result == 0)
                result = x.col.CompareTo(y.col);

            return result;
        }
    }

    class Astar
    {
        public double Fn { get; set; }

        public int row { get; set; }

        public int col { get; set; }
    }

    struct cell
    {
        // Row and Column index of its parent: 0 <= i <= ROW-1 & 0 <= j <= COL-1 
        public int parent_i;
        public int parent_j;
        // f = g + h 
        public double f;
        public double g;
        public double h;
    }

    /***
     * new_path - the stack that will contain the new set of the new path towards the destination
     * grid - the 2D map array that defines the path for our ghosts (0 = blocked, 1 = unblocked)
     * src_row & src_col - source tile location in the 2D array
     * dest_row & dest_col - destination tile location in the 2D array
     * block_pos - position to be blocked so the ghosts can't u-turn backwards
     */
    public void AStarSearch(Stack new_path, int[,] grid, int src_row, int src_col, 
                            int dest_row, int dest_col, ref Pair<int,int> block_pos)
    {
        // Sets the row/col of destination to a Pair
        Pair<int, int> src = new Pair<int, int>(src_row, src_col);
        Pair<int, int> dest = new Pair<int, int>(dest_row, dest_col);

        // Check if the source is out of range 
        if (IsTileValid(src.first, src.second) == false)
        {
            return;
        }
        // Check if the destination is out of range 
        if (IsTileValid(dest.first, dest.second) == false)
        {
            return;
        }
        // Check if the source or the destination is blocked 
        if (IsTileUnblocked(grid, src.first, src.second) == false ||
            IsTileUnblocked(grid, dest.first, dest.second) == false)
        {
            return;
        }
        // If the destination cell is the same as source cell 
        if (IsTileDestination(src.first, src.second, dest) == true)
        {
            return;
        }

        // Create a closed list - no cell has been included yet 
        bool[,] closedList = new bool[ROWS, COLS];

        // Declare a 2D array of structure to hold the details of that cell 
        cell[,] cellDetails = new cell[ROWS, COLS];
        int i;
        int j;
        for (i = 0; i < ROWS; i++)
        {
            for (j = 0; j < COLS; j++)
            {
                cellDetails[i, j].f = double.MaxValue;
                cellDetails[i, j].g = double.MaxValue;
                cellDetails[i, j].h = double.MaxValue;
                cellDetails[i, j].parent_i = -1;
                cellDetails[i, j].parent_j = -1;
            }
        }

        // The last [i,j] is the starting node, so we initialize it
        i = src.first;
        j = src.second;
        cellDetails[i, j].f = 0.0;
        cellDetails[i, j].g = 0.0;
        cellDetails[i, j].h = 0.0;
        cellDetails[i, j].parent_i = i;
        cellDetails[i, j].parent_j = j;

        // Create an open list
        // <f, <i, j>> , where f = g + h
        SortedSet<Astar> openList = new SortedSet<Astar>(new AstarComparer());
        Astar astar = new Astar();
        astar.Fn = 0;
        astar.row = i;
        astar.col = j;
        openList.Add(astar);

        // We set this boolean value as false as initially
        bool foundDest = false;

        // Loop, if we find a path as we scan through the map, 
        // we will keep adding to the openList
        while (openList.Count != 0)
        {
            Astar p = openList.First();
            openList.Remove(p);

            i = p.row;
            j = p.col;
            closedList[i, j] = true;

            double gNew, hNew, fNew;

            // Checks North side 
            if (IsTileValid(i - 1, j) == true)
            {
                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                if (IsTileDestination(i - 1, j, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i - 1, j].parent_i = i;
                    cellDetails[i - 1, j].parent_j = j;
                    FindPath(new_path, grid, cellDetails, dest, ref block_pos);
                    foundDest = true;
                    return;
                }
                else if (closedList[i - 1, j] == false &&
                    IsTileUnblocked(grid, i - 1, j) == true)
                {
                    gNew = cellDetails[i, j].g + 1.0;
                    hNew = CalculateH(i - 1, j, dest);
                    fNew = gNew + hNew;
                    // If its not in the open list, then add it, then make it the parent, and save f,g,h
                    // If it is in the open list, check to see if this path to that square is better, 
                    // measured by the f value
                    if (cellDetails[i - 1, j].f == double.MaxValue ||
                        cellDetails[i - 1, j].f > fNew)
                    {
                        Astar newAstar = new Astar();
                        newAstar.Fn = fNew;
                        newAstar.row = i - 1;
                        newAstar.col = j;
                        openList.Add(newAstar);

                        // Update the details of this cell 
                        cellDetails[i - 1, j].f = fNew;
                        cellDetails[i - 1, j].g = gNew;
                        cellDetails[i - 1, j].h = hNew;
                        cellDetails[i - 1, j].parent_i = i;
                        cellDetails[i - 1, j].parent_j = j;
                    }
                }
            }
            // Check for South Side
            if (IsTileValid(i + 1, j) == true)
            {
                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                if (IsTileDestination(i + 1, j, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i + 1, j].parent_i = i;
                    cellDetails[i + 1, j].parent_j = j;
                    //Debug.Log("The destination cell is found\n");
                    FindPath(new_path, grid, cellDetails, dest, ref block_pos);
                    foundDest = true;
                    return;
                }
                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                // Else do the following 
                else if (closedList[i + 1, j] == false &&
                    IsTileUnblocked(grid, i + 1, j) == true)
                {
                    gNew = cellDetails[i, j].g + 1.0;
                    hNew = CalculateH(i + 1, j, dest);
                    fNew = gNew + hNew;

                    // If its not in the open list, then add it, then make it the parent, and save f,g,h
                    // If it is in the open list, check to see if this path to that square is better, 
                    // measured by the f value in the Comparer class
                    if (cellDetails[i + 1, j].f == double.MaxValue ||
                        cellDetails[i + 1, j].f > fNew)
                    {
                        Astar newAstar = new Astar();
                        newAstar.Fn = fNew;
                        newAstar.row = i + 1;
                        newAstar.col = j;
                        openList.Add(newAstar);

                        // Update the details of this cell 
                        cellDetails[i + 1, j].f = fNew;
                        cellDetails[i + 1, j].g = gNew;
                        cellDetails[i + 1, j].h = hNew;
                        cellDetails[i + 1, j].parent_i = i;
                        cellDetails[i + 1, j].parent_j = j;
                    }
                }
            }
            // Checks for East side
            if (IsTileValid(i, j + 1) == true)
            {
                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                if (IsTileDestination(i, j + 1, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i, j + 1].parent_i = i;
                    cellDetails[i, j + 1].parent_j = j;
                    //Debug.Log("The destination cell is found\n");
                    FindPath(new_path, grid, cellDetails, dest, ref block_pos);
                    foundDest = true;
                    return;
                }

                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                // Else do the following 
                else if (closedList[i, j + 1] == false &&
                    IsTileUnblocked(grid, i, j + 1) == true)
                {
                    gNew = cellDetails[i, j].g + 1.0;
                    hNew = CalculateH(i, j + 1, dest);
                    fNew = gNew + hNew;

                    // If its not in the open list, then add it, then make it the parent, and save f,g,h
                    // If it is in the open list, check to see if this path to that square is better, 
                    // measured by the f value in the Comparer class
                    if (cellDetails[i, j + 1].f == double.MaxValue ||
                        cellDetails[i, j + 1].f > fNew)
                    {
                        Astar newAstar = new Astar();
                        newAstar.Fn = fNew;
                        newAstar.row = i;
                        newAstar.col = j + 1;
                        openList.Add(newAstar);

                        // Update the details of this cell 
                        cellDetails[i, j + 1].f = fNew;
                        cellDetails[i, j + 1].g = gNew;
                        cellDetails[i, j + 1].h = hNew;
                        cellDetails[i, j + 1].parent_i = i;
                        cellDetails[i, j + 1].parent_j = j;
                    }
                }
            }
            // Checks for West Side
            if (IsTileValid(i, j - 1) == true)
            {
                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                if (IsTileDestination(i, j - 1, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i, j - 1].parent_i = i;
                    cellDetails[i, j - 1].parent_j = j;
                    //Debug.Log("The destination cell is found\n");
                    FindPath(new_path, grid, cellDetails, dest, ref block_pos);
                    foundDest = true;
                    return;
                }

                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                // Else do the following 
                else if (closedList[i, j - 1] == false &&
                    IsTileUnblocked(grid, i, j - 1) == true)
                {
                    gNew = cellDetails[i, j].g + 1.0;
                    hNew = CalculateH(i, j - 1, dest);
                    fNew = gNew + hNew;

                    // If its not in the open list, then add it, then make it the parent, and save f,g,h
                    // If it is in the open list, check to see if this path to that square is better, 
                    // measured by the f value in the Comparer class
                    if (cellDetails[i, j - 1].f == double.MaxValue ||
                        cellDetails[i, j - 1].f > fNew)
                    {
                        Astar newAstar = new Astar();
                        newAstar.Fn = fNew;
                        newAstar.row = i;
                        newAstar.col = j - 1;
                        openList.Add(newAstar);

                        // Update the details of this cell 
                        cellDetails[i, j - 1].f = fNew;
                        cellDetails[i, j - 1].g = gNew;
                        cellDetails[i, j - 1].h = hNew;
                        cellDetails[i, j - 1].parent_i = i;
                        cellDetails[i, j - 1].parent_j = j;
                    }
                }
            }
        }

        if (foundDest == false)
        {
            Debug.Log("Failed to find the Destination Cell\n");
        }
        return;
    }

    // Checks if the Tile is blocked or unblock (0 or 1)
    bool IsTileUnblocked(int[,] grid, int row, int col)
    {
        if (grid[row, col] == 1)
        {
            return (true);
        }
        else
        {
            return (false);
        }
    }

    // Check if row,col is within the boundary
    bool IsTileValid(int row, int col)
    {
        // Returns true if row number and column number is in range 
        return (row >= 0) && (row < ROWS) &&
               (col >= 0) && (col < COLS);
    }

    // Checks if the row,col is the destination
    bool IsTileDestination(int row, int col, Pair<int, int> dest)
    {
        if (row == dest.first && col == dest.second)
        {
            return (true);
        }
        else
        {
            return (false);
        }
    }


    // Gets the h value for the heuristic
    double CalculateH(int row, int col, Pair<int, int> dest)
    {
        // Return using the manhattan distance formula 
        return ((double)Math.Sqrt((row - dest.first) * (row - dest.first)
                              + (col - dest.second) * (col - dest.second)));
    }


    // Records the path from start to destination to a Stack, 
    //push it to the new_path passed in from the ghost scripts
    void FindPath(Stack new_path, int[,] map, cell[,] cellDetails, Pair<int, int> dest, ref Pair<int, int> block_pos)
    {
        //Debug.Log("\nThe Path is ");
        int row = dest.first;
        int col = dest.second;

        Stack Path = new Stack();
        while (!(cellDetails[row, col].parent_i == row
                 && cellDetails[row, col].parent_j == col))
        {
            Path.Push(new Pair<int, int>(row, col));

            //New Path
            new_path.Push(new Pair<int, int>(row, col));

            int temp_row = cellDetails[row, col].parent_i;
            int temp_col = cellDetails[row, col].parent_j;
            row = temp_row;
            col = temp_col;
        }

        Path.Push(new Pair<int, int>(row, col));
        // checking for first node
        block_pos = (Pair<int, int>)Path.Peek();
        return;
    }

}
