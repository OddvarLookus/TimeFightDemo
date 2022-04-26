using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyableObject : MonoBehaviour, IInteractable
{
	BuyableObjectInfos objectInfos;
	public void SetObjectInfos(BuyableObjectInfos newObjectInfos)
	{
		objectInfos = newObjectInfos;
		InitializeGraphics();
	}
    
	protected void OnEnable()
	{
		//InitializeGraphics();
	}
    
	void InitializeGraphics()
	{
		if(transform.childCount == 0)
		{
			GameObject gg = Instantiate(objectInfos.graphicsPrefab);
			gg.transform.SetParent(transform);
			gg.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
	}
	
	public void Interact(PlayerController pc)
	{
		bool bought = pc.GetComponent<CreditsSucker>().TryBuy(objectInfos.cost);
		if(bought)
		{
			pc.GetComponent<PlayerStatsManager>().AddItem(objectInfos);
			Destroy(this.gameObject);
		}
		else
		{
			return;
		}
	}
    
	public string GetInteractionDescription()
	{
		return $"Buy {objectInfos.objectName} - {objectInfos.cost} CRUPS";
	}
    
}

[System.Serializable]
public class BuyableObjectInfos
{
	[Header("NAME")]
	public string objectName;
	
	[Header("COST")]
	public int cost;
	
	[Header("BONUSES")]
	public float damageBonus;
	public float attackSpeedBonus;
	public int projectilesPerTargetBonus;
	
	[Header("GRAPHICS")]
	public GameObject graphicsPrefab;
}
