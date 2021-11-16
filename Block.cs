using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum BlockType
    {
        I, O, T, S, Z, J, L
    }

    public int[,] shape_data = new int[4, 4]; // where the tiles are in the shape (each fits in a 2x4 at max)

    public int[] location = new int[2]; // top left coordinates on board, always starts at {0, 3}

    public int rotation = 0; //default, each increment is 90 degrees
    
    public BlockType shape;

    public void Rotate()
    {
        for (int i = 0; i < 4; i++) //reverses columns
        {
            for (int j = 0, k = 3; j < k; j++, k--) 
            {
                int temp = shape_data[j, i];
                shape_data[j, i] = shape_data[k, i];
                shape_data[k, i] = temp;
            }
        }

        for (int i = 0; i < 4; i++) //transposes
        {
            for (int j = i; j < 4; j++) 
            {
                int temp = shape_data[j, i];
                shape_data[j, i] = shape_data[i, j];
                shape_data[i, j] = temp;
            }
        }
    }

    // initialize block
    public void Init()
    {
        for(int i = 0; i < 4; i++) //initialize to all zeroes
        {
            for(int j = 0; j < 4; j++)
            {
                shape_data[i, j] = 0;
            }
        }

        location[0] = 19;
        location[1] = 3;

        if(shape == BlockType.I)
        {
            shape_data[1, 0] = 1;
            shape_data[1, 1] = 1;
            shape_data[1, 2] = 1;
            shape_data[1, 3] = 1;
        }
        else if(shape == BlockType.O)
        {
            shape_data[1, 1] = 1;
            shape_data[1, 2] = 1;
            shape_data[2, 1] = 1;
            shape_data[2, 2] = 1;
        }
        else if(shape == BlockType.T)
        {
            shape_data[1, 1] = 1;
            shape_data[2, 0] = 1;
            shape_data[2, 1] = 1;
            shape_data[2, 2] = 1;
        }
        else if(shape == BlockType.S)
        {
            shape_data[1, 1] = 1;
            shape_data[1, 2] = 1;
            shape_data[2, 0] = 1;
            shape_data[2, 1] = 1;
        }
        else if(shape == BlockType.Z)
        {
            shape_data[1, 1] = 1;
            shape_data[1, 2] = 1;
            shape_data[2, 2] = 1;
            shape_data[2, 3] = 1;
        }
        else if(shape == BlockType.J)
        {
            shape_data[0, 1] = 1;
            shape_data[1, 1] = 1;
            shape_data[2, 0] = 1;
            shape_data[2, 1] = 1;
        }
        else // L bozo
        {
            shape_data[0, 2] = 1;
            shape_data[1, 2] = 1;
            shape_data[2, 2] = 1;
            shape_data[2, 3] = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
