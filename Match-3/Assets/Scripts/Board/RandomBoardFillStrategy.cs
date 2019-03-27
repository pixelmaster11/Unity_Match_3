using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBoardFillStrategy : IBoardFillStrategy
{
    public int FillBoard (int width, int height, int i, int j, int map = 0, bool generateRandom = false)
    {
        //Random.InitState((int)System.DateTime.Now.Ticks);

        return Random.Range(1, 9);
       
    }
}
