using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a simple board shuffling method based on Fischer - Yates algorithm extended for 2-d arrays 
/// </summary>
public class SimpleBoardShuffle : IBoardShuffleStrategy
{
    public void ShuffleBoard(int [,] board, Tile[,] tilesBoard, BoardManager boardManager, TileManager tileManager)
    {
        System.Random rand = new System.Random();

        int width = board.GetLength(0);
        int height = board.GetLength(1);

        for (int i = 0; i < width; i++)
        {

            for (int j = 0; j < height; j++)
            {

                int x = i + rand.Next(width - i);
                int y = j + rand.Next(height - j);

                tileManager.AnimateTile(tilesBoard[x, y], new Vector2(i, j), 1f);
                boardManager.SwapTilesOnBoard(new Vector2(x, y), new Vector2(i, j));

            }

        }


    }
}
