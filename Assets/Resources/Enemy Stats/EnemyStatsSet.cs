using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;


[CreateAssetMenu(menuName = "ENEMY STATS ASSET")]
[System.Serializable]
public class EnemyStatsSet : SerializedScriptableObject
{
	public Dictionary<EnemySize, EnemyStatsScaler> enemyStats = new Dictionary<EnemySize, EnemyStatsScaler>();
    
}

[System.Serializable]
public enum EnemySize {MEDIUM = 0, LARGE = 1}

[System.Serializable]
public class EnemyStatsScaler
{
	public float scale;
	public float maxHealth;
	
	public int difficultyValue;
}