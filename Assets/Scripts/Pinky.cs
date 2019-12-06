using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : MonoBehaviour
{
    int pinky_start_row = 15;
    int pinky_start_col = 13;
    float pinky_current_row = 0;
    float pinky_current_col = 0;
    float pinky_move_row;
    float pinky_move_col;
    float pinky_corner_row = 28.0f;
    float pinky_corner_col = 25.0f;
    float pinky_speed = 8.0f;
    float scatterTime = 0.0f;
    float chaseTime = 0.0f;

    bool movingUP = false;
    bool movingDOWN = false;
    bool movingLEFT = false;
    bool movingRIGHT = false;
    bool moving = false;
    bool eaten = false;
    bool turn = false;
    bool breakBool = false;
    float jailTime = 0.0f;

    public GameObject scriptObj;
    private AStarAlgorithm astar_gen;
    public Board board;
    public PacMan pacman;
    public GameObject pinky_ghost;
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

    Stack pinky_path = new Stack();
    Pair<int, int> pair;
    Pair<int, int> second_last_pos;
    Pair<int, int> second_last_pos_temp;

    int move_counter;
    int pinky_moves = 1; //every x moves pinky a*

    Actions pinky_action = Actions.Chase;

    public enum Actions
    {
        Eaten,
        Frightened,
        Scatter,
        Chase
    };

    // Decision class
    public abstract class Decision
    {
        public abstract void Evaluate(PinkyCondition pinky, ref Actions dec);
    }

    // Pinky model
    public class PinkyCondition
    {
        public bool Eaten { get; set; }
        public bool PacmanChase { get; set; }
        public bool BreakTime { get; set; }
    }
    // The evaluation is based on a DecisionQuery (the question) and a DecisionResult (the response).
    public class DecisionQuery : Decision
    {
        public string Title { get; set; }
        public Decision Positive { get; set; }
        public Decision Negative { get; set; }
        public Func<PinkyCondition, bool> Test { get; set; }

        public override void Evaluate(PinkyCondition pinky, ref Actions dec)
        {
            bool result = this.Test(pinky);
            string resultAsString = result ? "yes" : "no";
            //Debug.Log($"\t- {this.Title}? {resultAsString}");

            if (result) this.Positive.Evaluate(pinky, ref dec);
            else this.Negative.Evaluate(pinky, ref dec);
        }
    }
    //Contains the final result of the decision tree
    public class DecisionResult : Decision
    {
        public Actions Result { get; set; }
        public override void Evaluate(PinkyCondition pinky, ref Actions decision)
        {
            decision = Result;
        }
    }

    // To create the tree, create the trunk and its branches
    // These branches will decide the actions
    private static DecisionQuery MainDecisionTree()
    {
        var breakBranch = new DecisionQuery
        {
            Title = "Does blinky need a break?",
            Test = (pinky) => pinky.BreakTime,
            // Scatter, Take a break / go to respective corner
            Positive = new DecisionResult { Result = Actions.Scatter },
            // Chase Pacman
            Negative = new DecisionResult { Result = Actions.Chase } 
        };

        var powerBranch = new DecisionQuery 
        { 
            Title = "Did pacman eat a power pellet?", 
            Test = (pinky) => pinky.PacmanChase,
            // Frightened, run away from pacman
            Positive = new DecisionResult { Result = Actions.Frightened },
            // Check breakBranch
            Negative = breakBranch
        };

        var trunk = new DecisionQuery
        {
            Title = "Is he eaten/jailed?",
            Test = (pinky) => pinky.Eaten,
            // Eaten, return to start
            Positive = new DecisionResult { Result = Actions.Eaten },
            // Check powerBranch
            Negative = powerBranch 
        };
        return trunk;
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

        // Set pinky start position
        transform.position = new Vector3(pinky_start_col, pinky_start_row, 0.0f);
        //Initialize move / current col
        pinky_move_col = pinky_start_col;
        pinky_move_row = pinky_start_row;
        pinky_current_row = pinky_start_row;
        pinky_current_col = pinky_start_col;
        //Gets the script for A Star tile pathfinding
        astar_gen = scriptObj.GetComponent<AStarAlgorithm>();
        second_last_pos = new Pair<int, int>(0,0);
        second_last_pos_temp = new Pair<int, int>(0, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When pinky collides with pacman after he eats a big pellet, pinky returns to start 
        if (other.gameObject.tag == "Pacman")
        {
            if (pacman.pacman_chase == true)
            {
                //Debug.Log("Return to Start");
                eaten = true;
            }
        }
    }

    //Finds a new path to take based on pinky's current tile and next destination
    private void Astar(int dest_row, int dest_col)
    {
        astar_gen.AStarSearch(pinky_path, path2D, (int)pinky_current_row, (int)pinky_current_col, dest_row, dest_col, ref second_last_pos);
        // Resets the last tile to be unblocked (it was blocked because we don't want the ghost to turn backwards)
        path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 1;
        move_counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        var trunk = MainDecisionTree();
        var pinky = new PinkyCondition
        {
            Eaten = eaten,
            PacmanChase = pacman.pacman_chase,
            BreakTime = breakBool
        };

        // Evaluates the pinky's current situation, then makes a decision
        trunk.Evaluate(pinky, ref pinky_action);
        // Choose the action based on the evaluated decision
        switch (pinky_action)
        {
            case Actions.Eaten:
                Jailed();
                break;
            case Actions.Frightened:
                Frightened();
                break;
            case Actions.Scatter:
                Scatter();
                break;
            case Actions.Chase:
                Chase();
                break;
            default:
                Debug.Log("Unknown state");
                break;
        }
        //Updates positions
        MoveGhost();
    }

    // Finds the destination to pacman
    private void Chase()
    {
        // Track chase time, to check if pinky needs a break
        chaseTime += Time.deltaTime;
        if (chaseTime > 10)
        {
            // If pinky needs a break
            breakBool = true;
            // Reset chase time
            chaseTime = 0.0f;
        }
        turn = false;

        // If pinky runs out of moves, 
        if (!moving)
        {
            second_last_pos_temp = second_last_pos;
            //Block the previous tile so pinky cant turn backwards
            path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
            // then find new path using A*, and reset pinky's speed
            pinky_ghost.transform.position = new Vector3(pacman.pacman_ahead_col, pacman.pacman_ahead_row, 0.0f);
            Astar((int)pacman.pacman_ahead_row, (int)pacman.pacman_ahead_col);
        }
        pinky_speed = 8.0f;
    }

    // Finds the destination to pinky's corner
    private void Scatter()
    {
        // Track scatter time, to check if pinky should return to chase pacman
        scatterTime += Time.deltaTime;
        if (scatterTime > 3)
        {
            // If pinky needs a break
            breakBool = false;
            // Reset scatter time
            scatterTime = 0.0f;
        }

        second_last_pos_temp = second_last_pos;
        //Indicator of where pinky is going in the game scene
        pinky_ghost.transform.position = new Vector3(pinky_corner_col, pinky_corner_row, 0.0f);
        //Block the previous tile so pinky cant turn backwards
        path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
        Astar((int)pinky_corner_row, (int)pinky_corner_col);


        //Loops around when he's in his corner, just to make sure he doesn't stay in one spot
        if (pinky_current_row == pinky_corner_row && pinky_current_col == pinky_corner_col)
        {
            second_last_pos_temp = second_last_pos;
            path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
            Astar(24, 20);
        }
        if (pinky_current_row == pinky_corner_row && pinky_current_col == pinky_corner_col - 1 || 
            pinky_current_row == pinky_corner_row - 1 && pinky_current_col == pinky_corner_col)
        {
            second_last_pos_temp = second_last_pos;
            path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
            Astar(24, 20);
        }
        pinky_speed = 8.0f;
    }

    // Finds the destination to start
    private void Jailed()
    {
        // When pinky is eaten and in the start position, start jail timer, then release after some time pass 
        if (pinky_current_col == pinky_start_col && pinky_current_row == pinky_start_row)
        {
            if (jailTime == 0) Debug.Log("Jail time started");
            jailTime += Time.deltaTime;
            if (jailTime > 5)
            {
                eaten = false;
                pinky_speed = 8f;
                jailTime = 0.0f;
                Debug.Log("Jail Time over");
            }
        }
        // When pinky is not in start position, find the path to pinky's start position
        if (jailTime == 0)
        {
            second_last_pos_temp = second_last_pos;
            pinky_ghost.transform.position = new Vector3(pinky_start_col, pinky_start_row, 0.0f);
            Astar(pinky_start_row, pinky_start_col);
            pinky_speed = 10f;
        }
    }

    // Finds the destination to pinky's corner
    private void Frightened()
    {
        // Allow pinky to turn backwards when he's frightened
        // Find path to pinky's corner
        if (!turn)
        {
            turn = true;
            chaseTime = 0.0f;
            scatterTime = 0.0f;
            pinky_ghost.transform.position = new Vector3(pinky_corner_col, pinky_corner_row, 0.0f);
            Astar((int)pinky_corner_row, (int)pinky_corner_col);
        }
        else
        {
            second_last_pos_temp = second_last_pos;
            path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
            Astar((int)pinky_corner_row, (int)pinky_corner_col);
        }

        //Loops around when he's in his corner, just to make sure he doesn't stay in one spot
        if (pinky_current_row == pinky_corner_row && pinky_current_col == pinky_corner_col)
        {
            second_last_pos_temp = second_last_pos;
            path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
            Astar(24, 20);
        }
        if (pinky_current_row == pinky_corner_row && pinky_current_col == pinky_corner_col - 1 ||
            pinky_current_row == pinky_corner_row - 1 && pinky_current_col == pinky_corner_col)
        {
            second_last_pos_temp = second_last_pos;
            path2D[second_last_pos_temp.first, second_last_pos_temp.second] = 0;
            Astar(24, 20);
        }
        // Slow down
        pinky_speed = 4.0f;
    }

    private void MoveGhost()
    {
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

            if (pair.first == pinky_current_row + 1)
            {
                //Debug.Log("Moved UP");
                movingUP = true;
                moving = true;
            }
            else if (pair.first == pinky_current_row - 1)
            {
                //Debug.Log("Moved DOWN");
                movingDOWN = true;
                moving = true;
            }
            else if (pair.second == pinky_current_col + 1)
            {
                //Debug.Log("Moved RIGHT");
                movingRIGHT = true;
                moving = true;
            }
            else if (pair.second == pinky_current_col - 1)
            {
                //Debug.Log("Moved LEFT");
                movingLEFT = true;
                moving = true;
            }
            UpdateAnimatorController();
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
                moving = false;
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
                moving = false;
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
                moving = false;
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
                moving = false;
            }
        }
    }
    void UpdateAnimatorController()
    {
        if (pinky_action == Actions.Chase)
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
        else if (pinky_action == Actions.Frightened)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = scared;
        }
        else if (pinky_action == Actions.Eaten)
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
}
