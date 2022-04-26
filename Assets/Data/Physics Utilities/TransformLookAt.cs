using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformLookAt : MonoBehaviour
{
	[SerializeField] Transform transformToLook;
	
    
	protected void Update()
	{
		Vector3 posToLook = transform.position + transform.position - transformToLook.position;
		transform.LookAt(posToLook);
	}
    
}
