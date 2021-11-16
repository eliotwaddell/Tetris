using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    GameObject[,] tiles = new GameObject[20, 10];

    Color lit;
    Color unlit; 
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < tiles.GetLength(0); i++)
        {
            for(int j = 0; j < tiles.GetLength(1); j++)
            {
                tiles[i, j] = GameObject.Instantiate(Resources.Load("Tile")) as GameObject;
                tiles[i, j].transform.position = new Vector3(0.975f + (j * 0.45f), -4.35f + (i * 0.45f), 0);
            }
        }
    }

    public void UpdateBoard(int[,] board_data) // converts raw data to turn on and off tiles
    {
        for(int i = 0; i < board_data.GetLength(0); i++)
        {
            for(int j = 0; j < board_data.GetLength(1); j++)
            {
                if(board_data[i, j] == 0)
                {
                    tiles[i, j].GetComponent<Tile>().Off();
                }
                else
                {
                    tiles[i, j].GetComponent<Tile>().On();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
