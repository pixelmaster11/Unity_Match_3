using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualBoardFillStrategy : IBoardFillStrategy
{
   
    public readonly static int[,] map1 = { { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2 },
                                           { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2 },
                                           { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2 },
                                           { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2 },
                                           { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2 },
                                           { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2 },
                                           { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2 },
                                           { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2 },
                                           { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2 },
                                           { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2 },
                                          };


    public int FillBoard(int width, int height, int i, int j, int map = 0, bool generateRandom = false)
    {
     
       
        if(generateRandom)
        {
            Random.InitState((int)System.DateTime.Now.Ticks);

            return Random.Range(1, 9);
        }

        //check dimensions of map with board dimensions 
        if (width != map1.GetLength(0) || height != map1.GetLength(1))
        {
            Utils.DebugUtils.LogError("Level data does not match with board data, please check dimensions");
            return 0;
        }

        return map1[i , j];

    }
}
