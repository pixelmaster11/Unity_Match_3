using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for handling all UI references 
/// Displaying / Animating UI
/// </summary>
public class UIManager : MonoBehaviour
{
    // All UI references to be dragged and dropped in the editor
    #region VARIABLES AND UI REFERENCES

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

    public GameObject scorePanel;
    public GameObject pausePanel;
    public GameObject winLosePanel;

    public Text winLoseText;

    public Canvas menuCanvas;
    public Image[] menuImages;
    public Button[] menuButtons;
    public Text menuText;

    #endregion // VARIABLE AND UI REFERENCES


    /// <summary>
    /// This coroutine is responsible for fading menu screen into gameplay
    /// </summary>
    /// <param name="alpha"></param>
    /// <param name="delay"></param>
    /// <param name="enable"></param>
    /// <returns></returns>
    public IEnumerator PlayRoutine(float alpha, float delay, bool enable)
    {
        if(enable)
        {
            menuCanvas.gameObject.SetActive(enable);
            scorePanel.gameObject.SetActive(!enable);
        }
       
        for(int i = 0; i < menuImages.Length; i++)
        {
            menuImages[i].CrossFadeAlpha(alpha, delay, false);
        }

        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuButtons[i].interactable = enable;
        }

        menuText.CrossFadeAlpha(alpha, delay, false);

          
        yield return new WaitForSeconds(delay);

       
        if(!enable)
        {
            menuCanvas.gameObject.SetActive(enable);
            scorePanel.gameObject.SetActive(!enable);
        }
        
    }


    //All ui stuff related to score / timer and goal tiles
    #region SCORE / TIMER / GOAL UI
    /// <summary>
    /// This will enable / disable moves left UI
    /// </summary>
    /// <param name="enable"></param>
    public void EnableMovesUI(bool enable)
    {
        for(int i = 0; i < moveUI.Length; i++)
        {
            moveUI[i].gameObject.SetActive(enable);
        }
    }

    /// <summary>
    /// This will enable / disable time left UI
    /// </summary>
    /// <param name="enable"></param>
    public void EnableTimerUIUI(bool enable)
    {
        for (int i = 0; i < timerUI.Length; i++)
        {
            timerUI[i].gameObject.SetActive(enable);
        }
    }


    /// <summary>
    /// This will update and display the score value
    /// </summary>
    /// <param name="value"></param>
    public void UpdateScoreText(int value)
    {
        scoreText.text = value.ToString();
    }


    /// <summary>
    /// This will update and display moves left value
    /// </summary>
    /// <param name="value"></param>
    public void UpdateMovesText(int value)
    {
        movesText.text = value.ToString();
    }

    
    
    /// <summary>
    /// This will update and display time left value
    /// </summary>
    /// <param name="timer"></param>
    /// <param name="totalTime"></param>
    public void UpdateTimerText(int timer, int totalTime)
    {

        timerFillImage.fillAmount = (float)((float)timer / (float)totalTime);
        timerText.text = timer.ToString();
    }


    /// <summary>
    /// This will update and display how many goal tiles cleared value
    /// </summary>
    /// <param name="value"></param>
    public void UpdateTilesClearedText(int value)
    {          
        tilesClearedText.text = value.ToString();
    }


    /// <summary>
    /// This will set once, the total goal tiles required to clear
    /// </summary>
    /// <param name="value"></param>
    public void SetTotalTilesText(int value)
    {
        totalTilesText.text = value.ToString();
    }

    /// <summary>
    /// This will update and display proper goal tile image
    /// </summary>
    /// <param name="tileCode"></param>
    public void SetGoalTileImage(int tileCode)
    {
        goalTileImage.sprite = goalTileSprites[tileCode - 1];

        //NOTE: DUE TO LACK OF PROPER COLOR SPRITES / THIS IS USED TO SET COLOR ACCORDINGLY FOR 2 TILES
        //SOMETIMES DUE TO THE BASE COLOR BEING GREY, PURPLE COLOR MIGHT BE SHOWN AS PINKISH TINT, 
        //THERE MIGHT BE A BUG WITHING THE UNITY EDITOR
        if (tileCode - 1 == 3)
        {
            goalTileImage.color = new Color(120, 0, 255);
        }

        else if (tileCode - 1 == 4)
        {
            goalTileImage.color = new Color(0, 255, 252);
        }

        else
        {
            goalTileImage.color = new Color(255, 255, 255);
        }
        
    }

    #endregion // SCORE / TIMER / GOAL UI 

    //All ui stuff related to pause / music / sfx 
    #region MUSIC / PAUSE  / WIN UI
    /// <summary>
    /// This will turn on/off background music
    /// </summary>
    /// <param name="on"></param>
    public void SetMusic(bool on)
    {
        musicOn.enabled = on;
        musicOff.enabled = !on;
      
    }

    ////This will turn on / off game sfx
    public void SetSFX(bool on)
    {
        sfxOn.enabled = on;
        sfxOff.enabled = !on;
    }


    //This will display pause panel on pause button click
    public void Pause(bool pause)
    {
        pausePanel.SetActive(pause);   
    }

    /// <summary>
    /// This will display proper win / lose text on win / lose
    /// </summary>
    /// <param name="win"></param>
    public void WinLose(bool win)
    {
        winLosePanel.SetActive(true);

        if (win)
            winLoseText.text = "CONGRATULATIONS LEVEL CLEARED !!";
        else
            winLoseText.text = "YOU FAILED TO COMPLETE THE LEVEL OBJECTIVE";
    }

    /// <summary>
    /// Will deactive win / lose panel on new level load
    /// </summary>
    public void NewLevel()
    {
        if (winLosePanel.activeSelf)
        {
            winLosePanel.SetActive(false);
        }
    }

    #endregion // MUSIC / PAUSE / WIN UI
}
