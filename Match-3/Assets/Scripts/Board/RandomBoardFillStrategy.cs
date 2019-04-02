using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBoardFillStrategy : IBoardFillStrategy
{
    public int FillBoard (int width, int height, int i, int j, int maxTileCodes, int map = 0, bool generateRandom = false)
    {
        //Random.InitState((int)System.DateTime.Now.Ticks);


        int randomNumber = Random.Range(0,100);

        if (randomNumber <= 96)
            return Random.Range(1, maxTileCodes + 1);
        else
            return Constants.BLOCK_TILE_CODE;

        /* TODO: WEIGHTED PROBABLITIES
        if (randomNumber < 30)
        {
            return Constants.RED_TILE_CODE;
        }

        else if(randomNumber >= 30 && randomNumber < 50)
        {
            return Constants.GREEN_TILE_CODE;
        }


        else if (randomNumber >= 50 && randomNumber < 52)
        {
            return Constants.ORANGE_TILE_CODE;
        }

        else if (randomNumber >= 52 && randomNumber < 55)
        {
            return Constants.PURPLE_TILE_CODE;
        }

        else if (randomNumber >= 55 && randomNumber < 60)
        {
            return Constants.AQUA_TILE_CODE;
        }

        else if (randomNumber >= 60 && randomNumber < 85)
        {
            return Constants.YELLOW_TILE_CODE;
        }

        else if (randomNumber >= 85 && randomNumber < 100)
        {
            return Constants.BLUE_TILE_CODE;
        }

        else if (randomNumber >= 100 && randomNumber < 130)
        {
            return Constants.PINK_TILE_CODE;
        }


        else if (randomNumber >= 130 && randomNumber < 132)
        {
            
            return Constants.BLOCK_TILE_CODE;
        }

        else
        {
            return Random.Range(1, maxTileCodes + 1);
        }
        */

    }
}
