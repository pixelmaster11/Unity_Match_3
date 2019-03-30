using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class TextBasedBoardFillStrategy : IBoardFillStrategy
{
     
    public int FillBoard(int width, int height, int i, int j,  int map = 0, bool generateRandom = false)
    {
        return Load(width, height, i, j, generateRandom);           
    }


    private int Load(int width, int height, int i, int j, bool generateRandom)
    {
        string path = "Assets/Resources/Level1.txt";

        //Read the text from directly from the .txt file
        string[] data = File.ReadAllLines(path);

        ////FORMAT OF TEXT FILES (SEPARATOR IS BLANK SPACE ' ' ):
        //// 1 2 3 4 5 6 7 8 8 ... WIDTH
        ///  1 2 3 4 5 6 7 8 1 ... WIDTH
        ///  .
        ///  .
        ///  .
        ///  HEIGHT ..     ..  ... WIDTH
        ////

    


        if (generateRandom)
        {
            Random.InitState((int)System.DateTime.Now.Ticks);

            return Random.Range(1, 9);
        }

        //Check if we have properly dimensioned data in accordance with the board dimensions
        if (data.Length == height)
        {

            //Utils.DebugUtils.Log("Data Length:" +data.Length.ToString() + " " + j.ToString());
            return System.Int32.Parse(data[height - 1 - j].Split(' ')[i]);
       
        }

        else
        {
            Utils.DebugUtils.Log("File data height: " + data.Length + " does not match with board height: " + height);
            return 0;
        }

    
    }

}
