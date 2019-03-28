using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoardShuffleStrategy 
{
    void ShuffleBoard(ref int [,] board, ref Tile [,] tilesBoard, BoardManager boardManager, TileManager tileManage);
	
}
