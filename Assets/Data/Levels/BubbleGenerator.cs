using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BubbleGenerator : MonoBehaviour
{
	//EDITOR
	[TitleGroup("Bubble Size")] [MinValue(0f)] [SerializeField] float radius;
	[TitleGroup("Bubble Size")] [MinValue(0f)] [SerializeField] float freeAreaRadius;
	
	[TitleGroup("References")] [SerializeField] [SceneObjectsOnly] Transform enemiesParent;
	[TitleGroup("References")] [SerializeField] [SceneObjectsOnly] Transform asteroidsParent;
	[TitleGroup("References")] [SerializeField] [SceneObjectsOnly] Transform interactablesParent;
	
	
	[TitleGroup("Generation/Asteroids")] [SerializeField] int numberOfAsteroids;
	[TitleGroup("Generation/Asteroids")] [SerializeField] [AssetsOnly] GameObject[] asteroidsPrefabs;
	
	[TitleGroup("Generation/Enemies")] [SerializeField] int numberOfEnemies;
	[TitleGroup("Generation/Enemies")] [SerializeField] [AssetsOnly] GameObject[] enemiesPrefabs;
	
	[TitleGroup("Generation")]
	[Button("GENERATE LEVEL")]
	public void GenerateLevelEditor()
	{
		GenerateLevel();
	}
	[TitleGroup("Generation")]
	[Button("PURGE LEVEL")]
	public void PurgeLevelEditor()
	{
		PurgeLevel();
	}
	
	
	//MONOBEHAVIOR
	
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    
	public void GenerateLevel()
	{
		//INSTANTIATE ASTEROIDS
		for(int i = 0; i < numberOfAsteroids; i++)
		{
			int rIndex = Random.Range(0, asteroidsPrefabs.Length);
			GameObject nAsteroid = Instantiate(asteroidsPrefabs[rIndex]);
			nAsteroid.transform.SetParent(asteroidsParent, false);
			nAsteroid.transform.position = GetRandomPointInSphere();
		}
		
		//INSTANTIATE ENEMIES
		for(int i = 0; i < numberOfEnemies; i++)
		{
			int rIndex = Random.Range(0, enemiesPrefabs.Length);
			GameObject nEnemy = Instantiate(enemiesPrefabs[rIndex]);
			nEnemy.transform.SetParent(enemiesParent, false);
			nEnemy.transform.position = GetRandomPointInSphere();
		}
		
	}
	
	public Vector3 GetRandomPointInSphere()
	{
		Vector3 nPoint = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
		nPoint *= Random.Range(freeAreaRadius, radius);
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
			//PURGE ENEMIES
			for(int i = 0; i < enemiesParent.childCount; i++)
			{
				DestroyImmediate(enemiesParent.GetChild(i).gameObject);
			}
			//PURGE ASTEROIDS
			for(int i = 0; i < asteroidsParent.childCount; i++)
			{
				DestroyImmediate(asteroidsParent.GetChild(i).gameObject);
			}
			//PURGE INTERACTABLES
			for(int i = 0; i < interactablesParent.childCount; i++)
			{
				DestroyImmediate(interactablesParent.GetChild(i).gameObject);
			}
		}

	}
    
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(Vector3.zero, radius);
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(Vector3.zero, freeAreaRadius);
	}
	
}
