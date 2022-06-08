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
		//DAMAGE RECALCULATION
		float dmg = attack.baseDamage;
		
		for(int i = 0; i < items.Count; i++)
		{
			
			dmg = dmg + items[i].damageBonus;
			
		}
		for(int i = 0; i < upgrades.Count; i++)
		{
			
			dmg = dmg + upgrades[i].statsUpgrade.damageBonus;
			
		}
		attack.SetDamage(dmg);
		
		//ATTACK SPEED RECALCULATION
		float atkSpeed = attack.baseAttackSpeed;
		float playerSpeedBonus = 0f;
		
		for(int i = 0; i < items.Count; i++)
		{
			
			atkSpeed = atkSpeed + items[i].attackSpeedBonus;
			
		}
		for(int i = 0; i < upgrades.Count; i++)
		{
			
			atkSpeed = atkSpeed + upgrades[i].statsUpgrade.attackSpeedBonus;
			
			playerSpeedBonus = playerSpeedBonus + (upgrades[i].statsUpgrade.attackSpeedBonus * 33.333f);
		}
		attack.SetAttackSpeed(atkSpeed);
		playerController.agilityBonus = playerSpeedBonus;
		
		//LUCK CALCULATIONS
		float lck = baseLuck;
		
		for(int i = 0; i < upgrades.Count; i++)
		{
			
			lck = lck + upgrades[i].statsUpgrade.luckBonus;
			
		}
		luck = lck;
		
	}
    
}
