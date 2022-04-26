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
	Vector3 localPos0, localPos1, localPos2;
	
	bool hasInteracted = false;
	public void Interact(PlayerController pc)
	{
		if(!hasInteracted)
		{
			bool fromFront = Vector3.Dot((pc.transform.position - transform.position).normalized, transform.forward) >= 0f;
			
			ThrowObjects(fromFront);
			hasInteracted = true;
		}
		
	}
	
	protected void Start()
	{
		localPos0 = pos0.localPosition;
		localPos1 = pos1.localPosition;
		localPos2 = pos2.localPosition;
	}
	
	void ThrowObjects(bool front)
	{
		if(front)
		{
			pos0.localPosition = localPos0;
			pos1.localPosition = localPos1;
			pos2.localPosition = localPos2;
		}
		else
		{
			pos0.localPosition = new Vector3(localPos0.x, localPos0.y, -localPos0.z);
			pos1.localPosition = new Vector3(localPos1.x, localPos1.y, -localPos1.z);
			pos2.localPosition = new Vector3(localPos2.x, localPos2.y, -localPos2.z);
		}
		
		InstantiateBuyableObjectAtPos(pos0.position, 0);
		InstantiateBuyableObjectAtPos(pos1.position, 1);
		InstantiateBuyableObjectAtPos(pos2.position, 2);

	}
	
	void InstantiateBuyableObjectAtPos(Vector3 pos, int idx)
	{
		GameObject g = Instantiate(buyableObjectPrefab);
		Transform tr = g.transform;
		tr.SetParent(null);
		tr.position = pos;
		tr.GetComponent<BuyableObject>().SetObjectInfos(objectInfos[idx]);
	}
	
	public string GetInteractionDescription()
	{
		return "E - Shop";
	}
	
}
