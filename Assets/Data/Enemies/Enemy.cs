using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

//ENEMY HAS THE GENERIC THINGS FOR ALL ENEMIES. 
public class Enemy : SerializedMonoBehaviour
{
    protected Rigidbody rb;

    protected Health health;
    public Affiliation GetHealthAffiliation()
    {
        return health.GetAffiliation();
    }

    [SerializeField] protected Renderer mainRenderer;

	[Header("Stats")]
	[SerializeField] EnemyStatsSet statsSet;
	public EnemyStatsSet GetStatsSet(){return statsSet;}
	[SerializeField] protected EnemySize currentSize;
	public void SetEnemySize(EnemySize nSize)
	{
		currentSize = nSize;
	}
	public EnemySize GetEnemySize()
	{
		return currentSize;
	}
    
	protected EnemyAggroState aggroState = EnemyAggroState.NEUTRAL;

    public Renderer GetRenderer()
    {
        return mainRenderer;
    }

    protected virtual void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();

        
    }
    
	protected virtual void OnDisable()
	{
		health.OnStaggerStart -= this.OnStaggerStart;
		health.OnStaggerEnd -= this.OnStaggerEnd;
		health.OnDeathAction -= this.OnDeath;
	}
    
	protected virtual void Start()
	{
		InitializeEnemy();
		StartSoundTimer();
		
		health.OnStaggerStart += this.OnStaggerStart;
		health.OnStaggerEnd += this.OnStaggerEnd;
		health.OnDeathAction += this.OnDeath;
	}
    
	protected virtual void Update()
	{
		if(health.IsStaggered())//while staggered stagger effect follows enemy
		{
			currentStaggerVFX.transform.position = transform.position + staggerVFXOffset;
		}
	}
	
	[Header("Stagger")]
	[AssetsOnly] [SerializeField] GameObject staggerVFXPrefab;
	[SerializeField] Vector3 staggerVFXOffset;
	GameObject currentStaggerVFX;
	
	protected virtual void OnStaggerStart()
	{
		currentStaggerVFX = Instantiate(staggerVFXPrefab);
		currentStaggerVFX.transform.SetParent(null);
		currentStaggerVFX.transform.position = transform.position + staggerVFXOffset;
	}
	
	protected virtual void OnStaggerEnd()
	{
		Destroy(currentStaggerVFX);
	}
	
	protected virtual void OnDeath()
	{
		if(health.IsStaggered())
		{
			Destroy(currentStaggerVFX);
		}
	}

    void InitializeEnemy()
    {
	    float mHealth = statsSet.enemyStats[currentSize].maxHealth;
	    float staggerPerc = statsSet.enemyStats[currentSize].staggerHealthPercentage;
	    float staggerDecRate = statsSet.enemyStats[currentSize].staggerDecreaseRate;
	    float staggerDur = statsSet.enemyStats[currentSize].staggerDuration;
	    health.Initialize(mHealth, staggerPerc, staggerDecRate, staggerDur);
	    
	    float nScale = statsSet.enemyStats[currentSize].scale;
	    transform.localScale = new Vector3(nScale, nScale, nScale);
    }
	
	protected virtual void RotateTowardsMovement(float rotationSpeed, bool onlyZ = false)
	{
		if(!onlyZ)//all axes
		{
			if(rb.velocity.sqrMagnitude >= 0.1f)
			{
				Vector3 newForward = transform.forward;
				newForward = Vector3.Lerp(newForward.normalized, rb.velocity.normalized, rotationSpeed * Time.fixedDeltaTime).normalized;
				transform.forward = newForward;
			}
		}
		else
		{
			if(rb.velocity.sqrMagnitude >= 0.1f && Vector3.ProjectOnPlane(rb.velocity, Vector3.up).sqrMagnitude >= 0.1f)
			{
				Vector3 newForward = transform.forward;
				Vector3 hVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
				newForward = Vector3.Lerp(newForward.normalized, hVel.normalized, rotationSpeed * Time.fixedDeltaTime).normalized;
				transform.forward = newForward;
			}
		}

	}
	
	[Header("VOICE")]
	[AssetsOnly] [SerializeField] SoundsPack enemyVoice;
	[SerializeField] float minVoiceTime;
	[SerializeField] float maxVoiceTime;
	float voiceTime = 0f;
	void StartSoundTimer()
	{
		voiceTime = Random.Range(minVoiceTime, maxVoiceTime);
		StartCoroutine(SoundTimerCoroutine());
	}
	IEnumerator SoundTimerCoroutine()
	{
		yield return new WaitForSeconds(voiceTime);
		StaticAudioStarter.instance.StartAudioEmitter(transform.position, enemyVoice.GetRandomSound(), enemyVoice.GetRandomPitch());
		StartSoundTimer();
	}
	
	
    public void Push(Vector3 _pushForce)
    {
        rb.AddForce(_pushForce, ForceMode.Impulse);
    }
    
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position + staggerVFXOffset, 0.2f);
	}

}

public enum EnemyAggroState {NEUTRAL = 0, AGGRO = 1}




