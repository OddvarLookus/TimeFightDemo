using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;


[CreateAssetMenu(menuName = "ENEMY STATS ASSET")]
public class EnemyStatsSet : SerializedScriptableObject
{
	[SerializeField] public Dictionary<EnemySize, EnemyStatsScaler> enemyStats = new Dictionary<EnemySize, EnemyStatsScaler>();
    
}

public enum EnemySize {MEDIUM = 0, LARGE = 1}


[System.Serializable]
public class EnemyStatsScaler
{
	public float scale;
	public float maxHealth;
	
	public int difficultyValue;
}