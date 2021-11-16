using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameBoard : MonoBehaviour
{
    public GameObject TileBoard;

    GameObject block;

    int[,] board_data = new int[20, 10];

    Scoreboard sb;

    public TextMeshProUGUI game_over_text;

    float block_drop; // floats related to time
    float tick_rate;
    float move_cooldown;

    bool game_time_started = false;
    bool game_over = false;
    
    public void BeginGameTime()
    {
        game_time_started = true;
    }

    public void Reset()
    {
        game_time_started = false;
        game_over = false;
        game_over_text.color = new Color(255f, 255f, 255f, 0f);
        sb.GetComponent<Scoreboard>().Reset();
        Start();
    }

    //zeroes indicate empty space
    void Start()
    {
        block_drop = 0f;
        sb = GetComponent<Scoreboard>();
        for(int i = 0; i < board_data.GetLength(0); i++)
        {
            for(int j = 0; j < board_data.GetLength(1); j++)
            {
                if(i < 0)
                {
                    board_data[i, j] = 1;
                }
                else
                {
                    board_data[i, j] = 0;
                }
            }
        }
        TileBoard.GetComponent<TileBoard>().UpdateBoard(board_data);
        StartCoroutine("SpawnBlock");
    }

    void Update()
    {
        if(game_over)
        {
            game_time_started = false;
            game_over_text.color = new Color(255f, 255f, 255f, 255f);
        }


        if(game_time_started)
        {
            if(Input.GetKey(KeyCode.S))
            {
                tick_rate = 0.1f;
            }
            else
            {
                tick_rate = 0.5f;
            }


            block_drop += Time.deltaTime;
            if(block_drop >= tick_rate)
            {
                if(!BlockTouchingGround())
                {
                    block_drop -= tick_rate;
                    BlockDrop();
                }
                else
                {
                    StartCoroutine("SpawnBlock");
                }
            }
            


            if(Input.GetKeyDown(KeyCode.A))
            {
                MoveBlock(-1);
                move_cooldown = 0f;
            }
            if(Input.GetKey(KeyCode.A))
            {
                move_cooldown += Time.deltaTime;
                if(move_cooldown > 0.2f)
                {
                    MoveBlock(-1);
                    move_cooldown -= 0.2f;
                }
            }


            if(Input.GetKeyDown(KeyCode.D))
            {
                MoveBlock(1);
                move_cooldown = 0f;
            }
            if(Input.GetKey(KeyCode.D))
            {
                move_cooldown += Time.deltaTime;
                if(move_cooldown > 0.2f)
                {
                    MoveBlock(1);
                    move_cooldown -= 0.2f;
                }
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                if(CanRotate())
                {
                    EraseBlockPosition();
                    block.GetComponent<Block>().Rotate();
                }
            }



            UpdateBoardData();
            TileBoard.GetComponent<TileBoard>().UpdateBoard(board_data);
        }
    }

    void UpdateBoardData()
    {
        int xpos = block.GetComponent<Block>().location[1];
        int ypos = block.GetComponent<Block>().location[0];

        int imax = Mathf.Min(4, ypos + 1);

        int jmin = Mathf.Max(0 - xpos, 0);
        int jmax = Mathf.Min(10 - xpos, 4);
        
        for(int i = 0; i < imax; i++)// the bigger the i, the further down the board
        {
            for(int j = jmin; j < jmax; j++)
            {
                //Debug.Log("i: " + (i) + " j: " + (j));
                if(board_data[ypos - i, xpos + j] == 0)
                {
                    board_data[ypos - i, xpos + j] = block.GetComponent<Block>().shape_data[i, j];
                }
            }
        }
        
    }

    void GameOver()
    {
        int xpos = block.GetComponent<Block>().location[1];
        int ypos = block.GetComponent<Block>().location[0];
        int[,] dimensions = block.GetComponent<Block>().shape_data;
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                if(dimensions[i, j] == 1)
                {
                    if(board_data[ypos - i, xpos + j] == 1)
                    {
                        game_over = true;
                    }
                }
            }
        }
    }

    void BlockDrop() //erases and moves current block position data down one
    {
        EraseBlockPosition();
        block.GetComponent<Block>().location[0]--;
    }

    bool CanMoveRight()
    {
        int xpos = block.GetComponent<Block>().location[1];
        int ypos = block.GetComponent<Block>().location[0];
        int[,] dimensions = block.GetComponent<Block>().shape_data;
        int jmin = Mathf.Max(0 - xpos, 0);
        int jmax = Mathf.Min(10 - xpos, 4);

        if(xpos + 3 < 9) // if its not touching right wall do these things
        {
            for(int i = 0; i < 4; i++)  //scan for all leftmost blocks of each row
            {
                for(int j = jmax - 1; j > jmin - 1; j--)
                {
                    if(dimensions[i, j] == 1) //check if there are blocks next to blocks of the block XD LAWL
                    {
                        if(board_data[ypos - i, xpos + j + 1] != 0)
                        {
                            return false;
                        }
                        break;
                    }
                }
            }
            return true;
        }
        else // if it is or if it's past it, do these
        {
            int j = jmax - 1;
            for(int i = 0; i < 4; i++) //scan rightmost column
            {
                if(dimensions[i, j] != 0 || board_data[ypos - i, xpos + j] != 0)
                {
                    return false;
                }
            }
            return true;
            
        }
    }

    bool CanMoveLeft() //can move left in set cases
    {
        int xpos = block.GetComponent<Block>().location[1];
        int ypos = block.GetComponent<Block>().location[0];
        int[,] dimensions = block.GetComponent<Block>().shape_data;
        int jmin = Mathf.Max(0 - xpos, 0);
        int jmax = Mathf.Min(10 - xpos, 4);

        if(xpos > 0) // if its not on the leftmost column it can move IF there is not tiles to the left of it
        {
            for(int i = 0; i < 4; i++)  //scan for all leftmost blocks of each row
            {
                for(int j = jmin; j < jmax; j++)
                {
                    if(dimensions[i, j] == 1) //check if there are blocks next to blocks of the block XD LAWL
                    {
                        if(board_data[ypos - i, xpos + j - 1] != 0)
                        {
                            return false;
                        }
                        break;
                    }
                }
            }

            return true;
        }
        else // if its less than or equal to zero there is a special method for checking
        {
            int j = jmin;
            for(int i = 0; i < 4; i++) //scan leftmost column
            {
                if(dimensions[i, j] != 0 || board_data[ypos - i - 1, xpos + j] != 0)
                {
                    return false;
                }
            }
            return true;
        }
    }

    bool CanRotate()
    {
        int[,] rotated = new int[4, 4];// copy of rotated array for checking
        for (int i = 0; i < 4; i++) //copies the shape_data so we don't modify the original
        {
            for (int j = 0; j < 4; j++) 
            {
                rotated[i, j] = block.GetComponent<Block>().shape_data[i, j];
            }
        }
        

        for (int i = 0; i < 4; i++) //reverses columns
        {
            for (int j = 0, k = 3; j < k; j++, k--) 
            {
                int temp = rotated[j, i];
                rotated[j, i] = rotated[k, i];
                rotated[k, i] = temp;
            }
        }

        for (int i = 0; i < 4; i++) //transposes
        {
            for (int j = i; j < 4; j++) 
            {
                int temp = rotated[j, i];
                rotated[j, i] = rotated[i, j];
                rotated[i, j] = temp;
            }
        }

        int xpos = block.GetComponent<Block>().location[1];
        int ypos = block.GetComponent<Block>().location[0];

        int imax = Mathf.Min(4, ypos + 1);

        int jmin = Mathf.Max(0 - xpos, 0);
        int jmax = Mathf.Min(10 - xpos, 4);
        
        for(int i = 0; i < 4; i++) //check rotated array to see if there are any conflicts
        {
            for(int j = 0; j < 4; j++)
            {
                if(rotated[i, j] == 1)
                {
                    if(j >= jmax || j < jmin)
                    {
                        return false;
                    }
                    else if(i > imax - 1)
                    {
                        return false;
                    }
                    else if(board_data[ypos - i, xpos + j] == 1 && block.GetComponent<Block>().shape_data[i, j] == 0)
                    {
                        return false;
                    }
                }
            }
        }
        return true;


    }

    void MoveBlock(float dir) //positive for right, negative for left
    {
        if(dir > 0) //right
        {
            if(CanMoveRight())
            {
                EraseBlockPosition();
                block.GetComponent<Block>().location[1]++;
            }
        }
        else if(dir < 0) //left
        {
            if(CanMoveLeft())
            {
                EraseBlockPosition();
                block.GetComponent<Block>().location[1]--;
            }
        }
    }

    void EraseBlockPosition()
    {
        int xpos = block.GetComponent<Block>().location[1];
        int ypos = block.GetComponent<Block>().location[0];

        int imax = Mathf.Min(4, ypos + 1);

        int jmin = Mathf.Max(0 - xpos, 0);
        int jmax = Mathf.Min(10 - xpos, 4);
        
        for(int i = 0; i < imax; i++)
        {
            for(int j = jmin; j < jmax; j++)
            {
                if(block.GetComponent<Block>().shape_data[i, j] == 1)
                {
                    board_data[ypos - i, xpos + j] = 0;
                }
            }
        }
    }
    bool BlockTouchingGround()
    {
        int xpos = block.GetComponent<Block>().location[1];
        int ypos = block.GetComponent<Block>().location[0];

        int imin = Mathf.Max(0, 2 - ypos);

        int jmin = Mathf.Max(0 - xpos, 0);
        int jmax = Mathf.Min(10 - xpos, 4);


        int[,] dimensions = block.GetComponent<Block>().shape_data; // save memory because we dont need to modify it, just make copy

        for(int j = jmin; j < jmax; j++) //bottom up, scan by column
        {
            for(int i = 3; i > imin - 1; i--)
            {
                if(dimensions[i, j] == 1)
                {
                    if(ypos - i == 0) // its on the lowest possible layer
                    {
                        return true;
                    }
                    else if(board_data[ypos - i - 1, xpos + j] == 1) //tile under found block
                    {
                        return true;
                    }
                    
                    break;
                }
            }
        }
        
        return false;
    }

    IEnumerator SpawnBlock()
    {
        int shape = Random.Range(0, 6);
        if(shape == 0) // follow the enum conventions
        {
            block = GameObject.Instantiate(Resources.Load("IBlock")) as GameObject;
        } 
        else if(shape == 1) 
        {
            block = GameObject.Instantiate(Resources.Load("OBlock")) as GameObject;
        }
        else if(shape == 2)
        {
            block = GameObject.Instantiate(Resources.Load("TBlock")) as GameObject;
        }
        else if(shape == 3)
        {
            block = GameObject.Instantiate(Resources.Load("SBlock")) as GameObject;
        }
        else if(shape == 4)
        {
            block = GameObject.Instantiate(Resources.Load("ZBlock")) as GameObject;
        }
        else if(shape == 5)
        {
            block = GameObject.Instantiate(Resources.Load("JBlock")) as GameObject;
        }
        else if(shape == 6)
        {
            block = GameObject.Instantiate(Resources.Load("LBlock")) as GameObject;
        }

        yield return StartCoroutine("CheckRows");

        block.GetComponent<Block>().Init();
        GameOver(); // checks to see if the user has trapped themselves
    }

    IEnumerator CheckRows()
    {
        for(int i = 0; i < 20; i++)
        {
            bool full_row = true;
            for(int j = 0; j < 10; j++)
            {
                if(board_data[i, j] == 0)
                {
                    full_row = false;
                }
            }
            if(full_row)
            {
                for(int k = 0; k < 10; k++)
                {
                    board_data[i, k] = 0;
                }
                yield return new WaitForSeconds(.5f);
                Gravity(i);
                i--;
            }
        }
    }

    void Gravity(int row_index)
    {
        for(int i = row_index; i < 19; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                board_data[i, j] = board_data[i + 1, j];
            }
        }

        sb.GetComponent<Scoreboard>().UpdateScore(100);
        UpdateBoardData();
        TileBoard.GetComponent<TileBoard>().UpdateBoard(board_data);
    }
}
