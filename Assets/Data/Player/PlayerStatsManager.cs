using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
	//REFERENCES
	Attack attack;
	PlayerController playerController;
	PlayerShield playerShield;
	protected void Awake()
	{
		attack = GetComponent<Attack>();
		playerController = GetComponent<PlayerController>();
		playerShield = GetComponent<PlayerShield>();
		RecalculateStats();
	}
	
	//ITEMS
	List<BuyableObjectInfos> items = new List<BuyableObjectInfos>();
	
	public void AddItem(BuyableObjectInfos newItem)
	{
		items.Add(newItem);
		
		RecalculateStats();
	}
	
	static List<Upgrade> upgrades = new List<Upgrade>();
	public void AddUpgrade(Upgrade nUpgrade)
	{
		upgrades.Add(nUpgrade);
		
		RecalculateStats();
	}
	
	public void RemoveAllUpgrades()
	{
		upgrades.Clear();
	}
	
	//HELD STATS
	static float baseLuck = 1f;
	public static float luck = 0f;
	
	public static float nukeKnucklesProbability = 0f;
	public static float nukeKnucklesDamage = 0f;
	
	
	//STATS CALCULATIONS
    
	public void RecalculateStats()
	{
		//RAW STATS
		float dmg = attack.baseDamage;
		
		float atkSpeed = attack.baseAttackSpeed;
		float playerSpeedBonus = 0f;
		
		float lck = baseLuck;
		
		//TECHNIQUES
		float bonusDmgAgainstAsteroids = 0f;
		float agilityMatrixBonus = 0f;
		
		nukeKnucklesProbability = 0f;
		nukeKnucklesDamage = 0f;
		
		
		for(int i = 0; i < upgrades.Count; i++)//get all upgrades
		{
			if(upgrades[i] is StatsUpgrade)//it's a stat
			{
				StatsUpgrade sUpgrade = upgrades[i] as StatsUpgrade;
				if(sUpgrade.damageBonus > 0f)
				{
					dmg = dmg + sUpgrade.damageBonus;
				}
				if(sUpgrade.attackSpeedBonus > 0f)
				{
					atkSpeed = atkSpeed + sUpgrade.attackSpeedBonus;
			
					playerSpeedBonus = playerSpeedBonus + (sUpgrade.attackSpeedBonus * 33.333f);
				}
				if(sUpgrade.luckBonus > 0f)
				{
					baseLuck = baseLuck + sUpgrade.luckBonus;
				}
			}
			if(upgrades[i] is TechniqueUpgrade)//it's a technique
			{
				if(upgrades[i] is DemolitionGlovesUpgrade)
				{
					DemolitionGlovesUpgrade dUpgrade = upgrades[i] as DemolitionGlovesUpgrade;
					bonusDmgAgainstAsteroids = bonusDmgAgainstAsteroids + dUpgrade.damageAgainstAsteroidsBonus;
				}
				if(upgrades[i] is AgilityMatrixUpgrade)
				{
					AgilityMatrixUpgrade agilityMatrixUpgrade = upgrades[i] as AgilityMatrixUpgrade;
					if(playerShield.GetCurrentShield() == 1)//no shield
					{
						
					}
					else if(playerShield.GetCurrentShield() > 1)//yes shield
					{
						atkSpeed = atkSpeed + agilityMatrixUpgrade.agilityBonus;
			
						playerSpeedBonus = playerSpeedBonus + (agilityMatrixUpgrade.agilityBonus * 33.333f);
					}
				}
				if(upgrades[i] is NukeKnucklesUpgrade)
				{
					NukeKnucklesUpgrade nukeKnucklesUpgrade = upgrades[i] as NukeKnucklesUpgrade;
					
					nukeKnucklesProbability += nukeKnucklesUpgrade.explosionProbability;
					nukeKnucklesDamage += nukeKnucklesUpgrade.damageBonus;
					
				}
			}
		}
		
		//SET THE RAW STATS
		attack.SetDamage(dmg);
		
		attack.SetAttackSpeed(atkSpeed);
		playerController.agilityBonus = playerSpeedBonus;
		
		luck = lck;
		
		//SET THE TECHNIQUES
		attack.SetDamageBonusAgainstAsteroids(bonusDmgAgainstAsteroids);
		
		
	}
    
}
