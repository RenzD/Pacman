using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Genetic : MonoBehaviour
{
    public GameObject pacmanPrefab;
    private const int ROWS = 29;
    private const int COLS = 26;

    static int pacnums = 8;
    static int parents_num = 2;
    static int movenums = 200;
    
    public int[,] path2D;
    public int[] movement;
    public int[] children_movement;
    public int[,] parent_movements;
    public int[,] children_movements;
    
    Movement[] moveScript;
    GameObject[] pacman;
    public GameObject pelletsPrefab;

    public Text gen_text;
    public Text fitness_text;
    public Text finText;
    double top_fitness = 0;
    int gen_counter = 1;
    bool once = false;
    bool generationDone = false;
    public double[] fitness;

    SortedSet<FitnessScore> individuals;

    class FitnessComparer : IComparer<FitnessScore>
    {
        public int Compare(FitnessScore x, FitnessScore y)
        {
            //first by fitness score
            int result = y.Fitness.CompareTo(x.Fitness);

            return result;
        }
    }
    class FitnessScore
    {
        public double Fitness { get; set; }

        public int[] MoveArray { get; set; }

    }
    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 3;
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

        InstantiatePelletPrefab();
        InitializePacmans();
    }
    // Initialization
    private void InitializePacmans()
    {
        int move;
        int counter;
        // Initialize 8 pacmans
        pacman = new GameObject[pacnums];
        moveScript = new Movement[pacnums];
        for (int i = 0; i < pacnums; i++)
        {
            pacman[i] = Instantiate(pacmanPrefab, transform.position, transform.rotation);
            pacman[i].tag = "Pacman" + i.ToString();
            // Get movement script for that pacman object
            moveScript[i] = pacman[i].GetComponent<Movement>();
            moveScript[i].SetPelletTag("Pellet" + i.ToString());
           
            // Randomize a series of 200 moves
            move = UnityEngine.Random.Range(1, 5);
            counter = 0;
            movement = new int[movenums];
            for (int j = 0; j < movenums; j++)
            {
                counter++;
                // After every 4, randomize direction
                if (counter == 4)
                {
                    //North:1 | East:2 | South:3 | West:4
                    move = UnityEngine.Random.Range(1, 5);
                    counter = 0;
                }
                movement[j] = move; //store it
            }
            // Pass it on to the pacman
            moveScript[i].SetMoves(movement);
        }
    }
    private void Update()
    {
        GetKeyInputReset();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // Check if the current generation of pacmans are done moving
        if (!generationDone)
        {
            int doneCounter = 0;
            // Check all pacman moves done
            for (int i = 0; i < pacnums; i++)
            {
                if (moveScript[i].movesDone)
                {
                    doneCounter++;
                }
            }
            // If all 8 pacmans are done, switch to GA
            if (doneCounter == pacnums)
            {
                generationDone = true;
            }
        }
        else
        {
            if (!once)
            {
                once = true; //DO ONLY ONCE

                Selection();
                Crossover();
                Mutation();

                ResetBoard();
                UpdateText();
            }
        }
    }

    private static void GetKeyInputReset()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(1);
            Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Time.timeScale < 5)
                Time.timeScale += 1;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Time.timeScale > 1)
                Time.timeScale = 1;
        }
    }
    private void UpdateText()
    {
        gen_counter++;
        gen_text.text = "Generation: " + gen_counter.ToString();
        fitness_text.text = "Fitness: " + top_fitness.ToString();
    }

    private void ResetBoard()
    {
        once = false;
        for (int i = 0; i < pacnums; i++)
        {
            moveScript[i].ResetPosition();
            moveScript[i].SetMovesFrom2D(children_movements, i);
            moveScript[i].ResetIndex();
            generationDone = false;
        }
        InstantiatePelletPrefab();
    }

    private void Selection()
    {
        // FitnessComparer sorts by highest Fitness first
        // FitnessScore contains -  (Fitness, Movement Array)
        individuals = new SortedSet<FitnessScore>(new FitnessComparer());
        FitnessScore fn;
        // Sort individuals by fitness score
        for (int i = 0; i < pacnums; i++)
        {
            // Initialize Fitness Score for each pacman to be sorted
            fn = new FitnessScore();
            fn.Fitness = moveScript[i].GetFitness();
            fn.MoveArray = moveScript[i].GetMoveArray();
            // Add pacman fitness-movement to the list
            individuals.Add(fn);
        }
        // The top 2 individuals from the list are stored to parent_movements[,]
        int parentCounter = 0;
        parent_movements = new int[parents_num, movenums];
        while (individuals.Count != 0)
        {
            FitnessScore pm = individuals.First();
            individuals.Remove(pm);
            // Only does first 2 iterations
            if (parentCounter < parents_num)
            {
                // The first iteration in the list has the best fitness 
                if (parentCounter == 0)
                {
                    // So store it
                    top_fitness = pm.Fitness;
                    // Convergence - If its optimal enough
                    // when fitness reaches over 120
                    if (top_fitness > 120)
                    {
                        // Stop the game
                        finText.gameObject.SetActive(true);
                        Time.timeScale = 0;
                    }
                }
                //Debug.Log("Fitness" + (parentCounter + 1) + ": " + p.Fitness);
                // Store the top 2 parents movements
                for (int x = 0; x < movenums; x++)
                {
                    parent_movements[parentCounter, x] = pm.MoveArray[x];
                }
                parentCounter++;
            }
        }
    }
    private void Crossover()
    {
        children_movements = new int[pacnums, movenums];
        bool parent_switch = false;
        // Splits to be used for crossovers
        int split1 = 100;
        int split2 = 50;
        int split3 = 25;
        int split4 = 175;

        //Mix parents movement[] to create 8 variety of movement[]
        // Keep best parent from the previous generation
        for (int i = 0; i < movenums; i++)
        {
            children_movements[0, i] = parent_movements[0, i];
        }
        //100
        InheritMovements(ref parent_switch, split1, 1);
        //100  reverse
        parent_switch = true;
        InheritMovements(ref parent_switch, split1, 2);
        //50
        InheritMovements(ref parent_switch, split2, 3);
        //50 reverse
        parent_switch = true;
        InheritMovements(ref parent_switch, split2, 4);
        //25
        InheritMovements(ref parent_switch, split3, 5);
        //25 reverse
        parent_switch = true;
        InheritMovements(ref parent_switch, split3, 6);
        //175
        InheritMovements(ref parent_switch, split4, 7);
    }

    private void InheritMovements(ref bool parent_switch, int split, int childNum)
    {
        int split_counter = 0;
        // Store parents' 1 and 2 movements to create a new array(offspring/child)
        for (int i = 0; i < movenums; i++)
        {
            // Whether parent 1 or parent 2 movements should be stored first
            if (!parent_switch) // Parent 1 is stored first
            {
                // Store the current parent's movements
                children_movements[childNum, i] = parent_movements[0, i];
                split_counter++;
                // Switch so the other parent's movements are stored instead
                if (split_counter == split)
                {
                    split_counter = 0;
                    parent_switch = !parent_switch;
                }
            }
            else // Parent 2 is stored first
            {
                children_movements[childNum, i] = parent_movements[1, i];
                split_counter++;
                if (split_counter == split)
                {
                    split_counter = 0;
                    parent_switch = !parent_switch;
                }
            }
        }
        parent_switch = false;
    }
    private void Mutation()
    {
        int rnd;
        int rnd_dir;
        // Starting at 1 so parent with index 0 do not get mutated
        // For every other new individual, apply mutation with a low probability
        for (int i = 1; i < pacnums; i++)
        {
            // Every 3 steps, check if 3 series of movement will be mutated
            for (int j = 0; j < movenums; j+=3)
            {
                // 10% chance that there will be a mutation
                rnd = UnityEngine.Random.Range(0, 100);
                if (rnd < 10)
                {
                    // direction randomizer - N:1 E:2 S:3 W:4
                    rnd_dir = UnityEngine.Random.Range(1, 5); 
                    children_movements[i, j] = rnd_dir; //1
                    children_movements[i, j + 1] = rnd_dir; //2
                    if (j+2 < 200)
                        children_movements[i, j + 2] = rnd_dir; //3
                }
            }
        }
    }

    // Pellets for each pacman
    public void InstantiatePelletPrefab()
    {
        Instantiate(pelletsPrefab, new Vector3(14.0f, 26.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(14.0f, 27.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(14.0f, 28.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(15.0f, 28.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(16.0f, 28.0f, 0.0f), transform.rotation);

        Instantiate(pelletsPrefab, new Vector3(22.0f, 21.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(23.0f, 21.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(24.0f, 21.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(25.0f, 21.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(25.0f, 22.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(25.0f, 23.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(25.0f, 24.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(25.0f, 25.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(25.0f, 26.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(25.0f, 27.0f, 0.0f), transform.rotation);

        Instantiate(pelletsPrefab, new Vector3(8.0f, 21.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(9.0f, 21.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(10.0f, 21.0f, 0.0f), transform.rotation);
        Instantiate(pelletsPrefab, new Vector3(11.0f, 21.0f, 0.0f), transform.rotation);
    }
}
