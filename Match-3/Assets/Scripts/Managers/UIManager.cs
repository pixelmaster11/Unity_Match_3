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
    public Image timerFillImage;


    public Image musicOn;
    public Image musicOff;

    public Image sfxOn;
    public Image sfxOff;


    public GameObject pausePanel;

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

    
    

    public void UpdateTimerText(int timer, int totalTime)
    {

        timerFillImage.fillAmount = (float)((float)timer / (float)totalTime);
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

        else
        {
            goalTileImage.color = Color.white;
        }
    }


    public void SetMusic(bool on)
    {
        musicOn.enabled = on;
        musicOff.enabled = !on;
      
    }

    public void SetSFX(bool on)
    {
        sfxOn.enabled = on;
        sfxOff.enabled = !on;
    }


    public void Pause(bool pause)
    {
        pausePanel.SetActive(pause);   
    }
}
