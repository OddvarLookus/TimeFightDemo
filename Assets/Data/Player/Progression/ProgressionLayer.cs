using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class ProgressionLayer : SerializedMonoBehaviour
{
	[SerializeField] List<Upgrade> allUpgrades = new List<Upgrade>();
    
    
}

[System.Serializable]
public class Upgrade
{
	public string upgradeName;
	public Sprite upgradeSprite;
	public StatsUpgrade statsUpgrade;
}

[System.Serializable]
public class StatsUpgrade
{
	[Header("Stats")]
	public float damageBonus;
	public float attackTimeBonus;
	public float luckBonus;
}

