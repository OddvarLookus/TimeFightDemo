using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySystemManager : MonoBehaviour
{
	int currentEnemies = 0;
	public int GetEnemiesNum(){return currentEnemies;}
	
	int prevEnemies = 0;
	[SerializeField] int maxEnemiesToNotify;
	bool fewEnemiesRemain = false;
	public bool GetFewEnemiesRemain(){return fewEnemiesRemain;}
	
	[SerializeField] UnityEvent onAllEnemiesKilled;
	bool allEnemiesKilledCalled = false;
	
	[SerializeField] AudioSource enemiesToNotifySound;
	bool enemiesToNotifySoundPlayed = false;
	
	public static EnemySystemManager instance;
	
	protected void Awake()
	{
		StartCoroutine(RefreshEnemiesCoroutine());
		
		instance = this;
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
			fewEnemiesRemain = true;
			
			GameUIManager.instance.SetEnemiesRemaining(currentEnemies);
		}
		else
		{
			fewEnemiesRemain = false;
			
			GameUIManager.instance.SetEnemiesRemaining(-1);
		}
		
		prevEnemies = currentEnemies;
	}
	
	public void OnLevelStart()
	{
		allEnemiesKilledCalled = false;
	}
	
	protected void OnDisable()
	{
		
	}
	
	
	public Transform GetNearestEnemy(Vector3 centerPos)
	{
		float minDist = float.MaxValue;
		int minIdx = int.MaxValue;
		for(int i = 0; i < transform.childCount; i++)
		{
			float dist = (transform.GetChild(i).position - centerPos).magnitude;
			if(dist < minDist)
			{
				minDist = dist;
				minIdx = i;
			}
		}
		return transform.GetChild(minIdx);
	}
}
