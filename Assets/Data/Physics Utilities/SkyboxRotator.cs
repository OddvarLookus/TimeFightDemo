using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    
	[SerializeField] float rotationSpeed;
	float currentSkyboxRot = 0f;
    
	
    
	protected void Start()
	{
		
	}
    
    void Update()
	{
		currentSkyboxRot += rotationSpeed * Time.deltaTime;
		if(currentSkyboxRot > 360f)
		{
			currentSkyboxRot = currentSkyboxRot - 360f;
		}
		else if(currentSkyboxRot < 0f)
		{
			currentSkyboxRot = 360f + currentSkyboxRot;
		}
		RenderSettings.skybox.SetFloat("_Rotation", currentSkyboxRot);
    }
}
