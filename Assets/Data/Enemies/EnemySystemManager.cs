using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySystemManager : MonoBehaviour
{
	int currentEnemies = 0;
	int prevEnemies = 0;
	
	
	[SerializeField] UnityEvent onAllEnemiesKilled;
	bool allEnemiesKilledCalled = false;
	
	
	protected void Awake()
	{
		StartCoroutine(RefreshEnemiesCoroutine());
	}
	
	IEnumerator RefreshEnemiesCoroutine()
	{
		yield return new WaitForSeconds(0.5f);
		RefreshEnemies();
		StartCoroutine(RefreshEnemiesCoroutine());
	}
    
	void RefreshEnemies()
	{
		currentEnemies = transform.childCount;
		if(currentEnemies == 0 && allEnemiesKilledCalled == false)
		{
			onAllEnemiesKilled?.Invoke();
			allEnemiesKilledCalled = true;
		}
		
		prevEnemies = currentEnemies;
	}
}
