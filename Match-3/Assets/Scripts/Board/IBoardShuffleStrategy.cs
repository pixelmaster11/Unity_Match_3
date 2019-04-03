using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoardShuffleStrategy 
{
    void ShuffleBoard(int [,] board, Tile [,] tilesBoard, BoardManager boardManager, TileManager tileManage);
	
}
