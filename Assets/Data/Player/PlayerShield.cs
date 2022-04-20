using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerShield : MonoBehaviour
{
	[MinValue(0)] [SerializeField] int maxShield;
	int currentShield = 0;
	
	[SerializeField] float maxShieldCharge = 100f;
	float currentShieldCharge = 0f;
	[SerializeField] float shieldChargeSpeed;
	
	[SceneObjectsOnly] [SerializeField] Transform shieldGraphicsTr;
	
	protected void Awake()
	{
		currentShield = maxShield;
		RefreshShieldGraphics();
	}
    
	protected void FixedUpdate()
	{
		ShieldChargeBehavior();
	}
    
	void ShieldChargeBehavior()
	{
		if(currentShield > 0 && currentShield < maxShield)//alive and not max shield
		{
			if(currentShieldCharge < maxShieldCharge)
			{
				currentShieldCharge += Time.fixedDeltaTime;
			}
			else//shield is loaded
			{
				currentShield += 1;
				RefreshShieldGraphics();
			}
		}
		else
		{
			currentShieldCharge = 0f;
		}
	}
    
	public void TakeDamage(int dmg)
	{
		currentShield -= dmg;
		currentShield = Mathf.Clamp(currentShield, 0, int.MaxValue);
		
		if(dmg != 0)
		{
			RefreshShieldGraphics();
		}
		
		if(currentShield <= 0)
		{
			Die();
		}
	}
    
	void Die()
	{
		GameManager.instance.SetGameLost();
	}
	
	//GRAPHICS
	void RefreshShieldGraphics()
	{
		shieldGraphicsTr.gameObject.SetActive(currentShield > 1);
	}
    
}
