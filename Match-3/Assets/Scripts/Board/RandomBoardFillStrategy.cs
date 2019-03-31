using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBoardFillStrategy : IBoardFillStrategy
{
    public int FillBoard (int width, int height, int i, int j, int maxTileCodes, int map = 0, bool generateRandom = false)
    {
        //Random.InitState((int)System.DateTime.Now.Ticks);


        int randomNumber = Random.Range(0, 139);

        if (randomNumber < 30)
        {
            return 1;
        }

        else if(randomNumber >= 30 && randomNumber < 50)
        {
            return 2;
        }


        else if (randomNumber >= 50 && randomNumber < 52)
        {
            return 3;
        }

        else if (randomNumber >= 52 && randomNumber < 55)
        {
            return 4;
        }

        else if (randomNumber >= 55 && randomNumber < 60)
        {
            return 5;
        }

        else if (randomNumber >= 60 && randomNumber < 85)
        {
            return 6;
        }

        else if (randomNumber >= 85 && randomNumber < 100)
        {
            return 7;
        }

        else if (randomNumber >= 100 && randomNumber < 130)
        {
            return 8;
        }


        else if (randomNumber >= 130 && randomNumber < 132)
        {
            // return 9;
            return Random.Range(1, maxTileCodes + 1);
        }

        else
        {
            return Random.Range(1, maxTileCodes + 1);
        }


    }
}
