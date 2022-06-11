using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BubbleGenerator : MonoBehaviour
{
	//EDITOR
	[TitleGroup("Bubble Size")] [MinValue(0f)] [SerializeField] float radiusStep;
	[TitleGroup("Bubble Size")] [MinValue(0f)] [SerializeField] float radius;
	[TitleGroup("Bubble Size")] [MinValue(0f)] [SerializeField] float freeAreaRadius;
	
	[TitleGroup("References")] [SerializeField] [SceneObjectsOnly] Transform enemiesParent;
	[TitleGroup("References")] [SerializeField] [SceneObjectsOnly] Transform asteroidsParent;
	[TitleGroup("References")] [SerializeField] [SceneObjectsOnly] Transform interactablesParent;
	
	
	[TitleGroup("Generation")] [SerializeField] [AssetsOnly] LevelsProgression levelsProgression;
	public int GetNumberOfLevels(){return levelsProgression.runLevels.Count;}
	Level currentLevel;
	
	[TitleGroup("Generation")]
	[Button("GENERATE LEVEL")]
	public void GenerateLevelEditor()
	{
		GenerateLevel(0);
	}
	[TitleGroup("Generation")]
	[Button("PURGE LEVEL")]
	public void PurgeLevelEditor()
	{
		PurgeLevel();
	}
	
	[TitleGroup("Generation")] [SerializeField] Transform worldLimitTr;
	//MONOBEHAVIOR
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		if(worldLimitTr == null)
		{
			worldLimitTr = GameObject.Find("world limit").transform;
		}
	}
	
	protected void Start()
	{
		//GenerateLevel();
	}
    
	public void GenerateLevel(int idx)
	{
		currentLevel = levelsProgression.GetLevelAt(idx);
		//INSTANTIATE ASTEROIDS
		GenerateAsteroids();

		
		//INSTANTIATE ENEMIES
		GenerateMandatoryEnemies();
		GenerateEnemies();
		
		//for(int i = 0; i < numberOfEnemies; i++)
		//{
		//	int rIndex = Random.Range(0, enemiesPrefabs.Length);
		//	GameObject nEnemy = Instantiate(enemiesPrefabs[rIndex]);
		//	nEnemy.transform.SetParent(enemiesParent, false);
		//	nEnemy.transform.position = GetRandomPointInSphere();
		//}
		
		//WORLD LIMIT
		worldLimitTr.localScale = new Vector3(radius, radius, radius);
		
	}
	#region ASTEROIDS_GENERATION
	void GenerateAsteroids()
	{
		for(int i = 0; i < currentLevel.asteroidsCount; i++)
		{
			float randNum = Random.Range(0f, 1f);
			float currentProb = 0f;
			for(int e = 0; e < currentLevel.asteroids.Count; e++)
			{
				currentProb += currentLevel.asteroids[e].probability;
				if(randNum <= currentProb)
				{
					GameObject nAsteroid = Instantiate(currentLevel.asteroids[e].asteroidPrefab);
					nAsteroid.transform.SetParent(asteroidsParent, false);
					nAsteroid.transform.position = GetRandomPointInSphere();
					break;
				}
			}
			
		}
	}
	
	#endregion
	#region ENEMY_GENERATION
	
	int currentDifficulty;
	Level thisLevel;
	void GenerateMandatoryEnemies()
	{
		currentDifficulty = currentLevel.difficultyValue;
		thisLevel = Instantiate(currentLevel);
		for(int i = 0; i < thisLevel.mandatoryEnemies.Count; i++)
		{
			GameObject nEnemy = Instantiate(thisLevel.mandatoryEnemies[i].enemyPrefab);
			Transform nEnemyTr = nEnemy.transform;
			nEnemyTr.SetParent(enemiesParent);
			nEnemyTr.position = GetRandomPointInSphere();
			Enemy nEnemyEnemy = nEnemy.GetComponent<Enemy>();
			EnemySize desiredSize = thisLevel.mandatoryEnemies[i].size;
			nEnemyEnemy.SetEnemySize(desiredSize);
			
			int diffToLower = nEnemyEnemy.GetStatsSet().enemyStats[desiredSize].difficultyValue;
			currentDifficulty -= diffToLower;
		}
		
	}
	
	void GenerateEnemies()
	{
		currentDifficulty = currentLevel.difficultyValue;
		thisLevel = Instantiate(currentLevel);
		List<EnemyRoller> enemiesGenList = thisLevel.enemies;
		
		while(currentDifficulty > 0)
		{
			float randEnemy = Random.Range(0f, 1f);
			float randVariant = Random.Range(0f, 1f);
			int enemyIdx = 0;
			int variantIdx = 0;
			
			float totProb = 0f;
			for(int i = 0; i < enemiesGenList.Count; i++)
			{
				totProb += enemiesGenList[i].probability;
				if(randEnemy <= totProb)
				{
					enemyIdx = i;
					break;
				}
			}
			
			float totVariantProb = 0f;
			for(int i = 0; i < enemiesGenList[enemyIdx].sizesRollers.Count; i++)
			{
				totVariantProb += enemiesGenList[enemyIdx].sizesRollers[i].probability;
				if(randVariant <= totVariantProb)
				{
					variantIdx = i;
					break;
				}
			}
			
			//SPAWN THE ENTITY
			GameObject nEnemy = Instantiate(enemiesGenList[enemyIdx].enemyPrefab);
			Transform nEnemyTr = nEnemy.transform;
			nEnemyTr.SetParent(enemiesParent);
			nEnemyTr.position = GetRandomPointInSphere();
			Enemy nEnemyEnemy = nEnemy.GetComponent<Enemy>();
			EnemySize desiredSize = enemiesGenList[enemyIdx].sizesRollers[variantIdx].size;
			nEnemyEnemy.SetEnemySize(desiredSize);
			
			//Debug.Log($"SPAWNED: {nEnemy.name}");
			//Debug.Log($"DIFFICULTY: {currentDifficulty} -> {currentDifficulty - nEnemyEnemy.GetStatsSet().enemyStats[desiredSize].difficultyValue}");
			
			int diffToLower = nEnemyEnemy.GetStatsSet().enemyStats[desiredSize].difficultyValue;
			
			//AFTER SPAWNING THE ENTITY, CHECK PROBABILITIES
			currentDifficulty -= diffToLower;
			enemiesGenList = RemoveTooDifficult(enemiesGenList, currentDifficulty);
		}
	}
	
	List<EnemyRoller> RemoveTooDifficult(List<EnemyRoller> rollers, int diff)
	{
		List<EnemyRoller> nRollers = rollers;
		for(int i = nRollers.Count - 1; i >= 0; i--)
		{
			for(int e = nRollers[i].sizesRollers.Count - 1; e >= 0; e--)
			{
				int enemyDiff = nRollers[i].enemy.GetStatsSet().enemyStats[nRollers[i].sizesRollers[e].size].difficultyValue;
				if(diff < enemyDiff)
				{
					nRollers[i].sizesRollers.RemoveAt(e);
					
				}
			}
			
			if(nRollers[i].sizesRollers.Count <= 0)
			{
				nRollers.RemoveAt(i);
				
			}
		}
		nRollers = InflateProbabilities(nRollers);
		return nRollers;
	}
	
	List<EnemyRoller> InflateProbabilities(List<EnemyRoller> rollers)
	{
		List<EnemyRoller> nRollers = rollers;
		//ENEMY ROLLERS
		float totalEnemyRollersProb = 0f;
		int enemiesRollersCount = nRollers.Count;
		if(enemiesRollersCount <= 0)
		{
			return new List<EnemyRoller>();
		}
		
		for(int i = nRollers.Count - 1; i >= 0; i--)
		{
			
			totalEnemyRollersProb += nRollers[i].probability;
			
			//SIZES ROLLERS.
			float totalSizesRollersProb = 0f;
			int rollersCount = nRollers[i].sizesRollers.Count;
			if(rollersCount == 0)
			{
				continue;
			}
			
			for(int e = nRollers[i].sizesRollers.Count - 1; e >= 0; e--)
			{
				totalSizesRollersProb += nRollers[i].sizesRollers[e].probability;
			}
			float remainingProbability = 1f - totalSizesRollersProb;
			float probabilityToAdd = remainingProbability / (float)rollersCount;
			for(int e = nRollers[i].sizesRollers.Count - 1; e >= 0; e--)
			{
				nRollers[i].sizesRollers[e].probability += probabilityToAdd;
			}
		}
		
		float remainingEnemiesProbability = 1f - totalEnemyRollersProb;
		float enemiesProbabilityToAdd = remainingEnemiesProbability / (float)enemiesRollersCount;
		//ADD REMAINING PROBABILITIES TO THE ENEMY ROLLERS
		for(int i = 0; i < nRollers.Count; i++)
		{
			nRollers[i].probability += enemiesProbabilityToAdd;
		}
		
		return nRollers;
	}
	
	#endregion
	
	
	
	public Vector3 GetRandomPointInSphere()
	{
		Vector3 nPoint = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
		nPoint *= Random.Range(freeAreaRadius, radius - radiusStep);
		return nPoint;
	}
    
	public void PurgeLevel()
	{
		if(Application.isPlaying)
		{
			//PURGE ENEMIES
			for(int i = 0; i < enemiesParent.childCount; i++)
			{
				Destroy(enemiesParent.GetChild(i).gameObject);
			}
			//PURGE ASTEROIDS
			for(int i = 0; i < asteroidsParent.childCount; i++)
			{
				Destroy(asteroidsParent.GetChild(i).gameObject);
			}
			//PURGE INTERACTABLES
			for(int i = 0; i < interactablesParent.childCount; i++)
			{
				Destroy(interactablesParent.GetChild(i).gameObject);
			}
		}
		else if(!Application.isPlaying)
		{
			int count = enemiesParent.childCount;
			//PURGE ENEMIES
			for(int i = 0; i < count; i++)
			{
				DestroyImmediate(enemiesParent.GetChild(0).gameObject);
			}
			count = asteroidsParent.childCount;
			//PURGE ASTEROIDS
			for(int i = 0; i < count; i++)
			{
				DestroyImmediate(asteroidsParent.GetChild(0).gameObject);
			}
			count = interactablesParent.childCount;
			//PURGE INTERACTABLES
			for(int i = 0; i < count; i++)
			{
				DestroyImmediate(interactablesParent.GetChild(0).gameObject);
			}
		}

	}
    
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(Vector3.zero, radius);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(Vector3.zero, radius - radiusStep);
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(Vector3.zero, freeAreaRadius);
	}
	
}
