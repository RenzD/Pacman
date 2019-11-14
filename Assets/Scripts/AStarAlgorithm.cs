﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarAlgorithm : MonoBehaviour
{
    const int ROWS = 29;
    const int COLS = 26;


    class AstarComparer : IComparer<Astar>
    {
        public int Compare(Astar x, Astar y)
        {
            //first by age
            int result = x.Fn.CompareTo(y.Fn);

            //then name
            if (result == 0)
                result = x.row.CompareTo(y.row);

            //a third sort
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
        // Row and Column index of its parent 
        // Note that 0 <= i <= ROW-1 & 0 <= j <= COL-1 
        public int parent_i;
        public int parent_j;
        // f = g + h 
        public double f;
        public double g;
        public double h;
    }

    // Start is called before the first frame update
    void Start()
    {

    }
    public void aStarSearch(Stack new_path, int[,] grid, int src_row, int src_col, int dest_row, int dest_col)
    {
        Pair<int, int> src = new Pair<int, int>(src_row, src_col);

        // Destination is the left-most top-most corner 
        Pair<int, int> dest = new Pair<int, int>(dest_row, dest_col);

        // If the source is out of range 
        if (isValid(src.first, src.second) == false)
        {
            Debug.Log("Source is invalid\n");
            return;
        }

        // If the destination is out of range 
        if (isValid(dest.first, dest.second) == false)
        {
            Debug.Log("Destination is invalid\n");
            return;
        }

        // Either the source or the destination is blocked 
        if (isUnBlocked(grid, src.first, src.second) == false ||
            isUnBlocked(grid, dest.first, dest.second) == false)
        {
            Debug.Log("Source or the destination is blocked\n");
            return;
        }

        // If the destination cell is the same as source cell 
        if (isDestination(src.first, src.second, dest) == true)
        {
            //Debug.Log("We are already at the destination\n");
            return;
        }

        // Create a closed list and initialise it to false which means 
        // that no cell has been included yet 
        // This closed list is implemented as a boolean 2D array 
        bool[,] closedList = new bool[ROWS, COLS];

        // Declare a 2D array of structure to hold the details 
        //of that cell 
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

        // Initialising the parameters of the starting node 
        i = src.first;
        j = src.second;
        cellDetails[i, j].f = 0.0;
        cellDetails[i, j].g = 0.0;
        cellDetails[i, j].h = 0.0;
        cellDetails[i, j].parent_i = i;
        cellDetails[i, j].parent_j = j;

        /*
	    Create an open list having information as-
	    <f, <i, j>>
	    where f = g + h,
	    and i, j are the row and column index of that cell
	    Note that 0 <= i <= ROW-1 & 0 <= j <= COL-1
	    This open list is implenented as a set of pair of pair.*/

        //HashSet<Pair<double, Pair<int, int>>> openList = new HashSet<Pair<double, Pair<int, int>>>();
        //openList.Add(new Pair<double, Pair<int, int>>(0.0, new Pair<int, int>(i, j)));

        SortedSet<Astar> openList = new SortedSet<Astar>(new AstarComparer());

        Astar astar1 = new Astar();
        astar1.Fn = 0;
        astar1.row = i;
        astar1.col = j;
        openList.Add(astar1);

        // We set this boolean value as false as initially 
        // the destination is not reached. 
        bool foundDest = false;

        while (openList.Count != 0)
        {
            Astar p = openList.First();
            openList.Remove(p);

            i = p.row;
            j = p.col;
            closedList[i, j] = true;

            double gNew, hNew, fNew;


            //----------- 1st Successor (North) ------------ 

            // Only process this cell if this is a valid one 
            if (isValid(i - 1, j) == true)
            {
                // If the destination cell is the same as the 
                // current successor 
                if (isDestination(i - 1, j, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i - 1, j].parent_i = i;
                    cellDetails[i - 1, j].parent_j = j;
                    Debug.Log("The destination cell is found\n");
                    tracePath(new_path, cellDetails, dest);
                    foundDest = true;
                    return;
                }
                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                // Else do the following 
                else if (closedList[i - 1, j] == false &&
                    isUnBlocked(grid, i - 1, j) == true)
                {
                    gNew = cellDetails[i, j].g + 1.0;
                    hNew = calculateHValue(i - 1, j, dest);
                    fNew = gNew + hNew;

                    // If it isn’t on the open list, add it to 
                    // the open list. Make the current square 
                    // the parent of this square. Record the 
                    // f, g, and h costs of the square cell 
                    //			 OR 
                    // If it is on the open list already, check 
                    // to see if this path to that square is better, 
                    // using 'f' cost as the measure. 
                    if (cellDetails[i - 1, j].f == double.MaxValue ||
                        cellDetails[i - 1, j].f > fNew)
                    {
                        //openList.Add(new Pair<double, Pair<int, int>>(fNew, new Pair<int, int>(i - 1, j)));

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
            //----------- 2nd Successor (South) ------------ 

            // Only process this cell if this is a valid one 
            if (isValid(i + 1, j) == true)
            {
                // If the destination cell is the same as the 
                // current successor 
                if (isDestination(i + 1, j, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i + 1, j].parent_i = i;
                    cellDetails[i + 1, j].parent_j = j;
                    Debug.Log("The destination cell is found\n");
                    tracePath(new_path, cellDetails, dest);
                    foundDest = true;
                    return;
                }
                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                // Else do the following 
                else if (closedList[i + 1, j] == false &&
                    isUnBlocked(grid, i + 1, j) == true)
                {
                    gNew = cellDetails[i, j].g + 1.0;
                    hNew = calculateHValue(i + 1, j, dest);
                    fNew = gNew + hNew;

                    // If it isn’t on the open list, add it to 
                    // the open list. Make the current square 
                    // the parent of this square. Record the 
                    // f, g, and h costs of the square cell 
                    //			 OR 
                    // If it is on the open list already, check 
                    // to see if this path to that square is better, 
                    // using 'f' cost as the measure. 
                    if (cellDetails[i + 1, j].f == double.MaxValue ||
                        cellDetails[i + 1, j].f > fNew)
                    {
                        //openList.Add(new Pair<double, Pair<int, int>>(fNew, new Pair<int, int>(i + 1, j)));

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

            //----------- 3rd Successor (East) ------------ 

            // Only process this cell if this is a valid one 
            if (isValid(i, j + 1) == true)
            {
                // If the destination cell is the same as the 
                // current successor 
                if (isDestination(i, j + 1, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i, j + 1].parent_i = i;
                    cellDetails[i, j + 1].parent_j = j;
                    Debug.Log("The destination cell is found\n");
                    tracePath(new_path, cellDetails, dest);
                    foundDest = true;
                    return;
                }

                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                // Else do the following 
                else if (closedList[i, j + 1] == false &&
                    isUnBlocked(grid, i, j + 1) == true)
                {
                    gNew = cellDetails[i, j].g + 1.0;
                    hNew = calculateHValue(i, j + 1, dest);
                    fNew = gNew + hNew;

                    // If it isn’t on the open list, add it to 
                    // the open list. Make the current square 
                    // the parent of this square. Record the 
                    // f, g, and h costs of the square cell 
                    //			 OR 
                    // If it is on the open list already, check 
                    // to see if this path to that square is better, 
                    // using 'f' cost as the measure. 
                    if (cellDetails[i, j + 1].f == double.MaxValue ||
                        cellDetails[i, j + 1].f > fNew)
                    {
                        //openList.Add(new Pair<double, Pair<int, int>>(fNew, new Pair<int, int>(i, j + 1)));

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

            //----------- 4th Successor (West) ------------ 

            // Only process this cell if this is a valid one 
            if (isValid(i, j - 1) == true)
            {
                // If the destination cell is the same as the 
                // current successor 
                if (isDestination(i, j - 1, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i, j - 1].parent_i = i;
                    cellDetails[i, j - 1].parent_j = j;
                    Debug.Log("The destination cell is found\n");
                    tracePath(new_path, cellDetails, dest);
                    foundDest = true;
                    return;
                }

                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                // Else do the following 
                else if (closedList[i, j - 1] == false &&
                    isUnBlocked(grid, i, j - 1) == true)
                {
                    gNew = cellDetails[i, j].g + 1.0;
                    hNew = calculateHValue(i, j - 1, dest);
                    fNew = gNew + hNew;

                    // If it isn’t on the open list, add it to 
                    // the open list. Make the current square 
                    // the parent of this square. Record the 
                    // f, g, and h costs of the square cell 
                    //			 OR 
                    // If it is on the open list already, check 
                    // to see if this path to that square is better, 
                    // using 'f' cost as the measure. 
                    if (cellDetails[i, j - 1].f == double.MaxValue ||
                        cellDetails[i, j - 1].f > fNew)
                    {
                        //openList.Add(new Pair<double, Pair<int, int>>(fNew, new Pair<int, int>(i, j - 1)));

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
            Debug.Log("Failed to find the Destination Cell\n");

        return;
    }

    /**
     * Returns true if the cell is not blocked else false 
     */
    bool isUnBlocked(int[,] grid, int row, int col)
    {
        if (grid[row, col] == 1)
            return (true);
        else
            return (false);
    }

    bool isValid(int row, int col)
    {
        // Returns true if row number and column number 
        // is in range 
        return (row >= 0) && (row < ROWS) &&
               (col >= 0) && (col < COLS);
    }

    bool isDestination(int row, int col, Pair<int, int> dest)
    {
        if (row == dest.first && col == dest.second)
            return (true);
        else
            return (false);
    }

    // A Utility Function to calculate the 'h' heuristics. 
    double calculateHValue(int row, int col, Pair<int, int> dest)
    {
        // Return using the distance formula 
        return ((double)Math.Sqrt((row - dest.first) * (row - dest.first)
                              + (col - dest.second) * (col - dest.second)));
    }


    void tracePath(Stack new_path, cell[,] cellDetails, Pair<int, int> dest)
    {
        //Debug.Log("\nThe Path is ");
        int row = dest.first;
        int col = dest.second;

        Stack Path = new Stack();

        while (!(cellDetails[row, col].parent_i == row
                 && cellDetails[row, col].parent_j == col))
        {
            //Debug.Log("Inside while loop");
            Path.Push(new Pair<int, int>(row, col));

            //New Path
            new_path.Push(new Pair<int, int>(row, col));

            int temp_row = cellDetails[row, col].parent_i;
            int temp_col = cellDetails[row, col].parent_j;
            row = temp_row;
            col = temp_col;
        }

        //int counter = 0;
        Path.Push(new Pair<int, int>(row, col));

        while (Path.Count != 0)
        {
            Pair<int, int> p = (Pair<int, int>)Path.Peek();
            Path.Pop();

            //Debug.Log("-> (" + p.first + "," + p.second + ")");
            //counter++;
            //Console.Write("-> (" + p.first + "," + p.second + ")");
        }

        //Debug.Log("Path.Count");
        return;
    }

}