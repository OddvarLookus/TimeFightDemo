using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth;
    float currentHealth;
    [SerializeField] UnityEvent OnDeath;
    public void Initialize()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float _damage)
    {
        currentHealth -= _damage;
        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke();
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