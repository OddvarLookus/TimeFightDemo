using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Shop : MonoBehaviour, IInteractable
{
	
	[SerializeField] BuyableObjectInfos[] objectInfos;
	
	[Header("Buyable Object Prefab")]
	[AssetsOnly] [SerializeField] GameObject buyableObjectPrefab;
	
	[Header("Object Positions")]
	[SerializeField] Transform pos0, pos1, pos2;
	
	bool hasInteracted = false;
	public void Interact(PlayerController pc)
	{
		if(!hasInteracted)
		{
			ThrowObjects();
			hasInteracted = true;
		}
		
	}
	
	
	void ThrowObjects()
	{
		InstantiateBuyableObjectAtPos(pos0.position);
		InstantiateBuyableObjectAtPos(pos1.position);
		InstantiateBuyableObjectAtPos(pos2.position);
	}
	
	void InstantiateBuyableObjectAtPos(Vector3 pos)
	{
		GameObject g = Instantiate(buyableObjectPrefab);
		Transform tr = g.transform;
		tr.SetParent(null);
		tr.position = pos;
		tr.GetComponent<BuyableObject>().SetObjectInfos(objectInfos[0]);
	}
	
	public string GetInteractionDescription()
	{
		return "E - Shop";
	}
	
}
