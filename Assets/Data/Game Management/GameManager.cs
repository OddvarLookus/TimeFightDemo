using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	GameState gameState;
	
	[SerializeField] float levelTime;
	float currentLevelTime = 0f;
	[SerializeField] UnityEvent onLevelTimeEnd;
	bool isTimeFinished = false;
	
	protected void OnEnable()
	{
		UnpauseGame();
		SetGameOverScreen(false);
		gameState = GameState.GAME;
	}
	
    void Update()
    {
	    LevelTimeBehavior();
	    
	    GameOverBehavior();
    }
    
	void LevelTimeBehavior()
	{
		currentLevelTime += Time.deltaTime;
		currentLevelTime = Mathf.Clamp(currentLevelTime, 0f, levelTime);
		if(currentLevelTime >= levelTime && !isTimeFinished)
		{
			isTimeFinished = true;
			gameState = GameState.GAME_OVER;
			onLevelTimeEnd?.Invoke();
		}

		GameUIManager.instance.SetStageTime(currentLevelTime, levelTime);
		
	}
	
	void GameOverBehavior()
	{
		if(gameState == GameState.GAME_OVER)
		{
			if(Input.anyKey)
			{
				ReloadLevel();
			}
		}
	}
	
	
	public void SetGameOverScreen(bool _active)
	{
		GameUIManager.instance.SetGameOverScreen(_active);
	}
	
	public void PauseGame()
	{
		Time.timeScale = 0f;
	}
	public void UnpauseGame()
	{
		Time.timeScale = 1f;
	}
	
	public void ReloadLevel()
	{
		SceneManager.LoadScene(0);
	}
	
}

public enum GameState {GAME = 0, GAME_OVER = 1}

