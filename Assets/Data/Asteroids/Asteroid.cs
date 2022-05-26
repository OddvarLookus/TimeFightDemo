using System.Collections;
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
	Material asteroidMat;
	
	[Header("Sound")]
	[SerializeField] SoundsPack asteroidsDestructionSounds;
	
	[Header("Feedback")]
	[SerializeField] float damageScaleChange;
	
	[Header("Drops")]
	[SerializeField] float dropsReleaseSpeed = 25f;
	[SerializeField] float dropsReleaseSpeedRandom = 13f;
    [SerializeField] float dropsReleaseRadius;
    [SerializeField] Drop[] drops;
	
	
	
    Rigidbody rb;
    void Start()
    {
        currentHealth = maxHealth;
		
	    asteroidMat = GetComponent<Renderer>().materials[0];
	    RefreshBreakTexture();
		
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

	public void TakeDamage(float _damage, Vector3 damagePoint)
	{
		//deplete health and die
		currentHealth -= _damage;
		
		DamageNumbersManager.instance.SpawnDamageNumber(_damage, damagePoint);
		
        if (currentHealth <= 0f)
        {
            AsteroidDestroy();
        }
        else
        {
        	RefreshBreakTexture();
        	if(_damage > 0f)
        	{
        		Vector3 baseScale = transform.localScale;
        		Vector3 nwScale = new Vector3(transform.localScale.x - damageScaleChange, transform.localScale.y - damageScaleChange, transform.localScale.z - damageScaleChange);
        		LTDescr tw = LeanTween.scale(this.gameObject, nwScale, 0.03f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() => 
        		{
        			LeanTween.scale(this.gameObject, baseScale, 0.05f).setEase(LeanTweenType.easeInQuad);
        		});
        	}
        }
    }

	void RefreshBreakTexture()
	{
		float breakVal = 1f - (currentHealth / maxHealth);
		asteroidMat.SetFloat("_breakValue", breakVal);
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
	            

	            //if(nDrop.TryGetComponent(out Credit crdt))
	            //{
	            //	Vector3 crdtVelocity = relativeSpawnPos.normalized * (dropsReleaseSpeed * Random.Range(-dropsReleaseSpeedRandom, dropsReleaseSpeedRandom));
	            //	crdt.SetVelocity(crdtVelocity);
	            //}
            }
        }
		StaticAudioStarter.instance.StartAudioEmitter(transform.position, asteroidsDestructionSounds.GetRandomSound(), asteroidsDestructionSounds.GetRandomPitch());


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
