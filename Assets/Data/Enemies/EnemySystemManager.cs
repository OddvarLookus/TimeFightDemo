using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySystemManager : MonoBehaviour
{
	int currentEnemies = 0;
	int prevEnemies = 0;
	[SerializeField] int maxEnemiesToNotify;
	
	[SerializeField] UnityEvent onAllEnemiesKilled;
	bool allEnemiesKilledCalled = false;
	
	[SerializeField] AudioSource enemiesToNotifySound;
	bool enemiesToNotifySoundPlayed = false;
	
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
		
		if(currentEnemies <= maxEnemiesToNotify)
		{
			if(enemiesToNotifySoundPlayed == false)
			{
				enemiesToNotifySound.Play();
				enemiesToNotifySoundPlayed = true;
			}
			
			GameUIManager.instance.SetEnemiesRemaining(currentEnemies);
		}
		else
		{
			GameUIManager.instance.SetEnemiesRemaining(-1);
		}
		
		prevEnemies = currentEnemies;
	}
}
