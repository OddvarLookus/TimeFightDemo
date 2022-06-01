using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
	[SerializeField] GameObject deathPrefab;
	[SerializeField] SoundsPack deathSound;
    
	public void Initialize(float nMaxHealth)
    {
	    maxHealth = nMaxHealth;
        currentHealth = maxHealth;
    }

	public void TakeDamage(float _damage, Vector3 damagePoint)
    {
	    currentHealth -= _damage;
        
	    DamageNumbersManager.instance.SpawnDamageNumber(_damage, damagePoint);
        
	    if(_damage > 0f)//feedback
	    {
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
	        OnDeath?.Invoke();
        }
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
        
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}

public enum Affiliation {PLAYER = 0, ENEMY = 1}