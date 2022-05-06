using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class EnemyDebugger : MonoBehaviour
{
    
	[SerializeField] Enemy enemyToDebug;
	Health health;
	TextMeshProUGUI healthText;
    
	protected void Start()
	{
		healthText = GetComponent<TextMeshProUGUI>();
		health = enemyToDebug.GetComponent<Health>();
	}
    
	protected void Update()
	{
		healthText.text = health.GetCurrentHealth().ToString();
		healthText.text += "\n" + enemyToDebug.GetStatsSet().enemyStats[EnemySize.MEDIUM].maxHealth.ToString();
		healthText.text += "\n" + enemyToDebug.GetStatsSet().enemyStats[EnemySize.LARGE].maxHealth.ToString();
		
		
	}
    
}
