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
	
	bool canPauseGame = true;
	public bool CanPauseGame(){return canPauseGame;}
	public void SetCanPauseGame(bool nCanPauseGame){canPauseGame = nCanPauseGame;}
	
	bool isGamePaused = false;
	public bool IsGamePaused(){return isGamePaused;}
	
	[SerializeField] float finishedGameTime;
	bool canReloadLevel = false;
	
	
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
	    PauseBehavior();
	    RestartGameBehavior();
	    GameOverBehavior();
    }
    
	void LevelTimeBehavior()
	{
		currentLevelTime += Time.deltaTime;
		currentLevelTime = Mathf.Clamp(currentLevelTime, 0f, levelTime);
		if(currentLevelTime >= levelTime && !isTimeFinished)
		{
			isTimeFinished = true;
			SetGameLost();
		}

		GameUIManager.instance.SetStageTime(currentLevelTime, levelTime);
		
	}
	
	void PauseBehavior()
	{
		if(gameState == GameState.GAME)
		{
			if((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start")) && canPauseGame)
			{
				if(isGamePaused)
				{
					UnpauseGame();
				}
				else
				{
					PauseGame();
				}
			}
		}

	}
	
	void RestartGameBehavior()
	{
		if(Input.GetKeyDown(KeyCode.R))
		{
			if(!isGamePaused)
			{
				canReloadLevel = true;
				ReloadLevel();
			}
		}
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
		isGamePaused = true;
		GameUIManager.instance.SetPausePanel(true);
	}
	public void UnpauseGame()
	{
		Time.timeScale = 1f;
		isGamePaused = false;
		GameUIManager.instance.SetPausePanel(false);
	}
	
	public void SetGameLost()
	{
		gameState = GameState.GAME_OVER;
		SetGameOverScreen(true);
		StartCoroutine(FinishedGameCoroutine());
		PauseGame();
	}
	public void SetGameWon()
	{
		gameState = GameState.GAME_WON;
		SetGameWonScreen(true);
		StartCoroutine(FinishedGameCoroutine());
		PauseGame();
	}
	
	IEnumerator FinishedGameCoroutine()
	{
		yield return new WaitForSecondsRealtime(finishedGameTime);
		canReloadLevel = true;
	}
	
	
	public void SetGameState(GameState newGameState)
	{
		gameState = newGameState;
	}
	
	public void ReloadLevel()
	{
		if(canReloadLevel)
		{
			SceneManager.LoadScene(0);
		}
		
	}
	
	
	
}

public enum GameState {GAME = 0, GAME_OVER = 1, GAME_WON = 2}

