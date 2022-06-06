using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] Affiliation affiliation;
    public Affiliation GetAffiliation()
    {
        return affiliation;
    }
    
    [SerializeField] float maxHealth;
	float currentHealth;
	public float GetCurrentHealth()
	{
		return currentHealth;
	}
    
	[SerializeField] GameObject feedbackObject;
	[SerializeField] float feedbackScaleChange = 2f;
    
	[SerializeField] UnityEvent OnDeath;
	public Action OnDeathAction;
	[SerializeField] GameObject deathPrefab;
	[SerializeField] SoundsPack deathSound;
	
	//VARS FOR STAGGERING
	float staggerHealth;
	float staggerDecreaseRate;
	float staggerDuration;
	float currentStaggerTime = 0f;
	
	float currentStagger = 0f;
	bool isStaggered = false;
	public bool IsStaggered(){return isStaggered;}
	
	//END OF STAGGERING
    
	public void Initialize(float nMaxHealth, float nStaggerHealthPercent, float nStaggerDecreaseRate, float nStaggerDuration)
    {
	    maxHealth = nMaxHealth;
	    currentHealth = maxHealth;
        
	    staggerHealth = nStaggerHealthPercent * maxHealth;
	    staggerDecreaseRate = nStaggerDecreaseRate * staggerHealth;
	    staggerDuration = nStaggerDuration;
    }

	public void TakeDamage(float _damage, Vector3 damagePoint)
    {
	    currentHealth -= _damage;
        
	    DamageNumbersManager.instance.SpawnDamageNumber(_damage, damagePoint);
        
	    if(_damage > 0f)
	    {
	    	//CALCULATE STAGGER
	    	if(!isStaggered && currentHealth > 0f)
	    	{
		    	currentStagger += _damage;
		    	if(currentStagger >= staggerHealth)
		    	{
			    	isStaggered = true;
			    	OnStaggerStart?.Invoke();
		    	}
	    	}

	    	//FEEDBACK	    	
		    Vector3 nextPos = _damage * feedbackScaleChange * (transform.position - damagePoint).normalized;
		    Vector3 startPos = feedbackObject.transform.position;
		    LeanTween.move(feedbackObject, startPos + nextPos, 0.1f).setEase(LeanTweenType.easeOutCubic).setOnComplete(() => 
		    {
		    	LeanTween.move(feedbackObject, startPos, 0.1f).setEase(LeanTweenType.easeOutCubic);
		    });
		    
	    }
        
        if (currentHealth <= 0f)
        {
        	Die();
        	OnDeathAction?.Invoke();
	        OnDeath?.Invoke();
        }
    }
    
	public void StopDamageFeedback()
	{
		LeanTween.cancel(feedbackObject);
	}
    
	void Die()
	{
		if(deathPrefab != null)
		{
			GameObject g = Instantiate(deathPrefab);
			Transform t = g.transform;
			t.parent = null;
			t.position = transform.position;
		}
		if(deathSound != null)
		{
			StaticAudioStarter.instance.StartAudioEmitter(transform.position, deathSound.GetRandomSound(), deathSound.GetRandomPitch());
		}
	}

    private void Update()
    {
	    StaggerBehavior();
    }
	
	public Action OnStaggerStart;
	public Action OnStaggerEnd;
	
	void StaggerBehavior()
	{
		if(!isStaggered)
		{
			currentStagger -= staggerDecreaseRate * Time.deltaTime;
			currentStagger = Mathf.Clamp(currentStagger, 0f, staggerHealth);
		}
		else if(isStaggered)
		{
			currentStaggerTime += Time.deltaTime;
			if(currentStaggerTime >= staggerDuration)
			{
				isStaggered = false;
				OnStaggerEnd?.Invoke();
				currentStaggerTime = 0f;
				currentStagger = 0f;
			}
		}
	}
	
    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}

public enum Affiliation {PLAYER = 0, ENEMY = 1}