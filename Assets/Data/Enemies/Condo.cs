using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Condo : Enemy
{
	Vector3 initialPos;
	
	[Header("Movement")]
	[SerializeField] float minMoveTime;
	[SerializeField] float maxMoveTime;
	float moveTime;
	float curMoveTime = 0f;
	
	[SerializeField] float moveDistanceRadius;
	[SerializeField] float minMoveForce;
	[SerializeField] float maxMoveForce;
	
	
	[SerializeField] float rotationSpeed;
	
	[Header("Aggro")]
	[SerializeField] float aggroRadius;
	Transform aggroTarget;
	
	[SerializeField] float minMoveTimeAggro;
	[SerializeField] float maxMoveTimeAggro;
	
	[SerializeField] float minMoveForceAggro;
	[SerializeField] float maxMoveForceAggro;
	[MinValue(0f)] [SerializeField] float moveForceAggroDistanceMultiplier;
	
	[SerializeField] float desiredDistanceAggro;
	[MinValue(0f)] [SerializeField] float moveRandomnessAggro;
	
	[Space]
	[MinValue(0f)] [SerializeField] float meleeAggroDistance;
	bool meleeAggro;
	[MinValue(0f)] [SerializeField] float minHeadbuttWaitTime;
	[MinValue(0f)] [SerializeField] float maxHeadbuttWaitTime;
	float headbuttWaitTime = 0f;
	float currentHeadbuttWaitTime = 0f;
	[MinValue(0f)] [SerializeField] float meleeAggroRotationSpeed;
	
	[MinValue(0f)] [SerializeField] float meleeAttackRadius;
	[MinValue(0f)] [SerializeField] float meleeAttackOffset;
	bool headbutting = false;
	
	[Header("Shooting")]
	[MinValue(1)] [SerializeField] int minShotFrequency;
	[MinValue(1)] [SerializeField] int maxShotFrequency;
	int shotFrequency = 0;
	int currentShotStep = 0;
	
	[AssetsOnly] [SerializeField] GameObject bullet;
	[AssetsOnly] [SerializeField] GameObject shootVFXPrefab;
	[SceneObjectsOnly] [SerializeField] Transform bulletSpawnPos;
	[SerializeField] SoundsPack shootSound;
	Animator animator;
	
	protected override void OnEnable()
	{
		base.OnEnable();
		
	}
	
	protected override void OnDisable()
	{
		base.OnDisable();
	}
	
	protected override void OnStaggerStart()
	{
		base.OnStaggerStart();
		
		base.rb.velocity = Vector3.zero;
		curMoveTime = 0f;
	}
	
	protected override void OnStaggerEnd()
	{
		base.OnStaggerEnd();
	}
	
	protected override void OnDeath()
	{
		base.OnDeath();
	}
	
	protected override void Start()
	{
		base.Start();
		InitializeTimeNeutral();
		initialPos = transform.position;
		animator = GetComponent<Animator>();
	}
	
	protected override void Update()
	{
		base.Update();
	}
	
	// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	protected void FixedUpdate()
	{
		CondoBehavior();
		
		
	}
	
	void CondoBehavior()
	{
		if(base.aggroState == EnemyAggroState.NEUTRAL && !health.IsStaggered())
		{
			if(curMoveTime < moveTime)
			{
				curMoveTime += Time.deltaTime;
			}
			else
			{
				InitializeTimeNeutral();
				MoveToNextPosNeutral();
			}
			
			base.RotateTowardsMovement(rotationSpeed, true);
			
			Collider[] collidersInArea = Physics.OverlapSphere(transform.position, aggroRadius, 1<<7, QueryTriggerInteraction.Ignore);
			if(collidersInArea.Length > 0)
			{
				aggroTarget = collidersInArea[0].transform;
				base.aggroState = EnemyAggroState.AGGRO;
				InitializeTimeAggro();
			}
			
		}
		else if(base.aggroState == EnemyAggroState.AGGRO && !health.IsStaggered())
		{
			
			if(meleeAggro == false)
			{
				Vector3 vecToTarget = aggroTarget.position - transform.position;
				if(vecToTarget.magnitude <= meleeAggroDistance)//ENTER MELEE AGGRO
				{
					meleeAggro = true;
					curMoveTime = 0f;
					InitializeMeleeAggroTime();
					return;
				}
				
				transform.LookAt(aggroTarget, Vector3.up);
			
				if(curMoveTime < moveTime)
				{
					curMoveTime += Time.deltaTime;
				}
				else
				{
					InitializeTimeAggro();
					MoveToNextPosAggro();
				}
			}
			else//IN MELEE AGGRO
			{
				Vector3 vecToTarget = aggroTarget.position - transform.position;
				Vector3 perp = Vector3.Cross(vecToTarget.normalized, Vector3.up);
				Vector3 realUp = Vector3.Cross(vecToTarget.normalized, -perp.normalized);
				Quaternion finalRot = Quaternion.LookRotation(vecToTarget, realUp);
				transform.rotation = Quaternion.Slerp(transform.rotation, finalRot, meleeAggroRotationSpeed * Time.fixedDeltaTime);
				
				if(!headbutting && currentHeadbuttWaitTime < headbuttWaitTime)
				{
					currentHeadbuttWaitTime += Time.fixedDeltaTime;
				}
				else if(!headbutting && currentHeadbuttWaitTime >= headbuttWaitTime)//HEADBUTT
				{
					StartPerformHeadbutt();
					headbutting = true;
					
				}
				
			}

		}
	}
	
	//NEUTRAL
	void MoveToNextPosNeutral()
	{
		Vector3 newPointInRadius = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
		newPointInRadius *= Random.Range(0f, moveDistanceRadius);
		newPointInRadius += initialPos;
		
		Vector3 vecToNewPoint = newPointInRadius - transform.position;
		base.rb.AddForce(vecToNewPoint.normalized * Random.Range(minMoveForce, maxMoveForce), ForceMode.Impulse);
	}
	
	void InitializeTimeNeutral()
	{
		moveTime = Random.Range(minMoveTime, maxMoveTime);
		curMoveTime = 0f;
	}
	
	//AGGRO
	void MoveToNextPosAggro()
	{
		Vector3 vecToTarget = aggroTarget.position - transform.position;
		float distToTarget = vecToTarget.magnitude;
		Vector3 newMoveDir = Vector3.zero;
		if(distToTarget <= desiredDistanceAggro)//go far
		{
			newMoveDir = -vecToTarget.normalized;
		}
		else//go towards target
		{
			newMoveDir = vecToTarget.normalized;
		}
		
		Quaternion randRot = Quaternion.Euler(Random.Range(-moveRandomnessAggro, moveRandomnessAggro), Random.Range(-moveRandomnessAggro, moveRandomnessAggro), Random.Range(-moveRandomnessAggro, moveRandomnessAggro));
		newMoveDir = randRot * newMoveDir;
		
		newMoveDir *= Random.Range(minMoveForceAggro, maxMoveForceAggro) + distToTarget * moveForceAggroDistanceMultiplier;
		base.rb.AddForce(newMoveDir, ForceMode.Impulse);
		
		
		ComputeShotFrequency(1);
	}
	
	void InitializeTimeAggro()
	{
		moveTime = Random.Range(minMoveTimeAggro, maxMoveTimeAggro);
		curMoveTime = 0f;
	}
	
	void ComputeShotFrequency(int addValue)
	{
		currentShotStep += addValue;
		if(currentShotStep >= shotFrequency)
		{
			shotFrequency = Random.Range(minShotFrequency, maxShotFrequency);
			currentShotStep = 0;
			//Start Animations for shooting
			animator.Play("Condo Attack Prepare", -1, 0f);
			StartCoroutine(WaitAttackPrepareCoroutine());
		}
	}
	IEnumerator WaitAttackPrepareCoroutine()
	{
		yield return new WaitForEndOfFrame();
		float animationTime = animator.GetCurrentAnimatorStateInfo(0).length;
		StartCoroutine(AttackPrepareCoroutine(animationTime));
	}
	IEnumerator AttackPrepareCoroutine(float animTime)
	{
		yield return new WaitForSeconds(animTime);
		//FINISHED PREPARE ANIMATION
		animator.Play("Condo Attack Perform", -1, 0f);
		StartCoroutine(WaitAttackPerformCoroutine());
	}
	IEnumerator WaitAttackPerformCoroutine()
	{
		yield return new WaitForEndOfFrame();
		float animationTime = animator.GetCurrentAnimatorStateInfo(0).length / 3f;
		StartCoroutine(AttackPerformCoroutine(animationTime));
	}
	IEnumerator AttackPerformCoroutine(float animTime)
	{
		yield return new WaitForSeconds(animTime);//TIME TO SHOOT
		Shoot();
	}
	
	
	void Shoot()
	{
		GameObject b = Instantiate(bullet);
		b.transform.SetParent(null);
		b.transform.position = bulletSpawnPos.position;
			
		GameObject bVfx = Instantiate(shootVFXPrefab);
		bVfx.transform.SetParent(this.transform);
		bVfx.transform.position = bulletSpawnPos.position;
			
		Vector3 shootDir = aggroTarget.position - bulletSpawnPos.position;
		b.GetComponent<EnemyBullet>().Shoot(shootDir, aggroTarget);
			
		StaticAudioStarter.instance.StartAudioEmitter(transform.position, shootSound.GetRandomSound(), shootSound.GetRandomPitch());
	}
	
	//MELEE AGGRO
	
	void InitializeMeleeAggroTime()
	{
		headbuttWaitTime = Random.Range(minHeadbuttWaitTime, maxHeadbuttWaitTime);
		currentHeadbuttWaitTime = 0f;
	}
	Coroutine headbuttCrt;
	void StartPerformHeadbutt()
	{
		animator.Play("Condo Attack Prepare Melee", -1, 0f);
		headbuttCrt = StartCoroutine(WaitHeadbuttPrepareCoroutine());
	}
	IEnumerator WaitHeadbuttPrepareCoroutine()
	{
		yield return new WaitForEndOfFrame();
		float animationTime = animator.GetCurrentAnimatorStateInfo(0).length;
		headbuttCrt = StartCoroutine(HeadbuttPrepareCoroutine(animationTime));
	}
	IEnumerator HeadbuttPrepareCoroutine(float animTime)
	{
		yield return new WaitForSeconds(animTime);
		//FINISHED PREPARE ANIMATION
		animator.Play("Condo Attack Perform Melee", -1, 0f);
		headbuttCrt = StartCoroutine(WaitHeadbuttPerformCoroutine());
	}
	IEnumerator WaitHeadbuttPerformCoroutine()
	{
		yield return new WaitForEndOfFrame();
		float animationTime = animator.GetCurrentAnimatorStateInfo(0).length / 3f;
		headbuttCrt = StartCoroutine(HeadbuttPerformCoroutine(animationTime));
	}
	IEnumerator HeadbuttPerformCoroutine(float animTime)
	{
		yield return new WaitForSeconds(animTime);//TIME TO SHOOT
		HeadbuttDamage();
		headbutting = false;
		InitializeMeleeAggroTime();
	}
	void HeadbuttDamage()
	{
		Vector3 damagePos = transform.position + transform.forward * meleeAttackOffset;
		Collider[] hitColliders = Physics.OverlapSphere(damagePos, meleeAttackRadius * transform.localScale.x, ~0, QueryTriggerInteraction.Ignore);
		for(int i = 0; i < hitColliders.Length; i++)
		{
			if(hitColliders[i].TryGetComponent(out PlayerShield shield))
			{
				shield.TakeDamage(1);
			}
		}
	}
	
	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, moveDistanceRadius);
		
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, aggroRadius);
		
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, desiredDistanceAggro);
		
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, meleeAggroDistance);
		Gizmos.DrawWireSphere(transform.position + transform.forward * meleeAttackOffset, meleeAttackRadius * transform.localScale.x);
		
		
	}
	
}
