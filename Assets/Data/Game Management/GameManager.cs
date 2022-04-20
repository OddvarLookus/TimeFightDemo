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
	
	public static GameManager instance;
	
	protected void Start()
	{
		instance = this;
		UnpauseGame();
		SetGameOverScreen(false);
		SetGameWonScreen(false);
		GameUIManager.instance.StartUISequence();
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
		if(gameState == GameState.GAME_WON)
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
	
	public void SetGameWonScreen(bool _active)
	{
		GameUIManager.instance.SetGameWonScreen(_active);
	}
	
	public void PauseGame()
	{
		Time.timeScale = 0f;
	}
	public void UnpauseGame()
	{
		Time.timeScale = 1f;
	}
	
	public void SetGameLost()
	{
		gameState = GameState.GAME_OVER;
		SetGameOverScreen(true);
	}
	public void SetGameWon()
	{
		gameState = GameState.GAME_WON;
		SetGameWonScreen(true);
	}
	
	public void SetGameState(GameState newGameState)
	{
		gameState = newGameState;
	}
	
	public void ReloadLevel()
	{
		SceneManager.LoadScene(0);
	}
	
	
	
}

public enum GameState {GAME = 0, GAME_OVER = 1, GAME_WON = 2}

