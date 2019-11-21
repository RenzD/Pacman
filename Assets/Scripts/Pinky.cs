﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : MonoBehaviour
{
    int pinky_start_row = 18;
    int pinky_start_col = 13;
    float pinky_current_row = 0;
    float pinky_current_col = 0;
    float pinky_move_row;
    float pinky_move_col;
    float pinky_corner_row = 28.0f;
    float pinky_corner_col = 25.0f;

    float pinky_speed = 8.0f;

    bool movingUP = false;
    bool movingDOWN = false;
    bool movingLEFT = false;
    bool movingRIGHT = false;

    bool eaten = false;

    private AStarAlgorithm astar_gen;
    public Board board;
    public PacMan pacman;

    private const int ROWS = 29;
    private const int COLS = 26;
    
    public int[,] path2D;

    Stack pinky_path = new Stack();
    Random rand;
    Pair<int, int> pair;
    Pair<int, int> second_last_pos;
    Pair<int, int> second_last_pos_temp2;
    Pair<int, int> second_last_pos_temp;

    int pinky_path_count_temp;
    int move_counter;
    int pinky_moves = 3; //every x moves pinky a*

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

        transform.position = new Vector3(pinky_start_col, pinky_start_row, 0.0f);

        pinky_move_col = pinky_start_col;
        pinky_move_row = pinky_start_row;

        pinky_current_row = pinky_start_row;
        pinky_current_col = pinky_start_col;

        //temp start destination
        //int dest_row = 0;
        //int dest_col = 0;
        astar_gen = new AStarAlgorithm();
        second_last_pos = new Pair<int, int>(0,0);
        second_last_pos_temp2 = new Pair<int, int>(0, 0);
        second_last_pos_temp = new Pair<int, int>(0, 0);
        //astar_gen.aStarSearch(pinky_path, path2D, pinky_start_row, pinky_start_col, dest_row, dest_col);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pacman")
        {
            if (pacman.pacman_chase == true)
            {
                Debug.Log("Return to Start");
                eaten = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pinky_current_col == pinky_start_col && pinky_current_row == pinky_start_row)
        {
            eaten = false;
            pinky_speed = 8f;
        }

        // DECISION TREE
        if (eaten)
        {
            //Return to start
            second_last_pos_temp = pinky_path_count_temp < pinky_moves - 1 ? second_last_pos : second_last_pos_temp2;
            Astar(pinky_start_row, pinky_start_col);
            pinky_speed = 10f;
        }
        else
        {
            if (pacman.pacman_chase == true)
            {
                //Frightened
                if (!movingUP && !movingDOWN && !movingRIGHT && !movingLEFT)
                {
                    second_last_pos_temp = pinky_path_count_temp < pinky_moves ? second_last_pos : second_last_pos_temp2;
                    Astar((int)pinky_corner_row, (int)pinky_corner_col);
                }
                // Slow down
                pinky_speed = 4.0f;
            } else
            {
                //if Pacman is near
                // Chase
                if (!movingUP && !movingDOWN && !movingRIGHT && !movingLEFT && pinky_path.Count == 0)
                {
                    second_last_pos_temp = pinky_path_count_temp < pinky_moves ? second_last_pos : second_last_pos_temp2;
                    path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
                    Astar((int)pacman.pacman_ahead_row, (int)pacman.pacman_ahead_col);
                } else
                {
                    // Wander
                }
                pinky_speed = 8.0f;
            }
        }


        if (move_counter == pinky_moves)
        {
            while (pinky_path.Count != 0)
            {
                pinky_path.Pop();
            }
        }

        if (pinky_path.Count != 0 && move_counter != pinky_moves 
                                  && !movingUP && !movingDOWN
                                  && !movingLEFT && !movingRIGHT)
        {
            move_counter++;
            pair = (Pair<int, int>)pinky_path.Peek();
            pinky_path.Pop();

            if (move_counter == 1 && pinky_path_count_temp > pinky_moves)
            {
                second_last_pos_temp2 = new Pair<int, int>(pair.first, pair.second);
            }

            if (pair.first == pinky_current_row + 1)
            {
                //move pinky row + 1
                //Debug.Log("Moved UP");
                movingUP = true;
            }
            else if (pair.first == pinky_current_row - 1)
            {
                //move pinky row - 1
                //Debug.Log("Moved DOWN");
                movingDOWN = true;
            }
            else if (pair.second == pinky_current_col + 1)
            {
                //move pinky col + 1
                //Debug.Log("Moved RIGHT");
                movingRIGHT = true;
            }
            else if (pair.second == pinky_current_col - 1)
            {
                //move pinky col - 1
                //Debug.Log("Moved LEFT");
                movingLEFT = true;
            }
        }
        if (movingUP)
        {
            pinky_move_row += pinky_speed * Time.deltaTime;
            transform.position = new Vector3(pinky_current_col, pinky_move_row, 0.0f);

            if (pinky_move_row > pair.first)
            {
                transform.position = new Vector3(pinky_current_col, pair.first, 0.0f);
                pinky_current_row = pair.first;

                pinky_move_row = pair.first;
                movingUP = false;
            }
        }
        else if (movingDOWN)
        {
            pinky_move_row -= pinky_speed * Time.deltaTime;
            transform.position = new Vector3(pinky_current_col, pinky_move_row, 0.0f);

            if (pinky_move_row < pair.first)
            {
                transform.position = new Vector3(pinky_current_col, pair.first, 0.0f);
                pinky_current_row = pair.first;

                pinky_move_row = pair.first;
                movingDOWN = false;
            }
        }
        else if (movingLEFT)
        {
            pinky_move_col -= pinky_speed * Time.deltaTime;
            transform.position = new Vector3(pinky_move_col, pinky_current_row, 0.0f);

            if (pinky_move_col < pair.second)
            {

                transform.position = new Vector3(pair.second, pinky_current_row, 0.0f);
                pinky_current_col = pair.second;
                pinky_move_col = pair.second;
                movingLEFT = false;
            }
        }
        else if (movingRIGHT)
        {
            pinky_move_col += pinky_speed * Time.deltaTime;
            transform.position = new Vector3(pinky_move_col, pinky_current_row, 0.0f);

            if (pinky_move_col > pair.second)
            {
                //Debug.Log("MOVING FALSE");
                transform.position = new Vector3(pair.second, pinky_current_row, 0.0f);
                pinky_current_col = pair.second;
                pinky_move_col = pair.second;
                movingRIGHT = false;
            }
        }
    }

    private void Astar(int dest_row, int dest_col)
    {
        astar_gen.aStarSearch(pinky_path, path2D, (int)pinky_current_row, (int)pinky_current_col, dest_row, dest_col, ref second_last_pos);
        pinky_path_count_temp = pinky_path.Count;
        path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 1;
        move_counter = 0;
    }
}
