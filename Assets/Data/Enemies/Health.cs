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
	[SerializeField] UnityEvent OnDeath;
	[SerializeField] GameObject deathPrefab;
    
	public void Initialize(float nMaxHealth)
    {
	    maxHealth = nMaxHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float _damage)
    {
        currentHealth -= _damage;
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