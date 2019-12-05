using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    static int movenums = 200;
    public int[] movement = new int[movenums];

    bool movingUP = false;
    bool movingDOWN = false;
    bool movingLEFT = false;
    bool movingRIGHT = false;
    public bool movesDone = false;

    public int[,] path2D;
    private const int ROWS = 29;
    private const int COLS = 26;

    int index = 0;
    bool once = false;
    float pacman_move_row = 0;
    float pacman_move_col = 0;
    float pacman_current_row = 0;
    float pacman_current_col = 0;
    float pacman_speed = 10.0f;
    float finishTime = 0.0f;
    [NonSerialized]
    public double fitness = 0;
    int pellets_eaten = 0;

    string pellet_tag = "";
    Animator anim;


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
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        finishTime += Time.deltaTime;
        MovePacman();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ghost" || other.tag == "BigPellet")
        {
            if (!once)
            {
                once = true;
                if (other.tag == "Ghost")
                {
                    fitness = ((ROWS - 1) - pacman_current_row) + ((COLS - 1) - pacman_current_col);
                    //Debug.Log("Fit: " + fitness);
                    fitness = 53 - fitness;
                    fitness += pellets_eaten;
                    //fitness = Math.Round(Math.Sqrt(fitness), 5);
                }
                else if (other.tag == "BigPellet")
                {
                    fitness = 53;
                    double time_fitness = 40 - Math.Round((double)finishTime, 2);
                    fitness += pellets_eaten;
                    fitness += time_fitness;
                    fitness += 20; //Finish reward
                }
            }
            movesDone = true;
            anim.enabled = false;
        }

        if (other.tag == pellet_tag)
        {
            Destroy(other.gameObject);
            pellets_eaten += 5;
            //Debug.Log("Eating");
        }
    }


    public void SetPelletTag(string pTag) 
    {
        pellet_tag = pTag;
    }

    public void Fitness()
    {
        if (!once)
        {
            once = true;
            fitness = ((ROWS - 1) - pacman_current_row) + ((COLS - 1) - pacman_current_col);
            fitness = 53 - fitness;
            fitness += pellets_eaten;
            
            //Debug.Log("Fit: " + fitness);
            //fitness = Math.Round(Math.Sqrt(fitness), 5);
            //Debug.Log("Fitness: " + fitness);
        }
    }

    private void MovePacman()
    {
        if (index < movenums && !movesDone)
        {
            if (movement[index] == 1 && !movingRIGHT && !movingDOWN && !movingLEFT)
            {
                movingUP = true;
                transform.localScale = new Vector3(0.8f, 0.8f, 1);
                transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            else if (movement[index] == 2 && !movingLEFT && !movingDOWN && !movingUP)
            {
                movingRIGHT = true;
                transform.localScale = new Vector3(0.8f, 0.8f, 1);
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else if (movement[index] == 3 && !movingRIGHT && !movingLEFT && !movingUP)
            {
                movingDOWN = true;
                transform.localScale = new Vector3(0.8f, 0.8f, 1);
                transform.localRotation = Quaternion.Euler(0, 0, 270);
            }
            else if (movement[index] == 4 && !movingRIGHT && !movingDOWN && !movingUP)
            {
                movingLEFT = true;
                transform.localScale = new Vector3(-0.8f, 0.8f, 1);
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }

            if (movingUP)
            {
                int pacman_up = (int)pacman_current_row + 1;
                if (pacman_up < 29 && path2D[pacman_up, (int)pacman_current_col] != 0)
                {
                    pacman_move_row += pacman_speed * Time.deltaTime;
                    transform.position = new Vector3(pacman_current_col, pacman_move_row, 0.0f);

                    if (pacman_move_row > pacman_up)
                    {
                        transform.position = new Vector3(pacman_current_col, pacman_up, 0.0f);
                        pacman_current_row = pacman_up;
                        pacman_move_row = pacman_up;
                        movingUP = false;
                    }
                }
                else
                {
                    movingUP = false;
                }
            } 
            else if (movingRIGHT)
            {
                float pacman_right = (int)pacman_current_col + 1;
                if (pacman_right < 26 && path2D[(int)pacman_current_row, (int)pacman_right] != 0)
                {
                    pacman_move_col += pacman_speed * Time.deltaTime;
                    transform.position = new Vector3(pacman_move_col, pacman_current_row, 0.0f);

                    if (pacman_move_col > pacman_right)
                    {
                        transform.position = new Vector3(pacman_right, pacman_current_row, 0.0f);
                        pacman_current_col = pacman_right;
                        pacman_move_col = pacman_right;
                        movingRIGHT = false;
                    }
                }
                else
                {
                    movingRIGHT = false;
                }
            } 
            else if (movingDOWN)
            {
                float pacman_down = (int)pacman_current_row - 1;
                if (pacman_down >= 0 && path2D[(int)pacman_down, (int)pacman_current_col] != 0)
                {
                    pacman_move_row -= pacman_speed * Time.deltaTime;
                    transform.position = new Vector3(pacman_current_col, pacman_move_row, 0.0f);


                    if (pacman_move_row < pacman_down)
                    {
                        transform.position = new Vector3(pacman_current_col, pacman_down, 0.0f);
                        pacman_current_row = pacman_down;
                        pacman_move_row = pacman_down;
                        movingDOWN = false;
                    }
                }
                else
                {
                    movingDOWN = false;
                }
            } 
            else if (movingLEFT)
            {
                float pacman_left = (int)pacman_current_col - 1;
                if (pacman_left >= 0 && path2D[(int)pacman_current_row, (int)pacman_left] != 0)
                {

                    pacman_move_col -= pacman_speed * Time.deltaTime;
                    transform.position = new Vector3(pacman_move_col, pacman_current_row, 0.0f);


                    if (pacman_move_col < pacman_left)
                    {
                        transform.position = new Vector3(pacman_left, pacman_current_row, 0.0f);
                        pacman_current_col = pacman_left;
                        pacman_move_col = pacman_left;
                        movingLEFT = false;
                    }
                }
                else
                {
                    movingLEFT = false;
                }
            }

            if (!movingUP && !movingRIGHT && !movingDOWN && !movingLEFT)
            {
                index++;
                if (index >= movement.Length)
                {

                    Fitness();
                    movesDone = true;
                    //Debug.Log(finishTime);
                }
            }
        }
    }

    public void SetMoves(int[] moves)
    {
        movement = moves;
    }
    public void SetMovesFrom2D(int[,] moves, int childNum)
    {
        string str = "";
        for (int i =0; i < movenums; i++)
        {
            str += movement[i];
        }
        str = "";
        movement = new int[movenums];
        for (int i = 0; i < movenums; i++)
        {
            movement[i] = moves[childNum, i];
            str += movement[i];
        }
    }

    public void ResetPosition()
    {
        transform.position = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void ResetIndex()
    {
        index = 0;
        movesDone = false;
        once = false;
        pacman_move_row = 0;
        pacman_move_col = 0;
        pacman_current_row = 0;
        pacman_current_col = 0;
        transform.localScale = new Vector3(0.8f, 0.8f, 1);
        transform.localRotation = Quaternion.Euler(0, 0, 90);
        anim.enabled = true;
        finishTime = 0.0f;
        pellets_eaten = 0;
        DestroyPellets();
    }

    public int[] GetMoveArray()
    {
        return movement;
    }

    public void DestroyPellets()
    {
        for (int i = 0; i < 8; i++)
        {
            var Pellets = GameObject.FindGameObjectsWithTag("Pellet" + i.ToString());
            foreach(var pellet in Pellets) {
                Destroy(pellet);
            }
        }
        
    }
}
