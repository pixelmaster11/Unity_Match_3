using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a type of board fill method. It will randomly return a tilecode to generate a tile. 
/// Can be extended to have weighted probabilites per level to have more control on which tiles should appear more on a particular level
/// </summary>
public class RandomBoardFillStrategy : IBoardFillStrategy
{
    public int FillBoard (int width, int height, int i, int j, int maxTileCodes, int map = 0, bool generateRandom = false)
    {
       

        int randomNumber = Random.Range(0,100);

        if (randomNumber <= 96)
            return Random.Range(1, maxTileCodes + 1);
        else
            return Constants.BLOCK_TILE_CODE;


        // TODO: WEIGHTED PROBABLITIES
       

    }
}
