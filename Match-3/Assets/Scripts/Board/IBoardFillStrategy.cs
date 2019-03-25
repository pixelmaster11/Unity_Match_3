using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This interface defines the fill method for board
/// 1. Random 2.String based (Manual) 3. Text-based
/// </summary>
public interface IBoardFillStrategy 
{

    int[ , ] GetFilledBoard(int width, int height, int map = 0);
	
}
