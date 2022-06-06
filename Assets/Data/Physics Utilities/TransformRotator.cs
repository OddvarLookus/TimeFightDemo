using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRotator : MonoBehaviour
{
    
	[SerializeField] float rotationSpeed;
	
	protected void Update()
	{
		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + rotationSpeed * Time.deltaTime, transform.rotation.eulerAngles.z);
	}
    
}
