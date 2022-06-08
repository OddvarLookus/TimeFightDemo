using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
	//REFERENCES
	Attack attack;
	PlayerController playerController;
	protected void Awake()
	{
		attack = GetComponent<Attack>();
		playerController = GetComponent<PlayerController>();
		RecalculateStats();
	}
	
	//ITEMS
	List<BuyableObjectInfos> items = new List<BuyableObjectInfos>();
	
	public void AddItem(BuyableObjectInfos newItem)
	{
		items.Add(newItem);
		
		RecalculateStats();
	}
	
	List<Upgrade> upgrades = new List<Upgrade>();
	public void AddUpgrade(Upgrade nUpgrade)
	{
		upgrades.Add(nUpgrade);
		
		RecalculateStats();
	}
	
	//HELD STATS
	static float baseLuck = 1f;
	public static float luck = 0f;
	
	
	//STATS CALCULATIONS
    
	void RecalculateStats()
	{
		float dmg = attack.baseDamage;
		
		float atkSpeed = attack.baseAttackSpeed;
		float playerSpeedBonus = 0f;
		
		float lck = baseLuck;
		
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
				
			}
		}
		

		attack.SetDamage(dmg);
		
		attack.SetAttackSpeed(atkSpeed);
		playerController.agilityBonus = playerSpeedBonus;
		
		luck = lck;
		
	}
    
}
