using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
	//REFERENCES
	Attack attack;
	RangedAttack rangedAttack;
	protected void Awake()
	{
		attack = GetComponent<Attack>();
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
		
		for(int i = 0; i < items.Count; i++)
		{
			
			atkSpeed = atkSpeed + items[i].attackSpeedBonus;
			
		}
		for(int i = 0; i < upgrades.Count; i++)
		{
			
			atkSpeed = atkSpeed + upgrades[i].statsUpgrade.attackSpeedBonus;
			
		}
		attack.SetAttackSpeed(atkSpeed);
		
	}
    
}
