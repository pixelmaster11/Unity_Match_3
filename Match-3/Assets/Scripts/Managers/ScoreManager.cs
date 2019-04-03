using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for handiling score and win / lose condition
/// </summary>
public class ScoreManager : Manager
{

    #region VARIABLE AND REFERENCES

    [SerializeField]
    private UIManager m_uimanager;

    [SerializeField]
    private TileManager m_tileManager;
   

    public Enums.GoalType goalType;

    private int m_currentMoves = -1;
    public int totalMoves;
    public int totalTimeMins;
    public int totalTimeSecs;
    private int m_timer = 0;

    private int m_currentTiles = -1;
    public int totalTiles;
    public int goalTileCode;

    private int m_prevGoalTileCode;

    #endregion // VARIABLE AND REFERENCES

    //Sets time or moves and checks for win condtions
    #region TIMER / MOVES / WIN CONDITION
    /// <summary>
    /// This wil set the game's goal type to be timer based or based on number of moves
    /// </summary>
    private void SetGoal()
    {
        switch(goalType)
        {
            case Enums.GoalType.TimeBased:
              
                m_uimanager.EnableTimerUIUI(true);
                m_uimanager.EnableMovesUI(false);
                StartTimer();
                break;

            case Enums.GoalType.MoveBased:
                m_uimanager.EnableTimerUIUI(false);
                m_uimanager.EnableMovesUI(true);
                UpdateMoves();
                break;

        }


        
        m_uimanager.SetGoalTileImage(goalTileCode);
        m_uimanager.SetTotalTilesText(totalTiles);
        UpdateTilesCleared();
    }
    
    /// <summary>
    /// Starts the timer if timer based goal
    /// </summary>
    private void StartTimer()
    {
        StartCoroutine(TimerRoutine());
    }

    /// <summary>
    /// Timer coroutine for time count
    /// </summary>
    /// <returns></returns>
    private IEnumerator TimerRoutine()
    {
        m_timer = 0;

        while (m_timer < (totalTimeMins * 60 + totalTimeSecs))
        {

          
           m_timer++;
           m_uimanager.UpdateTimerText((totalTimeMins * 60 + totalTimeSecs) - m_timer, (totalTimeMins * 60 + totalTimeSecs));


            yield return new WaitForSeconds(1);
      
           
        }

        CheckWin();

        //Game over
    }


    /// <summary>
    /// This will be called whenever a player successfully completes a move / swap
    /// </summary>
    public void UpdateMoves()
    {
        if(goalType == Enums.GoalType.MoveBased)
        {
            m_currentMoves++;

            if(m_currentMoves >= totalMoves)
            {
                //Game over
                CheckWin();
            }

            m_uimanager.UpdateMovesText(totalMoves - m_currentMoves);
        }
    }

    /// <summary>
    /// This will be called whenever a sequence of match of goal tiles is cleared
    /// </summary>
    public void UpdateTilesCleared()
    {
        m_currentTiles++;     
        m_uimanager.UpdateTilesClearedText(m_currentTiles);

        //If goal reached then check for win condition
        if (m_currentTiles >= totalTiles)
        {
            CheckWin();
        }

    }

    /// <summary>
    /// This functions checks for winning / losing conditon
    /// </summary>
    public void CheckWin()
    {
        if (m_currentTiles >= totalTiles)
        {
            m_tileManager.Won(true);
            m_uimanager.WinLose(true);
        }

        else
        {
            m_tileManager.Won(false);
            m_uimanager.WinLose(false);
        }    

    }

    #endregion // TIMER / MOVES / WIN CONDITON

    //All functionality related to play or reload or load new level
    #region / PLAY / RESTART / LOAD NEW LEVEL

    /// <summary>
    /// Called when level is restarted
    /// </summary>
    public void Restart()
    {
        OnReset();
        goalTileCode = m_prevGoalTileCode;
    }

    /// <summary>
    /// Called when a new level is loaded
    /// </summary>
    public void NewLevel()
    {
        goalTileCode = Random.Range(1, Constants.MAX_TILE_CODES + 1);
        m_prevGoalTileCode = goalTileCode;
        OnReset();
       
    }

    /// <summary>
    /// Resets all game counters
    /// </summary>
    public void OnReset()
    {
        m_currentMoves = -1;
        m_currentTiles = -1;
        m_timer = 0;
        StopAllCoroutines();
        SetGoal();
    }


    /// <summary>
    /// This is called on game start
    /// </summary>
    public void OnPlay()
    {
        NewLevel();
    }

    public override void ManagedUpdate()
    {
       
    }

    #endregion // / PLAY / RESTART / LOAD NEW LEVEL



}
