using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerShield : MonoBehaviour
{
	[MinValue(0)] [SerializeField] int maxShield;
	int currentShield = 0;
	[HideInInspector] public bool isDead = false;
	
	[SerializeField] float maxShieldCharge = 100f;
	float currentShieldCharge = 0f;
	[SerializeField] float shieldChargeSpeed;
	
	[SceneObjectsOnly] [SerializeField] Transform shieldGraphicsTr;
	[AssetsOnly] [SerializeField] GameObject playerExplosionPrefab;
	[SceneObjectsOnly] [SerializeField] Transform playerGraphics; 
	
	[SerializeField] SoundsPack shieldChargedSound;
	[SerializeField] SoundsPack shieldDepletedSound;
	[SerializeField] SoundsPack playerDeadSound;
	
	
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
				StaticAudioStarter.instance.StartAudioEmitter(transform.position, shieldChargedSound.GetRandomSound());
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
		else
		{
			StaticAudioStarter.instance.StartAudioEmitter(transform.position, shieldDepletedSound.GetRandomSound());
		}
		
	}
    
	void Die()
	{
		GetComponent<PlayerController>().SetMovementEnabled(false);
		GetComponent<Attack>().SetAttackEnabled(false);
		GetComponent<CapsuleCollider>().enabled = false;
		
		playerGraphics.gameObject.SetActive(false);
		Explode();
		StartCoroutine(PlayerDeadCoroutine());
	}
	
	IEnumerator PlayerDeadCoroutine()
	{
		yield return new WaitForSeconds(0.5f);
		GameManager.instance.SetGameLost();
	}
	
	void Explode()
	{
		GameObject bomb = Instantiate(playerExplosionPrefab);
		Transform bombTr = bomb.transform;
		bombTr.parent = null;
		bombTr.position = transform.position;
		StaticAudioStarter.instance.StartAudioEmitter(transform.position, playerDeadSound.GetRandomSound());
		
	}
	
	//GRAPHICS
	void RefreshShieldGraphics()
	{
		shieldGraphicsTr.gameObject.SetActive(currentShield > 1);
	}
    
}
