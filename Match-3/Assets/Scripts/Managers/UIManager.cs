using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text movesText;
    public Text scoreText;
    public Text timerText;
    public Text totalTilesText;
    public Text tilesClearedText;


    public GameObject[] moveUI;
    public GameObject[] timerUI;

    public Sprite[] goalTileSprites;

    public Image goalTileImage;

    
    public void EnableMovesUI(bool enable)
    {
        for(int i = 0; i < moveUI.Length; i++)
        {
            moveUI[i].gameObject.SetActive(enable);
        }
    }


    public void EnableTimerUIUI(bool enable)
    {
        for (int i = 0; i < timerUI.Length; i++)
        {
            timerUI[i].gameObject.SetActive(enable);
        }
    }



    public void UpdateScoreText(int value)
    {
        scoreText.text = value.ToString();
    }


    public void UpdateMovesText(int value)
    {
        movesText.text = value.ToString();
    }

    
    

    public void UpdateTimerText(int timer)
    {
     

        timerText.text = timer.ToString();
    }


    public void UpdateTilesClearedText(int value)
    {
        tilesClearedText.text = value.ToString();
    }


    public void SetTotalTilesText(int value)
    {
        totalTilesText.text = value.ToString();
    }

    public void SetGoalTileImage(int tileCode)
    {
        goalTileImage.sprite = goalTileSprites[tileCode - 1];

        if(tileCode - 1 == 3)
        {
            goalTileImage.color = new Color(143, 0, 255);
        }

        else if(tileCode - 1 == 4)
        {
            goalTileImage.color = new Color(0, 255, 252);
        }
    }
}
