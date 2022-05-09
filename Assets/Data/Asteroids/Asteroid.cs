﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Asteroid : MonoBehaviour
{
	[Header("Initialization")]
    [SerializeField] float minInitSpeed;
    [SerializeField] float maxInitSpeed;
    [SerializeField] float mass = 10f;
    [SerializeField] float maxHealth;
    float currentHealth;
	
	[Header("Sound")]
	[SerializeField] SoundsPack asteroidsDestructionSounds;
	
	[Header("Drops")]
	[SerializeField] float dropsReleaseSpeed = 25f;
	[SerializeField] float dropsReleaseSpeedRandom = 13f;
    [SerializeField] float dropsReleaseRadius;
    [SerializeField] Drop[] drops;
	
	
	
    Rigidbody rb;
    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody>();
        rb.mass = mass;

        Vector3 initSpeed = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        initSpeed = initSpeed.normalized;
        initSpeed *= Random.Range(minInitSpeed, maxInitSpeed);
        rb.AddForce(initSpeed, ForceMode.Impulse);
    }

    public void Push(Vector3 _pushForce)
    {
        rb.AddForce(_pushForce, ForceMode.Impulse);
    }

	public void TakeDamage(float _damage)
	{
		//deplete health and die
        currentHealth -= _damage;
        if (currentHealth <= 0f)
        {
            AsteroidDestroy();
        }
    }

    public void AsteroidDestroy()
	{
		
        for (int i = 0; i < drops.Length; i++)
        {
            for (int n = 0; n < drops[i].dropsNum; n++)
            {
                GameObject nDrop = Instantiate(drops[i].dropPrefab);
	            nDrop.transform.SetParent(transform.parent, true);
	            Vector3 relativeSpawnPos = GetSpawnPos();
	            nDrop.transform.position = transform.position + relativeSpawnPos;
	            
	            StaticAudioStarter.instance.StartAudioEmitter(transform.position, asteroidsDestructionSounds.GetRandomSound(), asteroidsDestructionSounds.GetRandomPitch());

	            //if(nDrop.TryGetComponent(out Credit crdt))
	            //{
	            //	Vector3 crdtVelocity = relativeSpawnPos.normalized * (dropsReleaseSpeed * Random.Range(-dropsReleaseSpeedRandom, dropsReleaseSpeedRandom));
	            //	crdt.SetVelocity(crdtVelocity);
	            //}
            }
        }

        Destroy(this.gameObject);
    }
    Vector3 GetSpawnPos()
    {
        Vector3 nPos = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        nPos = nPos.normalized;
        nPos *= Random.Range(0f, dropsReleaseRadius);
        return nPos;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, dropsReleaseRadius);
    }

}

[System.Serializable]
public class Drop
{
    [SerializeField] public GameObject dropPrefab;
    [SerializeField] public int dropsNum;

    //[SerializeField] int numberOfRolls;
    //[SerializeField] float dropProbability;
}
