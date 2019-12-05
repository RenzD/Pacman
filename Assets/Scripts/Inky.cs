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
    float inky_dest_row;
    float inky_dest_col;
    float inky_speed = 10.0f;
    float jailTime = 0.0f;
    float scatterTime = 0.0f;
    float chaseTime = 0.0f;

    int prediction = 0;
    int move_counter;
    int inky_moves = 1; //every x moves inky a*

    bool movingUP = false;
    bool movingDOWN = false;
    bool movingLEFT = false;
    bool movingRIGHT = false;
    bool turn = false;

    private AStarAlgorithm astar_gen;
    public Board board;
    public PacMan pacman;
    public GameObject inky_ghost;
    public GameObject scriptObj;
    public RuntimeAnimatorController ghostUp;
    public RuntimeAnimatorController ghostRight;
    public RuntimeAnimatorController ghostDown;
    public RuntimeAnimatorController ghostLeft;
    public RuntimeAnimatorController scared;
    public Sprite eyesUp;
    public Sprite eyesRight;
    public Sprite eyesDown;
    public Sprite eyesLeft;

    private const int ROWS = 29;
    private const int COLS = 26;

    public int[,] path2D;

    Stack inky_path = new Stack();
    Pair<int, int> pair;
    Pair<int, int> second_last_pos;
    Pair<int, int> second_last_pos_temp;


    State state;
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

        transform.position = new Vector3(inky_start_col, inky_start_row, 0.0f);

        inky_move_col = inky_start_col;
        inky_move_row = inky_start_row;

        inky_current_row = inky_start_row;
        inky_current_col = inky_start_col;

        astar_gen = scriptObj.GetComponent<AStarAlgorithm>();
        second_last_pos = new Pair<int, int>(0, 0);
        second_last_pos_temp = new Pair<int, int>(0, 0);
        
        state = State.Chase;

    }

    void UpdateAnimatorController()
    {
        if (state == State.Chase)
        {
            if (movingUP)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostUp;
            }
            else if (movingRIGHT)
            {

                transform.GetComponent<Animator>().runtimeAnimatorController = ghostRight;
            }
            else if (movingDOWN)
            {

                transform.GetComponent<Animator>().runtimeAnimatorController = ghostDown;
            }
            else if (movingLEFT)
            {

                transform.GetComponent<Animator>().runtimeAnimatorController = ghostLeft;
            }
            else
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostRight;
            }

        }
        else if (state == State.Frightened)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = scared;
        }
        else if (state == State.Eaten)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = null;
            if (movingUP)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesUp;
            }
            else if (movingRIGHT)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesRight;
            }
            else if (movingDOWN)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesDown;
            }
            else if (movingLEFT)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesLeft;
            }
            else
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesDown;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Pacman")
        {
            if (state == State.Frightened)
            {
                Debug.Log("Return to Start");
                state = State.Eaten;
            }
        }
    }

    private void Astar(int dest_row, int dest_col)
    {
        astar_gen.AStarSearch(inky_path, path2D, (int)inky_current_row, (int)inky_current_col, dest_row, dest_col, ref second_last_pos);
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
        MoveGhost();
    }

    private void Scatter()
    {
        // Time break time
        scatterTime += Time.deltaTime;
        if (scatterTime > 3)
        {
            scatterTime = 0.0f;
            //Switch state back to chase
            state = State.Chase;
        }
        inky_speed = 8.0f;

        second_last_pos_temp = second_last_pos;
        // Indicates/shows where inky's destination is in the game scene
        inky_ghost.transform.position = new Vector3(inky_corner_col, inky_corner_row, 0.0f);
        // Locks previous tile so inky cant turn back
        path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
        // Return to inky's corner
        Astar((int)inky_corner_row, (int)inky_corner_col);

        //Loops around when he's in his corner, just to make sure he doesn't stay in one spot
        if (inky_current_row == inky_corner_row && inky_current_col == inky_corner_col)
        {
            second_last_pos_temp = second_last_pos;
            path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
            Astar(6, 20);
        }
        if (inky_current_row == inky_corner_row && inky_current_col == inky_corner_col - 1 || 
            inky_current_row == inky_corner_row + 1 && inky_current_col == inky_corner_col)
        {
            second_last_pos_temp = second_last_pos;
            path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
            Astar(6, 20);
        }
    }

    private void ThirdDirectionCounter(int i, ref int dir_counter1, ref int dir_counter2, ref int dir_counter3, int dir1, int dir2, int dir3, int dir4)
    {
        if (pacman.directions[i] == dir1 && pacman.directions[i + 1] == dir1)
        {
            if (pacman.directions[i + 2] == dir2)
            {
                dir_counter1++;
            }
            else if (pacman.directions[i + 2] == dir3)
            {
                dir_counter2++;
            }
            else if (pacman.directions[i + 2] == dir4)
            {
                dir_counter3++;
            }
        }
    }

    private void NGramPrediction()
    {
        // When pacman has made 20+ moves
        if (pacman.directions.Count >= 20)
        {
            // Direction Counters for the third step
            int NNW = 0, NNE = 0, NNS = 0; // North North | WEST | EAST | SOUTH
            int EEN = 0, EES = 0, EEW = 0; // East East | NORTH | SOUTH | WEST
            int SSN = 0, SSE = 0, SSW = 0; // South South | NORTH | EAST | WEST
            int WWN = 0, WWE = 0, WWS = 0; // West West | NORTH | EAST  | SOUTH

            //Scan through the list of pacman's last X directions
            for (int i = 0; i < pacman.directions.Count - 2; i++) {
                // Count++ each side / backward move after a bigram
                ThirdDirectionCounter(i, ref NNE, ref NNS, ref NNW, 1, 2, 3, 4); //NN
                /**
                if (pacman.directions[i] == 1 && pacman.directions[i + 1] == 1) 
                {

                    if (pacman.directions[i+2] == 2) 
                    {
                        NNE++;
                    } 
                    else if (pacman.directions[i + 2] == 3)
                    {
                        NNS++;
                    }
                    else if (pacman.directions[i + 2] == 4)
                    {
                        NNW++;
                    }
                }
                */
                ThirdDirectionCounter(i, ref EEN, ref EES, ref EEW, 2, 1, 3, 4); //EE
                /**
                if (pacman.directions[i] == 2 && pacman.directions[i + 1] == 2)
                {
                    if (pacman.directions[i + 2] == 1)
                    {
                        EEN++;
                    }
                    else if (pacman.directions[i + 2] == 3)
                    {
                        EES++;
                    }
                    else if (pacman.directions[i + 2] == 4)
                    {
                        EEW++;
                    }
                }
                */
                ThirdDirectionCounter(i, ref SSN, ref SSE, ref SSW, 3, 1, 2, 4); //SS
                /**
                if (pacman.directions[i] == 3 && pacman.directions[i + 1] == 3)
                {
                    if (pacman.directions[i + 2] == 1)
                    {
                        SSN++;
                    }
                    else if (pacman.directions[i + 2] == 2)
                    {
                        SSE++;
                    }
                    else if (pacman.directions[i + 2] == 4)
                    {
                        SSW++;
                    }
                }
                */
                ThirdDirectionCounter(i, ref WWN, ref WWE, ref WWS, 4, 1, 2, 3); //WW
                /**
                if (pacman.directions[i] == 4 && pacman.directions[i + 1] == 4)
                {
                    if (pacman.directions[i + 2] == 1)
                    {
                        WWN++;
                    }
                    else if (pacman.directions[i + 2] == 2)
                    {
                        WWE++;
                    }
                    else if (pacman.directions[i + 2] == 3)
                    {
                        WWS++;
                    }
                }
                */
            }
            Debug.Log("North - E: " + NNE + "\tS: " + NNS + "\tW: " + NNW);
            Debug.Log("East - N: " + EEN + "\tS: " + EES + "\tW: " + EEW);
            Debug.Log("South - N: " + SSN + "\tE: " + SSE + "\tW: " + SSW);
            Debug.Log("West - N: " + WWN + "\tE: " + WWE + "\tS: " + WWS);
            // If Pacman's last movement matches any of the bigrams recorded
            // Predict the next move based on the probabilities
            if (pacman.directions[pacman.directions.Count - 1] == 1 &&
                pacman.directions[pacman.directions.Count - 2] == 1)
            {
                //Predict pacman's movement
                PredictDirection(NNW, NNE, NNS, 2, 3, 4);
            }
            else if (pacman.directions[pacman.directions.Count - 1] == 2 &&
                     pacman.directions[pacman.directions.Count - 2] == 2)
            {
                //Predict pacman's movement
                PredictDirection(EEW, EEN, EES, 1, 3, 4);
            }
            else if (pacman.directions[pacman.directions.Count - 1] == 3 &&
                     pacman.directions[pacman.directions.Count - 2] == 3)
            {
                //Predict pacman's movement
                PredictDirection(SSW, SSN, SSE, 1, 2, 4);
            }
            else if (pacman.directions[pacman.directions.Count - 1] == 4 &&
                     pacman.directions[pacman.directions.Count - 2] == 4)
            {
                //Predict pacman's movement
                PredictDirection(WWS, WWN, WWE, 1, 2, 3);
            }
        }
    }

    private void PredictDirection(int A, int B, int C, int dir1, int dir2, int dir3)
    {
        //Predict pacman's movement
        if (B > C && B > A)
        {
            prediction = dir1;
            Debug.Log("I predict you will go " + dir1);
        }
        else if (C > B && C > A)
        {
            prediction = dir2;
            Debug.Log("I predict you will go " + dir2);
        }
        else if (A > B && A > C)
        {
            prediction = dir3;
            Debug.Log("I predict you will go" + dir3);
        }
        else if (B == C && B > A)
        {
            if (prediction != dir1 && prediction != dir2)
            {
                int r = UnityEngine.Random.Range(0, 2);
                prediction = r == 0 ? dir1 : dir2;
            }
            Debug.Log("PREDICTION: " + prediction);
            Debug.Log("I predict you will go either " + dir1 + " OR " + dir2);

        }
        else if (B == A && B > C)
        {
            if (prediction != dir1 && prediction != dir3)
            {
                int r = UnityEngine.Random.Range(0, 2);
                prediction = r == 0 ? dir1 : dir3;
               
            }
            Debug.Log("PREDICTION: " + prediction);
            Debug.Log("I predict you will go either " + dir1 + " OR " + dir3);
        }
        else if (A == C && A > B)
        {
            if (prediction != dir2 && prediction != dir3)
            {
                int r = UnityEngine.Random.Range(0, 2);
                prediction = r == 0 ? dir2 : dir3;
                
            }
            Debug.Log("PREDICTION: " + prediction);
            Debug.Log("I predict you will go either " + dir2 + " OR " + dir3);
        }
        else if (A == C && C == B)
        {
            if (prediction != dir1 && prediction != dir2 && prediction != dir3)
            {
                int r = UnityEngine.Random.Range(0, 3);
                if (r == 0)
                {
                    prediction = dir1;
                }
                else if (r == 1)
                {
                    prediction = dir2;
                }
                else if (r == 2)
                {
                    prediction = dir3;
                }
            }
            Debug.Log("I predict you will go either " + dir1 + " OR " + dir2 + " OR " + dir3);
        }
        SetInkyAmbushPoint();
    }

    private void SetInkyAmbushPoint()
    {
        if (prediction != 0)
        {
            int max_tiles_ahead = 10;
            if (prediction == 1)
            {
                SetAmbushPointNorth(max_tiles_ahead);
            } 
            else if (prediction == 2)
            {
                SetAmbushPointEast(max_tiles_ahead);
            } 
            else if (prediction == 3)
            {
                SetAmbushPointSouth(max_tiles_ahead);
            }
            else if (prediction == 4)
            {
                SetAmbushPointWest(max_tiles_ahead);
            }
        }
    }

    private void SetAmbushPointWest(int max_tiles_ahead)
    {
        for (int tiles_ahead = 0; tiles_ahead <= max_tiles_ahead; tiles_ahead++)
        {
            if (pacman.pacman_eaten_col - tiles_ahead >= 0)
            {
                if (board.path2D[(int)pacman.pacman_current_row, (int)(pacman.pacman_eaten_col - tiles_ahead)] == 1 ||
                    board.path2D[(int)pacman.pacman_current_row, (int)(pacman.pacman_eaten_col - tiles_ahead)] == 2)
                {
                    inky_dest_col = pacman.pacman_eaten_col - tiles_ahead;
                    inky_dest_row = pacman.pacman_current_row;
                }
            }
        }
        Debug.Log("PACMAN WEST: " + inky_dest_col + "\t" + inky_dest_row);
    }

    private void SetAmbushPointSouth(int max_tiles_ahead)
    {
        for (int tiles_ahead = 0; tiles_ahead <= max_tiles_ahead; tiles_ahead++)
        {
            if (pacman.pacman_eaten_row - tiles_ahead >= 0)
            {
                if (board.path2D[(int)pacman.pacman_eaten_row - tiles_ahead, (int)(pacman.pacman_current_col)] == 1 ||
                    board.path2D[(int)pacman.pacman_eaten_row - tiles_ahead, (int)(pacman.pacman_current_col)] == 2)
                {
                    inky_dest_row = pacman.pacman_eaten_row - tiles_ahead;
                    inky_dest_col = pacman.pacman_current_col;
                }
            }
        }
        Debug.Log("PACMAN SOUTH: " + inky_dest_col + "\t" + inky_dest_row);
    }

    private void SetAmbushPointEast(int max_tiles_ahead)
    {
        for (int tiles_ahead = 0; tiles_ahead <= max_tiles_ahead; tiles_ahead++)
        {
            if (pacman.pacman_eaten_col + tiles_ahead < 26)
            {
                if (board.path2D[(int)pacman.pacman_current_row, (int)(pacman.pacman_eaten_col + tiles_ahead)] == 1 ||
                    board.path2D[(int)pacman.pacman_current_row, (int)(pacman.pacman_eaten_col + tiles_ahead)] == 2)
                {
                    inky_dest_col = pacman.pacman_eaten_col + tiles_ahead;
                    inky_dest_row = pacman.pacman_current_row;
                }
            }
        }
        Debug.Log("PACMAN EAST: " + inky_dest_col + "\t" + inky_dest_row);
    }

    private void SetAmbushPointNorth(int max_tiles_ahead)
    {
        for (int tiles_ahead = 0; tiles_ahead <= max_tiles_ahead; tiles_ahead++)
        {
            if (pacman.pacman_eaten_row + tiles_ahead < 29)
            {
                if (board.path2D[(int)pacman.pacman_eaten_row + tiles_ahead, (int)(pacman.pacman_current_col)] == 1 ||
                    board.path2D[(int)pacman.pacman_eaten_row + tiles_ahead, (int)(pacman.pacman_current_col)] == 2)
                {
                    inky_dest_row = pacman.pacman_eaten_row + tiles_ahead;
                    inky_dest_col = pacman.pacman_current_col;
                }
            }
        }
        Debug.Log("PACMAN NORTH: " + inky_dest_col + "\t" + inky_dest_row);
    }

    private void Frightened()
    {
        //Check if pacman chase mode is off
        if (pacman.pacman_chase == false)
        {
            // Switch back to chase
            state = State.Chase;
            chaseTime = 0.0f;
            scatterTime = 0.0f;
            turn = false;
        }
        // Frightened
        if (!movingUP && !movingDOWN && !movingRIGHT && !movingLEFT)
        {
            // Allow inky to turn back
            // Run away towards inky's corner
            if (!turn)
            {
                turn = true;
                inky_ghost.transform.position = new Vector3(inky_corner_col, inky_corner_row, 0.0f);
                Astar((int)inky_corner_row, (int)inky_corner_col);
            }
            else
            {
                second_last_pos_temp = second_last_pos;
                path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
                Astar((int)inky_corner_row, (int)inky_corner_col);
            }
            //Loops around when he's in his corner, just to make sure he doesn't stay in one spot
            if (inky_current_row == inky_corner_row && inky_current_col == inky_corner_col)
            {
                second_last_pos_temp = second_last_pos;
                path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
                Astar(6, 20);
            }
            if (inky_current_row == inky_corner_row && inky_current_col == inky_corner_col - 1 || 
                inky_current_row == inky_corner_row + 1 && inky_current_col == inky_corner_col)
            {
                second_last_pos_temp = second_last_pos;
                path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
                Astar(6, 20);
            }
        }
        // Slow down
        inky_speed = 4.0f;
    }

    private void Chase()
    {
        // Time the chase
        chaseTime += Time.deltaTime;
        // After 10s, take a break and scatter
        if (chaseTime > 10)
        {
            chaseTime = 0.0f;
            state = State.Scatter;
        }
        // If pacman ate a big pellet
        if (pacman.pacman_chase)
        {
            // Be scared
            state = State.Frightened;
        }
        else
        {
            // Chase
            if (!movingUP && !movingDOWN && !movingRIGHT && !movingLEFT && inky_path.Count == 0)
            {
                // Locks previous tile so inky can't turn back
                second_last_pos_temp = second_last_pos;
                path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
                
                // After pacman has made 20+ moves start prediction
                if (pacman.directions.Count >= 20) { 
                    NGramPrediction();
                    inky_ghost.transform.position = new Vector3(inky_dest_col, inky_dest_row, 0.0f);
                    Astar((int)inky_dest_row, (int)inky_dest_col);
                } else
                {
                    //If pacman hasn't moved 20+ times, move to the direction in front of pacman
                    Astar((int)pacman.pacman_ahead_row, (int)pacman.pacman_ahead_col);
                }
                
            }
            inky_speed = 8.0f;
        }
    }

    private void Eaten()
    {
        // When Inky is at start
        if (inky_current_col == inky_start_col && inky_current_row == inky_start_row)
        {
            // Start the jail time
            jailTime += Time.deltaTime;
            if (jailTime > 5)
            {
                // Switch back to chase
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
            inky_ghost.transform.position = new Vector3(inky_start_col, inky_start_row, 0.0f);
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
            UpdateAnimatorController();
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
