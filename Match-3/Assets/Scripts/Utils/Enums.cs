using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores all game enums
/// </summary>
public class Enums
{

    public enum TileType
    {
        Normal,
        Bonus,
        Block,
        Bomb
    };


    public enum BombType
    {
        Row,
        Column,
        RowCol,
        Adjacent
    };


    public enum TileColor
    {
        Red,
        Green,
        Pink,
        Aqua,
        Blue,
        Yellow,
        Purple
    };



    public enum GoalType
    {
        MoveBased,
        TimeBased
    };


    //The global game states
    public enum StateType
    {
        Init = 0,
        Menu,
        GamePlay,
        GameOver,
        Pause
    };

}
