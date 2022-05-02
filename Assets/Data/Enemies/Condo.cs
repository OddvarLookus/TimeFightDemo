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
	
	[Header("Shooting")]
	[MinValue(1)] [SerializeField] int minShotFrequency;
	[MinValue(1)] [SerializeField] int maxShotFrequency;
	int shotFrequency = 0;
	int currentShotStep = 0;
	
	[AssetsOnly] [SerializeField] GameObject bullet;
	[SceneObjectsOnly] [SerializeField] Transform bulletSpawnPos;
	
	protected override void OnEnable()
	{
		base.OnEnable();
		initialPos = transform.position;
		InitializeTimeNeutral();
	}
	
	protected override void Start()
	{
		base.Start();
	}
	
	// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	protected void FixedUpdate()
	{
		CondoBehavior();
		
		
	}
	
	void CondoBehavior()
	{
		if(base.aggroState == EnemyAggroState.NEUTRAL)
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
		else if(base.aggroState == EnemyAggroState.AGGRO)
		{
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
			//Shooting code
			
			GameObject b = Instantiate(bullet);
			b.transform.SetParent(null);
			b.transform.position = bulletSpawnPos.position;
			
			Vector3 shootDir = aggroTarget.position - bulletSpawnPos.position;
			b.GetComponent<EnemyBullet>().Shoot(shootDir, aggroTarget);
		}
	}
	
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, moveDistanceRadius);
		
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, aggroRadius);
		
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, desiredDistanceAggro);
	}
	
}
