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
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    [SerializeField] TextMeshProUGUI creditsLabel;
    public void SetCreditsLabel(int _credits)
    {
        creditsLabel.text = _credits.ToString();
    }
    
	[SerializeField] TextMeshProUGUI timeLabel;
	[SerializeField] RectTransform timeBar;
	public void SetStageTime(float _currentTime, float _stageTime)
	{
		float timeFactor = 1f - _currentTime/_stageTime;
        timeFactor = Mathf.Clamp(timeFactor, 0f, 1f);
        timeBar.localScale = new Vector3(timeFactor, 1f, 1f);

        float timeSeconds = _stageTime - _currentTime;
        int secondsInt = Mathf.FloorToInt(timeSeconds);
        int minutesInt = secondsInt / 60;
        secondsInt -= minutesInt * 60;
        timeLabel.text = $"{minutesInt} : {secondsInt}";
	}

}
