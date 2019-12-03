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
    float pacman_speed = 20.0f;
    public double fitness = 0;
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
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePacman();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ghost" || other.tag == "BigPellet")
        {
            Fitness();
            movesDone = true;
            anim.enabled = false;
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
                }
            }
        }
    }

    public void Fitness()
    {
        if (!once)
        {
            once = true;
            fitness = ((ROWS - 1) - pacman_current_row) + ((COLS - 1) - pacman_current_col);
            fitness = Math.Round(Math.Sqrt(fitness), 5);
            //Debug.Log("Fitness: " + fitness);
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
        //Debug.Log("=================================================================================");
        //Debug.Log(str);
        str = "";
        movement = new int[movenums];
        for (int i = 0; i < movenums; i++)
        {
            movement[i] = moves[childNum, i];
            str += movement[i];
        }
        //Debug.Log(str);
        //Debug.Log("=================================================================================");
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
    }

    public int[] GetMoveArray()
    {
        return movement;
    }
}
