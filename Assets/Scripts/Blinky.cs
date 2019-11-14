using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;

public class Blinky : MonoBehaviour
{
    int blinky_start_row = 24;
    int blinky_start_col = 0;
    float blinky_current_row = 0;
    float blinky_current_col = 0;
    float blinky_move_row;
    float blinky_move_col;
    float blinky_speed = 8.0f;

    bool movingUP = false;
    bool movingDOWN = false;
    bool movingLEFT = false;
    bool movingRIGHT = false;

    private AStarAlgorithm astar_gen;
    public Board board;
    public PacMan pacman;
    //private int[,] path2D;
    private const int ROWS = 29;
    private const int COLS = 26;
    public int[,] path2D;

    Stack blinky_path = new Stack();

    Pair<int, int> pair;
    Random rand;
   

    // Start is called before the first frame update
    void Start()
    {
        path2D = new int[ROWS, COLS] {
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,1 },
            { 1,1,1,1,1,1,0,0,1,1,1,1,0,0,1,1,1,1,0,0,1,1,1,1,1,1 },
            { 0,0,1,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,1,0,0 },
            { 0,0,1,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,1,0,0 },
            { 1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1 },
            { 1,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,1 },
            { 1,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,1 },
            { 1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1 },
            { 0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0 },
            { 0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0 },
            { 0,0,0,0,0,1,0,0,1,1,1,1,1,1,1,1,1,1,0,0,1,0,0,0,0,0 },
            { 0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0 },
            { 0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0 },
            { 0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0 },
            { 0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0 },
            { 0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0 },
            { 0,0,0,0,0,1,0,0,1,1,1,1,1,1,1,1,1,1,0,0,1,0,0,0,0,0 },
            { 0,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,0 },
            { 0,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,0 },
            { 1,1,1,1,1,1,0,0,1,1,1,1,0,0,1,1,1,1,0,0,1,1,1,1,1,1 },
            { 1,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,1 },
            { 1,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,1 },
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
            { 1,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,1 },
            { 1,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,1 },
            { 1,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,1 },
            { 1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1 }
        };

        transform.position = new Vector3(blinky_start_col, blinky_start_row, 0.0f);

        blinky_move_col = blinky_start_col;
        blinky_move_row = blinky_start_row;

        blinky_current_row = blinky_start_row;
        blinky_current_col = blinky_start_col;

        //temp start destination
        int dest_row = 0;
        int dest_col = 0;
        astar_gen = new AStarAlgorithm();
        astar_gen.aStarSearch(blinky_path, path2D, blinky_start_row, blinky_start_col, dest_row, dest_col);
    }

    // Update is called once per frame
    void Update()
    {
        //blinky_new_posX += blinky_speed * Time.deltaTime;
        //blinky.transform.position = new Vector3(blinky_new_posX, blinky_start_row, 0.0f);
        if (blinky_path.Count != 0 && !movingUP && !movingDOWN
                                   && !movingLEFT && !movingRIGHT)
        {
            pair = (Pair<int, int>)blinky_path.Peek();
            blinky_path.Pop();
            //Debug.Log("Popped");
            if (pair.first == blinky_current_row + 1)
            {
                //move blinky row + 1
                //Debug.Log("Moved UP");
                movingUP = true;
            }
            else if (pair.first == blinky_current_row - 1)
            {
                //move blinky row - 1
                //Debug.Log("Moved DOWN");
                movingDOWN = true;
            }
            else if (pair.second == blinky_current_col + 1)
            {
                //move blinky col + 1
                //Debug.Log("Moved RIGHT");
                movingRIGHT = true;
            }
            else if (pair.second == blinky_current_col - 1)
            {
                //move blinky col - 1
                //Debug.Log("Moved LEFT");
                movingLEFT = true;
            }
        }
        if (movingUP)
        {
            blinky_move_row += blinky_speed * Time.deltaTime;
            transform.position = new Vector3(blinky_current_col, blinky_move_row, 0.0f);
            //Debug.Log(blinky_current_row + " : " + p.first);
            if (blinky_move_row > pair.first)
            {
                transform.position = new Vector3(blinky_current_col, pair.first, 0.0f);
                blinky_current_row = pair.first;
                //Debug.Log("ASTAR: ROW: " + blinky_current_row + "\t First: " + pair.first);
                blinky_move_row = pair.first;
                movingUP = false;
            }
        } else if (movingDOWN)
        {
            blinky_move_row -= blinky_speed * Time.deltaTime;
            transform.position = new Vector3(blinky_current_col, blinky_move_row, 0.0f);
            //Debug.Log(blinky_current_row + " : " + p.first);
            if (blinky_move_row < pair.first)
            {
                transform.position = new Vector3(blinky_current_col, pair.first, 0.0f);
                blinky_current_row = pair.first;
                //Debug.Log("ASTAR: ROW: " + blinky_current_row + "\t First: " + pair.first);
                blinky_move_row = pair.first;
                movingDOWN = false;
            }
        } else if (movingLEFT)
        {
            blinky_move_col -= blinky_speed * Time.deltaTime;
            transform.position = new Vector3(blinky_move_col, blinky_current_row, 0.0f);
            //Debug.Log(blinky_current_row + " : " + p.first);
            if (blinky_move_col < pair.second)
            {
                //Debug.Log("MOVING FALSE");
                transform.position = new Vector3(pair.second, blinky_current_row, 0.0f);
                blinky_current_col = pair.second;
                blinky_move_col = pair.second;
                movingLEFT = false;
            }
        } else if (movingRIGHT)
        {
            blinky_move_col += blinky_speed * Time.deltaTime;
            transform.position = new Vector3(blinky_move_col, blinky_current_row, 0.0f);
            //Debug.Log(blinky_current_row + " : " + p.first);
            if (blinky_move_col > pair.second)
            {
                //Debug.Log("MOVING FALSE");
                transform.position = new Vector3(pair.second, blinky_current_row, 0.0f);
                blinky_current_col = pair.second;
                blinky_move_col = pair.second;
                movingRIGHT = false;
            }
        }

        
        if (!movingUP && !movingDOWN && !movingRIGHT && !movingLEFT && blinky_path.Count == 0)
        {
            //Debug.Log("ASTAR: ROW: " + blinky_current_row + " COL: " + blinky_current_col);
            astar_gen.aStarSearch(blinky_path, path2D, (int)blinky_current_row, (int)blinky_current_col, (int)pacman.pacman_current_row, (int)pacman.pacman_current_col);
        }
    }
}
