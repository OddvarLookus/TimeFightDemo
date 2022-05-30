using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DisablerUtility : MonoBehaviour
{
    
	[SerializeField] GameObject[] thingsToDisable;
	[SerializeField] Renderer[] renderersToDisable;
	[SerializeField] Collider[] collidersToDisable;
	
	public void SetThingsEnabled(bool nEnabled)
	{
		for(int i = 0; i < thingsToDisable.Length; i++)
		{
			thingsToDisable[i].SetActive(nEnabled);
		}
		
		for(int i = 0; i < renderersToDisable.Length; i++)
		{
			renderersToDisable[i].enabled = nEnabled;
		}
		
		for(int i = 0; i < collidersToDisable.Length; i++)
		{
			collidersToDisable[i].enabled = nEnabled;
		}
	}
    
}
