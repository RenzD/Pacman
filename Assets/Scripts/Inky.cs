using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : MonoBehaviour
{
    int inky_start_row = 15;
    int inky_start_col = 12;
    float inky_current_row = 0;
    float inky_current_col = 0;
    float inky_move_row;
    float inky_move_col;
    float inky_corner_row = 0.0f;
    float inky_corner_col = 25.0f;
    float inky_speed = 8.0f;

    float inky_dest_row;
    float inky_dest_col;

    bool movingUP = false;
    bool movingDOWN = false;
    bool movingLEFT = false;
    bool movingRIGHT = false;
    float jailTime = 0.0f;

    private AStarAlgorithm astar_gen;
    public Board board;
    public PacMan pacman;
    public Blinky blinky;

    private const int ROWS = 29;
    private const int COLS = 26;

    public int[,] path2D;

    Stack inky_path = new Stack();
    //Random rand;

    Pair<int, int> pair;
    Pair<int, int> second_last_pos;
    Pair<int, int> second_last_pos_temp;

    int inky_path_count_temp;
    int move_counter;
    int inky_moves = 1; //every x moves inky a*
    State state;

    enum State
    {
        Chase,
        Frightened,
        Eaten
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

        transform.position = new Vector3(inky_start_col, inky_start_row, 0.0f);

        inky_move_col = inky_start_col;
        inky_move_row = inky_start_row;

        inky_current_row = inky_start_row;
        inky_current_col = inky_start_col;

        astar_gen = new AStarAlgorithm();
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
        astar_gen.aStarSearch(inky_path, path2D, (int)inky_current_row, (int)inky_current_col, dest_row, dest_col, ref second_last_pos);
        inky_path_count_temp = inky_path.Count;
        path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 1;
        move_counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        GetInkyAmbushPoint();

        switch (state)
        {
            case State.Chase:
                Chase();
                //Debug.Log("Chase state");
                break;
            case State.Frightened:
                Frightened();
                //Debug.Log("Frightened state");
                break;
            case State.Eaten:
                ReturnToStart();
                //Debug.Log("Eaten state");
                break;
            default:
                Debug.Log("Unknown state");
                break;
        }

        MoveGhost();
    }

    private void GetInkyAmbushPoint()
    {
        if (pacman.pacman_current_row > blinky.blinky_current_row &&
            pacman.pacman_current_col > blinky.blinky_current_col)
        {
            // top right
        } else if (pacman.pacman_current_row < blinky.blinky_current_row &&
                   pacman.pacman_current_col < blinky.blinky_current_col)
        {
            // bottom left
        } else if (pacman.pacman_current_row > blinky.blinky_current_row &&
                   pacman.pacman_current_col < blinky.blinky_current_col)
        {
            // top left
        } else if (pacman.pacman_current_row < blinky.blinky_current_row &&
                   pacman.pacman_current_col > blinky.blinky_current_col){
            //bottom right
        }
    }

    private void Frightened()
    {
        if (pacman.pacman_chase == false)
        {
            state = State.Chase;
        }

        // Frightened
        if (!movingUP && !movingDOWN && !movingRIGHT && !movingLEFT)
        {
            //second_last_pos_temp = inky_path_count_temp < inky_moves ? second_last_pos : second_last_pos_temp2;
            Astar((int)inky_corner_row, (int)inky_corner_col);
        }
        // Slow down
        inky_speed = 4.0f;
    }

    private void Chase()
    {
        if (pacman.pacman_chase)
        {
            state = State.Frightened;
        }
        else
        {
            // Chase
            if (!movingUP && !movingDOWN && !movingRIGHT && !movingLEFT && inky_path.Count == 0)
            {
                second_last_pos_temp = second_last_pos;
                path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
                Astar((int)pacman.pacman_ahead_row, (int)pacman.pacman_ahead_col);
            }
            inky_speed = 8.0f;
        }
        
    }

    private void ReturnToStart()
    {
        if (inky_current_col == inky_start_col && inky_current_row == inky_start_row)
        {
            if (jailTime == 0) Debug.Log("Jail time started");
            jailTime += Time.deltaTime;
            if (jailTime > 5)
            {
                state = State.Chase;
                inky_speed = 8f;
                jailTime = 0.0f;
                Debug.Log("Jail Time over");
            }
        }

        // Return to start
        if (jailTime == 0)
        {
            second_last_pos_temp = second_last_pos;
            path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 1;
            Astar(inky_start_row, inky_start_col);
            inky_speed = 10f;
        }
    }

    private void MoveGhost()
    {
        if (move_counter == inky_moves)
        {
            while (inky_path.Count != 0)
            {
                inky_path.Pop();
            }
        }

        if (inky_path.Count != 0 && move_counter != inky_moves
                                  && !movingUP && !movingDOWN
                                  && !movingLEFT && !movingRIGHT)
        {
            move_counter++;
            pair = (Pair<int, int>)inky_path.Peek();
            inky_path.Pop();

            if (pair.first == inky_current_row + 1)
            {
                //Debug.Log("Moved UP");
                movingUP = true;
            }
            else if (pair.first == inky_current_row - 1)
            {
                //Debug.Log("Moved DOWN");
                movingDOWN = true;
            }
            else if (pair.second == inky_current_col + 1)
            {
                //Debug.Log("Moved RIGHT");
                movingRIGHT = true;
            }
            else if (pair.second == inky_current_col - 1)
            {
                //Debug.Log("Moved LEFT");
                movingLEFT = true;
            }
        }

        if (movingUP)
        {
            inky_move_row += inky_speed * Time.deltaTime;
            transform.position = new Vector3(inky_current_col, inky_move_row, 0.0f);

            if (inky_move_row > pair.first)
            {
                transform.position = new Vector3(inky_current_col, pair.first, 0.0f);
                inky_current_row = pair.first;

                inky_move_row = pair.first;
                movingUP = false;
            }
        }
        else if (movingDOWN)
        {
            inky_move_row -= inky_speed * Time.deltaTime;
            transform.position = new Vector3(inky_current_col, inky_move_row, 0.0f);

            if (inky_move_row < pair.first)
            {
                transform.position = new Vector3(inky_current_col, pair.first, 0.0f);
                inky_current_row = pair.first;

                inky_move_row = pair.first;
                movingDOWN = false;
            }
        }
        else if (movingLEFT)
        {
            inky_move_col -= inky_speed * Time.deltaTime;
            transform.position = new Vector3(inky_move_col, inky_current_row, 0.0f);

            if (inky_move_col < pair.second)
            {
                transform.position = new Vector3(pair.second, inky_current_row, 0.0f);
                inky_current_col = pair.second;
                inky_move_col = pair.second;
                movingLEFT = false;
            }
        }
        else if (movingRIGHT)
        {
            inky_move_col += inky_speed * Time.deltaTime;
            transform.position = new Vector3(inky_move_col, inky_current_row, 0.0f);

            if (inky_move_col > pair.second)
            {
                //Debug.Log("MOVING FALSE");
                transform.position = new Vector3(pair.second, inky_current_row, 0.0f);
                inky_current_col = pair.second;
                inky_move_col = pair.second;
                movingRIGHT = false;
            }
        }
    }
}
