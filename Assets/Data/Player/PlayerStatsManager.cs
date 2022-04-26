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
		rangedAttack = GetComponent<RangedAttack>();
		RecalculateStats();
	}
	
	//ITEMS
	List<BuyableObjectInfos> items = new List<BuyableObjectInfos>();
	
	public void AddItem(BuyableObjectInfos newItem)
	{
		items.Add(newItem);
		
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
		attack.SetDamage(dmg);
		
		//ATTACK SPEED RECALCULATION
		float atkSpeed = attack.baseAttackSpeed;
		
		for(int i = 0; i < items.Count; i++)
		{
			
			atkSpeed = atkSpeed + items[i].attackSpeedBonus;
			
		}
		attack.SetAttackSpeed(atkSpeed);
		
		//RANGED ATTACK BULLETS PER TARGET RECALCULATION
		int bulletsPerTarget = rangedAttack.baseBulletsPerTarget;
		
		for(int i = 0; i < items.Count; i++)
		{
			
			bulletsPerTarget = bulletsPerTarget + items[i].projectilesPerTargetBonus;
			
		}
		rangedAttack.SetBulletsPerTarget(bulletsPerTarget);
	}
    
}
