using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PacMan : MonoBehaviour {

    private const int pacman_start_row = 6;
    private const int pacman_start_col = 12;
    public float pacman_speed = 4.0f;

    public Board board;
    public Text score;
    int score_counter = 0;
    private bool movingLEFT = false;
    private bool movingRIGHT = false;
    private bool movingUP = false;
    private bool movingDOWN = false;

    [NonSerialized]
    public bool pacman_chase = false;
    float chaseTime = 0;

    [NonSerialized]
    public float pacman_current_row;
    [NonSerialized]
    public float pacman_current_col;
    [NonSerialized]
    public float pacman_eaten_row;
    [NonSerialized]
    public float pacman_eaten_col;

    //FOR PINKY - 4 tiles ahead of pacman
    [NonSerialized]
    public float pacman_ahead_col;
    [NonSerialized]
    public float pacman_ahead_row;
    //FOR INKY - BASED ON BLINKY'S POSITION

    float pacman_move_row;
    float pacman_move_col;
    int eaten_pellet = 0;
    int max_tiles_ahead = 10;

    public List<int> directions = new List<int>(); // 1:North 2:East 3:South 4:West

    [NonSerialized]
    public int sequence = 200;

    // Use this for initialization
    void Start ()
    {
        transform.position = new Vector3(pacman_start_col, pacman_start_row, 0);

        pacman_current_row = pacman_start_row;
        pacman_current_col = pacman_start_col;

        pacman_move_row = pacman_start_row;
        pacman_move_col = pacman_start_col;

        pacman_eaten_row = pacman_start_row;
        pacman_eaten_col = pacman_start_col;

        pacman_ahead_row = pacman_start_row;
        pacman_ahead_col = pacman_start_col;

    }

    // Update is called once per frame
    void Update()
    {
        if (eaten_pellet == 246)
        {
            SceneManager.LoadScene(0);
        }
        Chase();
        CheckInput ();
        MovePacman();
        UpdateOrientation();
    }

    private void Chase()
    {
        if (pacman_chase)
        {
            chaseTime += Time.deltaTime;
            //Debug.Log(chaseTime);
            if (chaseTime > 5)
            {
                Debug.Log("Chase over");
                pacman_chase = false;
                chaseTime = 0;
            }
        }
    }

    void CheckInput () {

		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
            if (!movingRIGHT && !movingDOWN && !movingUP)
            {
                movingLEFT = true;
            }

		} else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
            if (!movingLEFT && !movingDOWN && !movingUP) 
            {
                movingRIGHT = true;
            }

		} else if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
            if (!movingRIGHT && !movingDOWN && !movingLEFT)
            {
                movingUP = true;
            }

		} else if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
            if (!movingRIGHT && !movingLEFT && !movingUP)
            {
                movingDOWN = true;
            }
		}
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pellet")
        {
            Destroy(other.gameObject);
            eaten_pellet++;
            score_counter += 10;
            score.text = "Score: " + score_counter;
            //Debug.Log("Eaten Pellets: " + eaten_pellet + "\tRow: " + pacman_eaten_row + "\tCol: " + pacman_eaten_col);
            board.map2D[(int)pacman_current_row, (int)pacman_eaten_col] = 0;

        }

        if (other.tag == "BigPellet")
        {
            Destroy(other.gameObject);
            Debug.Log("POWER MODE");
            pacman_chase = true;
            chaseTime = 0;
            eaten_pellet++;
            score_counter += 50;
            score.text = "Score: " + score_counter;
            //Debug.Log("Eaten Pellets: " + eaten_pellet + "\tRow: " + pacman_eaten_row + "\tCol: " + pacman_eaten_col);
            //Debug.Log("Eaten Big Pellets: " + board.map2D[(int)pacman_current_row, (int)pacman_eaten_col]);
            board.map2D[(int)pacman_current_row, (int)pacman_eaten_col] = 0;
        }

        if (other.tag == "Ghost")
        {
            if (!pacman_chase)
            {
                SceneManager.LoadScene(0);
            } else
            {
                score_counter += 100;
                score.text = "Score: " + score_counter;
            }
        }
    }

    void MovePacman()
    {
        if (movingLEFT)
        {
            float pacman_left = (int)pacman_current_col - 1;
            if (pacman_left >= 0 && board.path2D[(int)pacman_current_row, (int)pacman_left] != 0)
            {
                pacman_eaten_col = pacman_left;
                //Pinky tiles ahead
                for (int tiles_ahead = 0; tiles_ahead <= max_tiles_ahead; tiles_ahead++)
                {
                    if (pacman_eaten_col - tiles_ahead >= 0)
                    {
                        if (board.path2D[(int)pacman_current_row, (int)(pacman_eaten_col - tiles_ahead)] == 1 ||
                            board.path2D[(int)pacman_current_row, (int)(pacman_eaten_col - tiles_ahead)] == 2)
                        {
                            pacman_ahead_col = pacman_eaten_col - tiles_ahead;
                            pacman_ahead_row = pacman_current_row;
                            //Debug.Log("PACMAN AHEAD: " + pacman_ahead_col + "\t" + pacman_ahead_row);
                        }
                    }
                }

                pacman_move_col -= pacman_speed * Time.deltaTime;
                transform.position = new Vector3(pacman_move_col, pacman_current_row, 0.0f);


                if (pacman_move_col < pacman_left)
                {
                    transform.position = new Vector3(pacman_left, pacman_current_row, 0.0f);
                    pacman_current_col = pacman_left;
                    pacman_move_col = pacman_left;
                    movingLEFT = false;
                    //Left++;
                    //Debug.Log("Left: " + Left);

                    if (directions.Count >= sequence)
                    {
                        directions.RemoveAt(0);
                    } 
                    directions.Add(4);
                }
            } else
            {
                movingLEFT = false;
            }
        }
        else if (movingRIGHT)
        {
            float pacman_right = (int)pacman_current_col + 1;
            if (pacman_right < 26 && board.path2D[(int)pacman_current_row, (int)pacman_right] != 0)
            {
                pacman_eaten_col = pacman_right;
                //Pinky tiles ahead
                for (int tiles_ahead = 0; tiles_ahead <= max_tiles_ahead; tiles_ahead++)
                {
                    if (pacman_eaten_col + tiles_ahead < 26)
                    {
                        if (board.path2D[(int)pacman_current_row, (int)(pacman_eaten_col + tiles_ahead)] == 1 ||
                            board.path2D[(int)pacman_current_row, (int)(pacman_eaten_col + tiles_ahead)] == 2)
                        {
                            pacman_ahead_col = pacman_eaten_col + tiles_ahead;
                            pacman_ahead_row = pacman_current_row;
                            //Debug.Log("PACMAN AHEAD: " + pacman_ahead_col + "\t" + pacman_ahead_row);
                        }
                    }
                }

                pacman_move_col += pacman_speed * Time.deltaTime;
                transform.position = new Vector3(pacman_move_col, pacman_current_row, 0.0f);

                if (pacman_move_col > pacman_right)
                {
                    transform.position = new Vector3(pacman_right, pacman_current_row, 0.0f);
                    pacman_current_col = pacman_right;
                    pacman_move_col = pacman_right;
                    movingRIGHT = false;
                    //Right++;
                    //Debug.Log("Right: " + Right);

                    if (directions.Count >= sequence)
                    {
                        directions.RemoveAt(0);
                    }
                    directions.Add(2);
                }
            } else
            {
                movingRIGHT = false;
            }
        }
        else if (movingUP)
        {
            float pacman_up = (int)pacman_current_row + 1;
            if (pacman_up < 29 && board.path2D[(int)pacman_up, (int)pacman_current_col] != 0)
            {
                pacman_eaten_row = pacman_up;
                //Pinky tiles ahead
                for (int tiles_ahead = 0; tiles_ahead <= max_tiles_ahead; tiles_ahead++)
                {
                    if (pacman_eaten_row + tiles_ahead < 29)
                    {
                        if (board.path2D[(int)pacman_eaten_row + tiles_ahead, (int)(pacman_current_col)] == 1 ||
                            board.path2D[(int)pacman_eaten_row + tiles_ahead, (int)(pacman_current_col)] == 2)
                        {
                            pacman_ahead_row = pacman_eaten_row + tiles_ahead;
                            pacman_ahead_col = pacman_current_col;
                            //Debug.Log("PACMAN AHEAD: " + pacman_ahead_col + "\t" + pacman_ahead_row);
                        }
                    }
                }
                pacman_move_row += pacman_speed * Time.deltaTime;
                transform.position = new Vector3(pacman_current_col, pacman_move_row, 0.0f);


                if (pacman_move_row > pacman_up)
                {
                    transform.position = new Vector3(pacman_current_col, pacman_up, 0.0f);
                    pacman_current_row = pacman_up;
                    pacman_move_row = pacman_up;
                    movingUP = false;
                    //Up++;
                    //Debug.Log("Up: " + Up);

                    if (directions.Count >= sequence)
                    {
                        directions.RemoveAt(0);
                    }
                    directions.Add(1);
                }
            }
            else
            {
                movingUP = false;
            }
        }
        else if (movingDOWN)
        {
            float pacman_down = (int)pacman_current_row - 1;
            if (pacman_down >= 0 && board.path2D[(int)pacman_down, (int)pacman_current_col] != 0)
            {
                pacman_eaten_row = pacman_down;
                //Pinky tiles ahead
                for (int tiles_ahead = 0; tiles_ahead <= max_tiles_ahead; tiles_ahead++)
                {
                    if (pacman_eaten_row - tiles_ahead >= 0)
                    {
                        if (board.path2D[(int)pacman_eaten_row - tiles_ahead, (int)(pacman_current_col)] == 1 ||
                            board.path2D[(int)pacman_eaten_row - tiles_ahead, (int)(pacman_current_col)] == 2)
                        {
                            pacman_ahead_row = pacman_eaten_row - tiles_ahead;
                            pacman_ahead_col = pacman_current_col;
                            //Debug.Log("PACMAN AHEAD: " + pacman_ahead_col + "\t" + pacman_ahead_row);
                        }
                    }
                }

                pacman_move_row -= pacman_speed * Time.deltaTime;
                transform.position = new Vector3(pacman_current_col, pacman_move_row, 0.0f);


                if (pacman_move_row < pacman_down)
                {
                    transform.position = new Vector3(pacman_current_col, pacman_down, 0.0f);
                    pacman_current_row = pacman_down;
                    pacman_move_row = pacman_down;
                    movingDOWN = false;
                    //Down++;
                    //Debug.Log("Down: " + Down);

                    if (directions.Count >= sequence)
                    {
                        directions.RemoveAt(0);
                    }
                    directions.Add(3);
                }
            }
            else
            {
                movingDOWN = false;
            }
        }
        
    }

	void UpdateOrientation () {

		if (movingLEFT) 
        {
			transform.localScale = new Vector3 (-0.8f, 0.8f, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 0);
		} 
        else if (movingRIGHT) 
        {
			transform.localScale = new Vector3 (0.8f, 0.8f, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 0);
		} 
        else if (movingUP) 
        {
			transform.localScale = new Vector3 (0.8f, 0.8f, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 90);

		} 
        else if (movingDOWN) 
        {
			transform.localScale = new Vector3 (0.8f, 0.8f, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 270);
		}
	}
    
}
