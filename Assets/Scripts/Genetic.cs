using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    public Text gen_text;
    public Text fitness_text;
    double top_fitness;
    int gen_counter = 1;
    bool once = false;
    bool generationDone = false;
    double[] fitness;

    SortedSet<FitnessScore> openList;

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
        Time.timeScale = 5;
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

        
        int move = UnityEngine.Random.Range(1, 5);
        int counter = 0;
        pacman = new GameObject[pacnums];
        moveScript = new Movement[pacnums];
        for (int i = 0; i < pacnums; i++)
        {
            pacman[i] = Instantiate(pacmanPrefab, transform.position, transform.rotation);
            moveScript[i] = pacman[i].GetComponent<Movement>();
            movement = new int[movenums];

            for (int j = 0; j < movenums; j++)
            {
                counter++;
                if (counter == 3)
                {
                    move = UnityEngine.Random.Range(1, 5);
                    counter = 0;
                }
                movement[j] = move;
            }
            moveScript[i].SetMoves(movement);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the current generation of pacmans 
        if (!generationDone)
        {
            int doneCounter = 0;
            for (int i = 0; i < pacnums; i++)
            {
                if (moveScript[i].movesDone)
                {
                    doneCounter++;
                }
            }
            if (doneCounter == pacnums)
            {
                generationDone = true; 
                //Debug.Log("generationDone");
                fitness = new double[pacnums];
                for (int i = 0; i < pacnums; i++)
                {
                    fitness[i] = moveScript[i].fitness;
                    //Debug.Log("Fitness " + i + ": " + fitness[i]);
                }
            }
        } else
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

    private void UpdateText()
    {
        gen_counter++;
        gen_text.text = "Generation: " + gen_counter.ToString();
        fitness_text.text = "Fitness: " + top_fitness.ToString();
    }

    private void ResetBoard()
    {
        for (int i = 0; i < pacnums; i++)
        {
            moveScript[i].ResetPosition();
            moveScript[i].SetMovesFrom2D(children_movements, i);
            moveScript[i].ResetIndex();
            generationDone = false;
            once = false;
        }

    }

    private void Selection()
    {
        // Sorts the movements by fitness score
        openList = new SortedSet<FitnessScore>(new FitnessComparer());
        FitnessScore fn;
        for (int i = 0; i < pacnums; i++)
        {
            fn = new FitnessScore();
            fn.Fitness = fitness[i];
            fn.MoveArray = moveScript[i].GetMoveArray();
            string str = "";
            foreach (int x in fn.MoveArray) {
                str += x.ToString();
            }
            //Debug.Log("Fitness " + i + ": " + fitness[i]);
            //Debug.Log(str);
            // Add pacman fitness-movement to the list
            openList.Add(fn);
        }
        //Debug.Log("\n");

        int parentCounter = 0;
        parent_movements = new int[parents_num, movenums];
        while (openList.Count != 0)
        {
            FitnessScore p = openList.First();
            openList.Remove(p);

            if (parentCounter < parents_num)
            {
                if (parentCounter == 0)
                {
                    top_fitness = p.Fitness;
                }
                string str2 = "";
                Debug.Log("Fitness" + (parentCounter + 1) + ": " + p.Fitness);
                for (int x = 0; x < movenums; x++)
                {
                    parent_movements[parentCounter, x] = p.MoveArray[x];
                    if (x == 100)
                    {
                        str2 += "\t\t";
                    }
                    str2 += parent_movements[parentCounter, x].ToString();
                }
                //Debug.Log("Fitness2: " + p.Fitness);
                //Debug.Log("Array2: " + str2);
                parentCounter++;
            }
        }
    }

    private void Crossover()
    {
        children_movements = new int[pacnums, movenums];
        bool parent_switch = false;
        //hardcoded splits for a 200 
        int split1 = 100;
        int split2 = 50;
        int split3 = 25;
        int split4 = 13;

        //Mix parents movement[] to create 8 variety of movement[]
        //100
        InheritMovements(ref parent_switch, split1, 0);
        //100  reverse
        parent_switch = true;
        InheritMovements(ref parent_switch, split1, 1);
        //50
        InheritMovements(ref parent_switch, split2, 2);
        //50 reverse
        parent_switch = true;
        InheritMovements(ref parent_switch, split2, 3);
        //25
        InheritMovements(ref parent_switch, split3, 4);
        //25 reverse
        parent_switch = true;
        InheritMovements(ref parent_switch, split3, 5);
        //13
        InheritMovements(ref parent_switch, split4, 6);
        //keep best parent from the previous gen
        
        //InheritMovements(ref parent_switch, split4, 7);
        for (int i = 0; i < movenums; i++)
        {
            children_movements[7, i] = parent_movements[0, i];
        }
    }

    private void InheritMovements(ref bool parent_switch, int split, int childNum)
    {
        int split_counter = 0;
        for (int i = 0; i < movenums; i++)
        {
            if (!parent_switch)
            {
                children_movements[childNum, i] = parent_movements[0, i];
                split_counter++;
                if (split_counter == split)
                {
                    split_counter = 0;
                    parent_switch = !parent_switch;
                }
            }
            else
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

        string s = "";
        for (int x = 0; x < movenums; x++)
        {
            if (x == split)
            {
                s += "\t\t";
            }
            s += children_movements[childNum, x].ToString();
        }
        //Debug.Log("");
        //Debug.Log("Array2: " + s);
        parent_switch = false;
    }

    private void Mutation()
    {
        int rnd = 0;
        int rnd_dir = 0;
        for (int i = 0; i < pacnums-1; i++)
        {
            for (int j = 0; j < movenums; j+=2)
            {
                rnd = UnityEngine.Random.Range(0, 100);
                if (rnd < 5)
                {
                    rnd_dir = UnityEngine.Random.Range(1, 5);
                    children_movements[i, j] = rnd_dir;
                    children_movements[i, j+1] = rnd_dir;
                }
            }
        }
    }
}
