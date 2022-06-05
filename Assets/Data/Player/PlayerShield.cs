using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerShield : MonoBehaviour
{
	[MinValue(0)] [SerializeField] int maxShield;
	int currentShield = 0;
	public int GetCurrentShield(){return currentShield;}
	[HideInInspector] public bool isDead = false;
	
	[SerializeField] float maxShieldCharge = 100f;
	float currentShieldCharge = 0f;
	[SerializeField] float shieldChargeSpeed;
	
	[SceneObjectsOnly] [SerializeField] Transform shieldGraphicsTr;
	[SceneObjectsOnly] [SerializeField] Transform shieldReturnGraphicsTr;
	[AssetsOnly] [SerializeField] GameObject playerExplosionPrefab;
	[SceneObjectsOnly] [SerializeField] Transform playerGraphics; 
	
	[SerializeField] SoundsPack shieldChargedSound;
	[SerializeField] SoundsPack shieldDepletedSound;
	[SerializeField] SoundsPack playerDeadSound;
	
	Material shieldMaterial;
	
	protected void Awake()
	{
		currentShield = maxShield;
		
		shieldMaterial = shieldGraphicsTr.GetComponent<Renderer>().material;
		RefreshShieldGraphics();
	}
    
	protected void FixedUpdate()
	{
		ShieldChargeBehavior();
	}
    
    
	int currentBlink = 0;
	void ShieldChargeBehavior()
	{
		if(currentShield > 0 && currentShield < maxShield)//alive and not max shield
		{
			if(currentShieldCharge < maxShieldCharge)
			{
				currentShieldCharge += Time.fixedDeltaTime;
				
				ShieldBlinkAnim();
			}
			else//shield is loaded
			{
				currentShield += 1;
				
				RefreshShieldGraphics();
				if(currentShield > 1)//shield return anim
				{
					ShieldReturnAnim();
				}
				
				StaticAudioStarter.instance.StartAudioEmitter(transform.position, shieldChargedSound.GetRandomSound());
				
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
			currentBlink = 0;
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
		if(currentShield > 1)
		{
			shieldMaterial.SetFloat("_Alpha", 1f);
		}
		else if(currentShield <= 1)
		{
			shieldMaterial.SetFloat("_Alpha", 0f);
		}
	}
	
	void ShieldReturnAnim()
	{
		Material shieldReturnMat = shieldReturnGraphicsTr.GetComponent<Renderer>().material;
		LeanTween.value(shieldReturnGraphicsTr.gameObject, 1f, 0f, 0.2f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float nval) =>
		{
			shieldReturnMat.SetFloat("_Alpha", nval);
		});
		
		shieldReturnGraphicsTr.localScale = new Vector3(7f, 7f, 7f);
		LeanTween.scale(shieldReturnGraphicsTr.gameObject, new Vector3(3f, 3f, 3f), 0.2f).setEase(LeanTweenType.easeOutQuad);
	}
    
	void ShieldBlinkAnim()
	{
		if(currentShieldCharge >= maxShieldCharge - 1.8f && currentBlink == 0)//first blink
		{
			LeanTween.cancel(shieldGraphicsTr.gameObject);
			
			LTSeq blinkSequence = LeanTween.sequence();
			blinkSequence.append(LeanTween.value(shieldGraphicsTr.gameObject, 0f, 0.3f, 0.1f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float nval) =>
			{
				shieldMaterial.SetFloat("_Alpha", nval);
			}));
			blinkSequence.append(LeanTween.value(shieldGraphicsTr.gameObject, 0.3f, 0f, 0.1f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float nval) =>
			{
				shieldMaterial.SetFloat("_Alpha", nval);
			}));
			currentBlink += 1;
		}
		
		if(currentShieldCharge >= maxShieldCharge - 0.9f && currentBlink == 1)//second blink
		{
			LeanTween.cancel(shieldGraphicsTr.gameObject);
			
			LTSeq blinkSequence = LeanTween.sequence();
			blinkSequence.append(LeanTween.value(shieldGraphicsTr.gameObject, 0f, 0.3f, 0.1f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float nval) =>
			{
				shieldMaterial.SetFloat("_Alpha", nval);
			}));
			blinkSequence.append(LeanTween.value(shieldGraphicsTr.gameObject, 0.3f, 0f, 0.1f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float nval) =>
			{
				shieldMaterial.SetFloat("_Alpha", nval);
			}));
			blinkSequence.append(0.2f);
			blinkSequence.append(LeanTween.value(shieldGraphicsTr.gameObject, 0f, 0.3f, 0.1f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float nval) =>
			{
				shieldMaterial.SetFloat("_Alpha", nval);
			}));
			blinkSequence.append(LeanTween.value(shieldGraphicsTr.gameObject, 0.3f, 0f, 0.1f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float nval) =>
			{
				shieldMaterial.SetFloat("_Alpha", nval);
			}));
			
			currentBlink += 1;
		}

		
	}

	
    
}
