using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public int[] parent_movement;
    Movement[] moveScript;
    GameObject[] pacman;

    bool once = false;
    bool generationDone = false;
    double[] fitness;

    SortedSet<FitnessScore> openList;

    class FitnessComparer : IComparer<FitnessScore>
    {
        public int Compare(FitnessScore x, FitnessScore y)
        {
            //first by age
            int result = x.Fitness.CompareTo(y.Fitness);

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

        
        int move;
        pacman = new GameObject[pacnums];
        moveScript = new Movement[pacnums];
        for (int i = 0; i < pacnums; i++)
        {
            pacman[i] = Instantiate(pacmanPrefab, transform.position, transform.rotation);
            moveScript[i] = pacman[i].GetComponent<Movement>();
            movement = new int[movenums];

            for (int j = 0; j < movenums; j++)
            {
                move = UnityEngine.Random.Range(1, 5);
                movement[j] = move;
            }
            moveScript[i].SetMoves(movement);
        }

    }

    // Update is called once per frame
    void Update()
    {
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
                Debug.Log("generationDone");
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
                //DO ONLY ONCE
                once = true;
                Selection();
                Crossover();
                Mutation();
            }
            
        }
    }

    private void Selection()
    {
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
            Debug.Log("Fitness " + i + ": " + fitness[i]);
            Debug.Log(str);
            openList.Add(fn);
        }
        Debug.Log("\n");

        int parentCounter = 2;
        parent_movement = new int[movenums];
        while (openList.Count != 0)
        {
            FitnessScore p = openList.First();
            openList.Remove(p);

            if (parentCounter > 0)
            {
                parent_movement = p.MoveArray;
                Debug.Log("Fitness: " + p.Fitness);
                string str = "";
                foreach (int x in p.MoveArray)
                {
                    str += x.ToString();
                }
                Debug.Log("Array: " + str);
                parentCounter--;
            }
        }
    }

    private void Crossover()
    {
        //Mix parents movement[] to create 8 variety of movement[] 
        //1/2
        //1/2 reverse
        //1/4
        //1/4 reverse
        //1/8
        //1/8 reverse
        //keep top 2 from the previous gen
    }

    private void Mutation()
    {

    }
}
