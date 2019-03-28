using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnotherShuffle : IBoardShuffleStrategy
{
    public void ShuffleBoard(ref int[,] board, ref Tile[,] tilesBoard, BoardManager boardManager, TileManager tileManager)
    {
        System.Random rand = new System.Random();

        int height = board.GetUpperBound(1) + 1;
      
        for (int n = board.Length; n > 0;)
        {
            int k = rand.Next(n);
            --n;

            int dr = n / height;
            int dc = n % height;
            int sr = k / height;
            int sc = k % height;

            tileManager.AnimateTile(tilesBoard[sr, sc], new Vector2(dr, dc), 1f);
            boardManager.SwapTilesOnBoard(new Vector2(sr, sc), new Vector2(dr, dc));


        }
    }
}
