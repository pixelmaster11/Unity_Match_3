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


    public int[,] GetFilledBoard(int width, int height, int map = 0)
    {
        //check dimensions of map with board dimensions 
        if (width != map1.GetLength(0) || height != map1.GetLength(1))
        {
            Utils.DebugUtils.LogError("Level data does not match with board data, please check dimensions");
            return null;
        }

        return map1;

    }
}
