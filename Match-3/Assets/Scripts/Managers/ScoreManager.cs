using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Manager
{
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



    private void Start()
    {
        SetGoal();
    }


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

        goalTileCode = Random.Range(1, Constants.MAX_TILE_CODES + 1);
        m_uimanager.SetGoalTileImage(goalTileCode);
        m_uimanager.SetTotalTilesText(totalTiles);
        UpdateTilesCleared();
    }
    

    private void StartTimer()
    {
        StartCoroutine(TimerRoutine());
    }


    private IEnumerator TimerRoutine()
    {
        m_timer = 0;

        while (m_timer < (totalTimeMins * 60 + totalTimeSecs))
        {

          
           m_timer++;
           m_uimanager.UpdateTimerText((totalTimeMins * 60 + totalTimeSecs) - m_timer);


            yield return new WaitForSeconds(1);
      
           
        }

        CheckWin();

        //Game over
    }


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


    public void UpdateTilesCleared()
    {
        m_currentTiles++;     
        m_uimanager.UpdateTilesClearedText(m_currentTiles);

        if (m_currentTiles >= totalTiles)
        {
            CheckWin();
        }

    }


    public void CheckWin()
    {
        if (m_currentTiles >= totalTiles)
        {
            m_tileManager.Won(true);
        }

        else
        {
            m_tileManager.Won(false);
        }

      

    }


    public override void ManagedUpdate()
    {
       
    }


   
    

}
