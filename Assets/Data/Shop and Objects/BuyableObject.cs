using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyableObject : MonoBehaviour
{
	BuyableObjectInfos objectInfos;
	public void SetObjectInfos(BuyableObjectInfos newObjectInfos)
	{
		objectInfos = newObjectInfos;
		//InitializeGraphics();
	}
    
	protected void OnEnable()
	{
		InitializeGraphics();
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
    
    
}

[System.Serializable]
public class BuyableObjectInfos
{
	public float damageBonus;
	public float attackSpeedBonus;
	public int projectilesNumBonus;
	
	public GameObject graphicsPrefab;
}
