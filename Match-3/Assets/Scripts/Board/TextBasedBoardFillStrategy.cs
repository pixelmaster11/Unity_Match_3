using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class TextBasedBoardFillStrategy : IBoardFillStrategy
{
    public int[,] GetFilledBoard(int width, int height, int map = 0)
    {
        return Load(width, height);    
        
    }


    private int [,] Load(int width, int height)
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

        //Check if we have properly dimensioned data in accordance with the board dimensions
        if(data.Length == width)
        {
            int[,] board = new int[width, height];


            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                   
                    board[i, j] = System.Int32.Parse(data[i].Split(' ')[j]);

                }
            }

            return board;
        }

        else
        {
            Utils.DebugUtils.Log("File data width: " + data.Length + " does not match with board width: " + width);
            return null;
        }

    
    }

}
