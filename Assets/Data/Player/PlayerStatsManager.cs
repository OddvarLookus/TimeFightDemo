using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
	//REFERENCES
	Attack attack;
    
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
	
	//STATS CALCULATIONS
    
	void RecalculateStats()
	{
		float dmg = attack.baseDamage;
		
		for(int i = 0; i < items.Count; i++)
		{
			
			dmg = dmg + items[i].damageBonus;
			
		}
		
		attack.SetDamage(dmg);
	}
    
}
