using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Asteroid : MonoBehaviour
{
	
    [SerializeField] float minInitSpeed;
    [SerializeField] float maxInitSpeed;
    [SerializeField] float mass = 10f;
    [SerializeField] float maxHealth;
    float currentHealth;

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
                nDrop.transform.position = transform.position + GetSpawnPos();
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
