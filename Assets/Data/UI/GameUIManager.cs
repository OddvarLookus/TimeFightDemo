using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SetCreditsLabel(0);//start credits label at 0
            timeBarImage = timeBar.GetComponent<Image>();
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

	//CREDITS
    [SerializeField] TextMeshProUGUI creditsLabel;
    public void SetCreditsLabel(int _credits)
    {
        creditsLabel.text = _credits.ToString();
    }
    
	//TIME LABEL AND BAR
	[SerializeField] TextMeshProUGUI timeLabel;
	[SerializeField] RectTransform timeBar;
    Image timeBarImage;
    [SerializeField] Color timeBarFullColor;
    [SerializeField] Color timeBarEmptyColor;
	public void SetStageTime(float _currentTime, float _stageTime)
	{
		float timeFactor = 1f - _currentTime/_stageTime;
        timeFactor = Mathf.Clamp(timeFactor, 0f, 1f);
        timeBar.localScale = new Vector3(timeFactor, 1f, 1f);
        timeBarImage.color = Color.Lerp(timeBarEmptyColor, timeBarFullColor, timeFactor);

        float timeSeconds = _stageTime - _currentTime;
        int secondsInt = Mathf.FloorToInt(timeSeconds);
        int minutesInt = secondsInt / 60;
        secondsInt -= minutesInt * 60;
        
        string secondsStr = secondsInt.ToString();
        if(secondsInt < 10)
        {
            secondsStr = $"0{secondsStr}";
        }
        
        timeLabel.text = $"{minutesInt} : {secondsStr}";

	}
	
	//GAME OVER
	[SerializeField] RectTransform gameOverPanel;
	public void SetGameOverScreen(bool _active)
	{
		gameOverPanel.gameObject.SetActive(_active);
	}
	
	//GAME WON
	[SerializeField] RectTransform gameWonPanel;
	public void SetGameWonScreen(bool _active)
	{
		gameWonPanel.gameObject.SetActive(_active);
	}
	
	//GAME START
	[SerializeField] Animator gameStartPanelAnimator;
	public void StartUISequence()
	{
		gameStartPanelAnimator.Play("GameStartUIEnter", -1, 0f);
	}
	
	//PAUSE
	[SerializeField] RectTransform pausePanel;
	public void SetPausePanel(bool active)
	{
		pausePanel.gameObject.SetActive(active);
	}
	
	
}
