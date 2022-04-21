using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Interactor : MonoBehaviour
{
    
	[MinValue(0f)] [SerializeField] float interactionRadius;
	[SerializeField] float interactionDistance;
	[SerializeField] float interactionHeight;
    
    
	protected void Update()
	{
		Interact();
	}
	
	protected void FixedUpdate()
	{
		CheckInteractables();
	}
	
	//INTERACTOR LOGIC
	IInteractable currentInteractable;
	void CheckInteractables()
	{
		Collider[] cols = new Collider[5];
		Vector3 castPos = transform.position + (transform.forward * interactionDistance) + new Vector3(0f, interactionHeight, 0f);
		int colsNum = Physics.OverlapSphereNonAlloc(castPos, interactionRadius, cols, 1<<10, QueryTriggerInteraction.Collide);
		
		
		//iterate the colliders that are interactable, and check the nearest one to the player
		float smallestDist = float.MaxValue;
		int smallestDistIdx = int.MaxValue;
		for(int i = 0; i < colsNum; i++)
		{
			float distToCol = (cols[i].transform.position - transform.position).magnitude;
			if(distToCol < smallestDist)
			{
				smallestDist = distToCol;
				smallestDistIdx = i;
			}
		}
		
		if(smallestDistIdx != int.MaxValue)//interactables found
		{
			currentInteractable = cols[smallestDistIdx].gameObject.GetComponent<IInteractable>();
		}
		else//interactables not found
		{
			currentInteractable = null;
		}
		
	}
	
	void Interact()
	{
		if(Input.GetKeyDown(KeyCode.E))
		{
			if(currentInteractable != null)
			{
				currentInteractable.Interact(GetComponent<PlayerController>());
			}
		}
	}
	
	
    
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position + (transform.forward * interactionDistance) + new Vector3(0f, interactionHeight, 0f), interactionRadius);
		
	}
    
}
