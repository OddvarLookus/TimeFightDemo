using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "LEVEL")]
public class Level : SerializedScriptableObject
{
	public int difficultyValue;
    
	[OnInspectorGUI("RecalculateProbabilities")]public List<EnemyRoller> enemies = new List<EnemyRoller>();
	void RecalculateProbabilities()
	{
		Debug.Log("VALUE CHANGED");
		//ITERATE THE ENEMIES
		float currentEnemyRollerProb = 0f;
		for(int i = 0; i < enemies.Count; i++)
		{
			float nEnemyProb = currentEnemyRollerProb + enemies[i].probability;
			if(nEnemyProb > 1f)
			{
				enemies[i].probability = 1f - currentEnemyRollerProb;
				currentEnemyRollerProb = 1f;
			}
			else
			{
				currentEnemyRollerProb = nEnemyProb;
			}
			
			
			//ITERATE THE SIZE ROLLERS
			float currentSizeRollerProb = 0f;
			for(int e = 0; e < enemies[i].sizesRollers.Count; e++)
			{
				float nProb = currentSizeRollerProb + enemies[i].sizesRollers[e].probability;
				if(nProb > 1f)
				{
					enemies[i].sizesRollers[e].probability = 1f - currentSizeRollerProb;
					currentSizeRollerProb = 1f;
				}
				else
				{
					currentSizeRollerProb = nProb;
				}
			}
		}
	}
	
	
    
}

public class EnemySizeRoller
{
	public EnemySize size;
	[Range(0f, 1f)] public float probability;
}

public class EnemyRoller
{
	[OnValueChanged("GetEnemyFromPrefab")] public GameObject enemyPrefab;
	[HideInInspector] public Enemy enemy;
	void GetEnemyFromPrefab()
	{
		if(enemyPrefab != null)
		{
			enemy = enemyPrefab.GetComponent<Enemy>();
		}
	}
	
	[Range(0f, 1f)] public float probability;
	public List<EnemySizeRoller> sizesRollers;
	
	
}
