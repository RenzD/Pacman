﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;

public class Blinky : MonoBehaviour
{
    int blinky_start_row = 16;
    int blinky_start_col = 12;
    float blinky_current_row = 0;
    float blinky_current_col = 0;
    float blinky_move_row;
    float blinky_move_col;
    float blinky_corner_row = 28.0f;
    float blinky_corner_col = 0.0f;
    float blinky_dest_row;
    float blinky_dest_col;
    float blinky_speed = 8.0f;
    float jailTime = 0.0f;
    float chaseTime = 0.0f;
    float scatterTime = 0.0f;
    int prediction = 0;
    int move_counter;
    int blinky_moves = 1; //every x moves blinky a*

    bool movingUP = false;
    bool movingDOWN = false;
    bool movingLEFT = false;
    bool movingRIGHT = false;
    bool turn = false;

    private AStarAlgorithm astar_gen;
    public Board board;
    public PacMan pacman;
    public GameObject scriptObj;

    private const int ROWS = 29;
    private const int COLS = 26;

    public int[,] path2D;

    Stack blinky_path = new Stack();
    Pair<int, int> pair;
    Pair<int, int> second_last_pos;
    Pair<int, int> second_last_pos_temp;

    State state;

    //State Machine
    enum State
    {
        Chase,
        Frightened,
        Eaten,
        Scatter
    }

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
            { 0,0,0,0,0,1,1,1,1,0,0,1,1,1,1,0,0,1,1,1,1,0,0,0,0,0 },
            { 0,0,0,0,0,1,0,0,1,0,0,0,1,1,0,0,0,1,0,0,1,0,0,0,0,0 },
            { 0,0,0,0,0,1,0,0,1,0,0,0,1,1,0,0,0,1,0,0,1,0,0,0,0,0 },
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

        astar_gen = scriptObj.GetComponent<AStarAlgorithm>();
        second_last_pos = new Pair<int, int>(0, 0);
        second_last_pos_temp = new Pair<int, int>(0, 0);

        state = State.Chase;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pacman")
        {
            if (pacman.pacman_chase == true)
            {
                Debug.Log("Return to Start");
                state = State.Eaten;
            }
        }
    }
    private void Astar(int dest_row, int dest_col)
    {
        astar_gen.aStarSearch(blinky_path, path2D, (int)blinky_current_row, (int)blinky_current_col, dest_row, dest_col, ref second_last_pos);
        path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 1;
        move_counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //State machine
        switch (state)
        {
            case State.Chase:
                Chase();
                break;
            case State.Frightened:
                Frightened();
                break;
            case State.Eaten:
                Eaten();
                break;
            case State.Scatter:
                Scatter();
                break;
            default:
                Debug.Log("Unknown state");
                break;
        }
        chaseTime += Time.deltaTime;
        MoveGhost();
    }

    private void Scatter()
    {
        second_last_pos_temp = second_last_pos;
        path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
        Astar((int)blinky_corner_row, (int)blinky_corner_col);
        //After 3 seconds return to chase
        scatterTime += Time.deltaTime;
        if (scatterTime > 3.0f)
        {
            state = State.Chase;
            chaseTime = 0.0f;
        }
        if (pacman.pacman_chase)
        {
            state = State.Frightened;
        }
    }

    private void Frightened()
    {
        if (pacman.pacman_chase == false)
        {
            state = State.Chase;
            chaseTime = 0.0f;
            turn = false;
        }

        // Frightened
        if (!movingUP && !movingDOWN && !movingRIGHT && !movingLEFT)
        {
            //Blinky is allowed to go backwards when hes frightened
            if (!turn)
            {
                turn = true;
                Astar((int)blinky_corner_row, (int)blinky_corner_col);
            } else
            {
                second_last_pos_temp = second_last_pos;
                path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
                Astar((int)blinky_corner_row, (int)blinky_corner_col);
            }

            //Loops around when he's in his corner, just to make sure he doesn't stay in one spot
            if (blinky_current_row == blinky_corner_row && blinky_current_col == blinky_corner_col)
            {
                second_last_pos_temp = second_last_pos;
                path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
                Astar(24, 5);
            }
            if (blinky_current_row == blinky_corner_row && blinky_current_col == blinky_corner_col+1 || blinky_current_row == blinky_corner_row-1 && blinky_current_col == blinky_corner_col)
            {
                second_last_pos_temp = second_last_pos;
                path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
                Astar(24, 5);
            }
        }
        // Slow down
        blinky_speed = 4.0f;
    }
    private void Chase()
    {
        if (chaseTime > 10.0f)
        {
            state = State.Scatter;
            scatterTime = 0.0f;
        }
        if (pacman.pacman_chase)
        {
            state = State.Frightened;
        }
        else
        {
            if (!movingUP && !movingDOWN && !movingRIGHT && !movingLEFT && blinky_path.Count == 0)
            {
                second_last_pos_temp = second_last_pos;
                path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
                Astar((int)pacman.pacman_current_row, (int)pacman.pacman_current_col);
            }
            blinky_speed = 8.0f;
        }
    }

    private void Eaten()
    {
        if (blinky_current_col == blinky_start_col && blinky_current_row == blinky_start_row)
        {
            if (jailTime == 0) Debug.Log("Jail time started");
            jailTime += Time.deltaTime;
            if (jailTime > 5)
            {
                state = State.Chase;
                blinky_speed = 8f;
                jailTime = 0.0f;
                Debug.Log("Jail Time over");
            }
        }

        // Return to start
        if (jailTime == 0)
        {
            second_last_pos_temp = second_last_pos;
            path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 1;
            Astar(blinky_start_row, blinky_start_col);
            blinky_speed = 10f;
        }
    }

    private void MoveGhost()
    {
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

            if (pair.first == blinky_current_row + 1)
            {
                //Debug.Log("Moved UP");
                movingUP = true;
            }
            else if (pair.first == blinky_current_row - 1)
            {
                //Debug.Log("Moved DOWN");
                movingDOWN = true;
            }
            else if (pair.second == blinky_current_col + 1)
            {
                //Debug.Log("Moved RIGHT");
                movingRIGHT = true;
            }
            else if (pair.second == blinky_current_col - 1)
            {
                //Debug.Log("Moved LEFT");
                movingLEFT = true;
            }
        }

        if (movingUP)
        {
            blinky_move_row += blinky_speed * Time.deltaTime;
            transform.position = new Vector3(blinky_current_col, blinky_move_row, 0.0f);

            if (blinky_move_row > pair.first)
            {
                transform.position = new Vector3(blinky_current_col, pair.first, 0.0f);
                blinky_current_row = pair.first;

                blinky_move_row = pair.first;
                movingUP = false;
            }
        }
        else if (movingDOWN)
        {
            blinky_move_row -= blinky_speed * Time.deltaTime;
            transform.position = new Vector3(blinky_current_col, blinky_move_row, 0.0f);

            if (blinky_move_row < pair.first)
            {
                transform.position = new Vector3(blinky_current_col, pair.first, 0.0f);
                blinky_current_row = pair.first;

                blinky_move_row = pair.first;
                movingDOWN = false;
            }
        }
        else if (movingLEFT)
        {
            blinky_move_col -= blinky_speed * Time.deltaTime;
            transform.position = new Vector3(blinky_move_col, blinky_current_row, 0.0f);

            if (blinky_move_col < pair.second)
            {
                transform.position = new Vector3(pair.second, blinky_current_row, 0.0f);
                blinky_current_col = pair.second;
                blinky_move_col = pair.second;
                movingLEFT = false;
            }
        }
        else if (movingRIGHT)
        {
            blinky_move_col += blinky_speed * Time.deltaTime;
            transform.position = new Vector3(blinky_move_col, blinky_current_row, 0.0f);

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
