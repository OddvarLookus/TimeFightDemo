using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
	[SerializeField] float levelTime;
	float currentLevelTime = 0f;
	
	
	protected void Awake()
	{
		
	}
	
    void Update()
    {
	    LevelTimeBehavior();
    }
    
	void LevelTimeBehavior()
	{
		currentLevelTime += Time.deltaTime;
		currentLevelTime = Mathf.Clamp(currentLevelTime, 0f, levelTime);

		GameUIManager.instance.SetStageTime(currentLevelTime, levelTime);
		
	}
}
