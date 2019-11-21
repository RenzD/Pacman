﻿using System;
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
    float blinky_move_row; // For blinky update
    float blinky_move_col; // For blinky update
    float blinky_speed = 8.0f;

    bool movingUP = false;
    bool movingDOWN = false;
    bool movingLEFT = false;
    bool movingRIGHT = false;

    private AStarAlgorithm astar_gen;
    public Board board;
    public PacMan pacman;

    private const int ROWS = 29;
    private const int COLS = 26;

    public int[,] path2D;

    Stack blinky_path = new Stack();
    Random rand;
    Pair<int, int> pair;
    Pair<int, int> second_last_pos;
    Pair<int, int> second_last_pos_temp;
    Pair<int, int> second_last_pos_temp2;

    int blinky_path_found_moves;
    int move_counter;
    int blinky_moves = 3; //every x moves blinky a*

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

        //Blinky start pos
        transform.position = new Vector3(blinky_start_col, blinky_start_row, 0.0f);

        blinky_move_col = blinky_start_col;
        blinky_move_row = blinky_start_row;

        blinky_current_row = blinky_start_row;
        blinky_current_col = blinky_start_col;

        astar_gen = new AStarAlgorithm();
        second_last_pos = new Pair<int, int>(0, 0);
        second_last_pos_temp2 = new Pair<int, int>(0, 0);
        second_last_pos_temp = new Pair<int, int>(0, 0);

    }

    // Update is called once per frame
    void Update()
    {
        if (!movingUP && !movingDOWN && !movingRIGHT && !movingLEFT && blinky_path.Count == 0)
        {
            if (blinky_path_found_moves < blinky_moves - 1)
            {
                second_last_pos_temp = second_last_pos;
            } else
            {
                second_last_pos_temp = second_last_pos_temp2;
            }
            path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
            // Find blinky's path using A*
            //Debug.Log((int)pacman.pacman_current_row + "\t" + (int)pacman.pacman_current_col);
            astar_gen.aStarSearch(blinky_path, path2D, (int)blinky_current_row, (int)blinky_current_col, (int)pacman.pacman_current_row, (int)pacman.pacman_current_col, ref second_last_pos);
            // Found moves
            blinky_path_found_moves = blinky_path.Count;
            // Set the blocked position back to movable in the 2D map array
            path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 1;
            // Set blinky second last position to 0

            move_counter = 0; // Blinky move counter
        }

        if (move_counter == blinky_moves)
        {
            while (blinky_path.Count != 0)
            {
                blinky_path.Pop();
            }
        }
        
        if (blinky_path.Count != 0 && move_counter != blinky_moves
                                   && !movingUP && !movingDOWN
                                   && !movingLEFT && !movingRIGHT)
        {
            move_counter++;
            pair = (Pair<int, int>)blinky_path.Peek();
            blinky_path.Pop();

            //If the found path is greater than every time blinky checks for a*,
            //set the his second last position to be blocked to his second move
            if (move_counter == 2 && blinky_path_found_moves > blinky_moves)
            {
                second_last_pos_temp2 = new Pair<int, int>(pair.first, pair.second);
            }

            //Based on the next cell, move pinky towards that direction
            if (pair.first == blinky_current_row + 1)
            {
                movingUP = true;
            }
            else if (pair.first == blinky_current_row - 1)
            {
                movingDOWN = true;
            }
            else if (pair.second == blinky_current_col + 1)
            {
                movingRIGHT = true;
            }
            else if (pair.second == blinky_current_col - 1)
            {
                movingLEFT = true;
            }
        }

        // Movement update
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
    }
}
