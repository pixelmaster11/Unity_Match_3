using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBoardFillStrategy : IBoardFillStrategy
{
    public int[, ] GetFilledBoard (int width, int height, int map = 0)
    {
        Random.InitState((int)System.DateTime.Now.Ticks);

        int[,] board = new int[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                board[i, j] = Random.Range(1, 9);        

            }
        }

        return board;
    }
}
