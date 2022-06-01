using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Ball : Enemy
{
	[SerializeField] float aggroRadius;
	
	[Header("AGGRO BEHAVIOR")]
	[SerializeField] float rotationSpeed;
	[MinValue(0.01f)] [SerializeField] float attackRollTimeInterval;
	[MinValue(0f), MaxValue(1f)] [SerializeField] float attackRollProbability;
	[MinValue(0f)] [SerializeField] float resetTimeAfterAttack;
	
	[Header("ATTACK VARIABLES")]
	[SerializeField] int damage;
	[SerializeField] float attackDuration;
	
	[SerializeField] float maxVibeDistance;
	[SerializeField] float monoVibeTime;
	[SerializeField] float attackRadius;
	[AssetsOnly] [SerializeField] GameObject tondoAttackVFX;
	
	Transform graphicsTr;
    protected override void OnEnable()
    {
        base.OnEnable();
    }
	
	protected override void Start()
	{
		base.Start();
		graphicsTr = transform.GetChild(0);
		StartCoroutine(TondoAttackRollCoroutine());
	}

    void FixedUpdate()
	{
		TondoBehavior();
    	

    }

	
	void TondoBehavior()
	{
		AggroCheck();
		
		if(aggroState == EnemyAggroState.AGGRO)
		{
			//ROTATE TOWARDS TARGET
			Vector3 vecToTarget = aggroTarget.position - transform.position;
			Vector3 perpendicular = Vector3.Cross(vecToTarget.normalized, Vector3.up);
			Vector3 nUp = Vector3.Cross(vecToTarget.normalized, -perpendicular.normalized);
			
			Quaternion targetRot = Quaternion.LookRotation(vecToTarget.normalized, nUp.normalized);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
			
			
		}
		else if(aggroState == EnemyAggroState.NEUTRAL)
		{
			//FLOAT LIKE A BUTTERFLY
			
		}
		
		if(isPerformingExplosion)
		{
			//CHECK THE DAMAGE ON PLAYER FOR EXPLOSION
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius, 1<<7, QueryTriggerInteraction.Ignore);
			for(int i = 0; i < hitColliders.Length; i++)
			{
				if(!damagedObjects.Contains(hitColliders[i].transform) && hitColliders[i].TryGetComponent(out PlayerShield playerShield))
				{
					playerShield.TakeDamage(damage);
					damagedObjects.Add(hitColliders[i].transform);
				}
			}
		}
		
		
	}
	
	Transform aggroTarget;
	void AggroCheck()
	{
		Collider[] aggroColliders = Physics.OverlapSphere(transform.position, aggroRadius, 1<<7, QueryTriggerInteraction.Ignore);
		for(int i = 0; i < aggroColliders.Length; i++)
		{
			if(aggroColliders[i].TryGetComponent(out PlayerController playerController))
			{
				aggroTarget = aggroColliders[i].transform;
				aggroState = EnemyAggroState.AGGRO;
				return;
			}
		}
		aggroTarget = null;
		aggroState = EnemyAggroState.NEUTRAL;
	}
	
	bool attacking = false;
	bool isVibing = false;
	bool isPerformingExplosion = false;
	List<Transform> damagedObjects = new List<Transform>();
	void TondoAttackRoll()
	{
		if(aggroState == EnemyAggroState.AGGRO && !attacking)
		{
			bool roll = Random.Range(0f, 1f) <= attackRollProbability;
			if(roll)//attack
			{
				attacking = true;
				isVibing = true;
				LeanTween.scale(graphicsTr.gameObject, new Vector3(2.1f, 2.1f, 2.1f), attackDuration * 0.7f).setEase(LeanTweenType.easeOutCubic).setOnComplete(TondoAttackShrink);
				TondoVibe();
			}
		}
	}
	
	void TondoVibe()
	{
		if(isVibing)
		{
			Vector3 nRandPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
			nRandPos *= Random.Range(0f, maxVibeDistance);
			nRandPos += transform.position;
			LeanTween.move(graphicsTr.gameObject, nRandPos, monoVibeTime).setEase(LeanTweenType.easeShake).setOnComplete(TondoVibe);
		}
		else
		{
			graphicsTr.localPosition = Vector3.zero;
		}
	}
	
	void TondoAttackShrink()
	{
		isVibing = false;
		LeanTween.scale(graphicsTr.gameObject, new Vector3(0.6f, 0.6f, 0.6f), attackDuration * 0.2f).setEase(LeanTweenType.easeOutCubic).setOnComplete(TondoAttack);
	}
	
	void TondoAttack()
	{
		LeanTween.scale(graphicsTr.gameObject, new Vector3(1f, 1f, 1f), attackDuration * 0.1f).setEase(LeanTweenType.easeOutCubic);
		//INSTANTIATE ATTACK HITBOX AND EFFECTS
		GameObject nVfx = Instantiate(tondoAttackVFX);
		Transform vfxTr = nVfx.transform;
		vfxTr.SetParent(null, false);
		vfxTr.position = transform.position;
		
		
		damagedObjects.Clear();
		isPerformingExplosion = true;
		StartCoroutine(ResetExplosionCoroutine());
		StartCoroutine(ResetAttackingCoroutine());
		
	}
	
	IEnumerator ResetExplosionCoroutine()
	{
		yield return new WaitForSeconds(0.2f);
		
		isPerformingExplosion = false;
	}
	
	IEnumerator ResetAttackingCoroutine()
	{
		yield return new WaitForSeconds(resetTimeAfterAttack);
		attacking = false;
		
	}
	
	
	IEnumerator TondoAttackRollCoroutine()
	{
		yield return new WaitForSeconds(attackRollTimeInterval);
		TondoAttackRoll();
		StartCoroutine(TondoAttackRollCoroutine());
	}
	
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, aggroRadius);
		Gizmos.DrawWireSphere(transform.position, attackRadius);
	}

}
