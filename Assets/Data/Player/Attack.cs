using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Attack : MonoBehaviour
{
	[SceneObjectsOnly] [SerializeField] GameObject[] punchesGameObjects;
	[SceneObjectsOnly] [SerializeField] Transform[] punchesStartPositions;
	Collider[] punchesColliders;
	TriggerReporter[] punchesTriggerReporters;
	
	int attackIdx = 0;
	[SerializeField] CameraController cameraController;
	[SerializeField] PlayerController playerController;
	[SerializeField] float pushForce;
	public float baseDamage;
	float damage;
	[AssetsOnly] [SerializeField] GameObject punchVFX;
	[SerializeField] SoundsPack punchSounds;
	
	//ATTACK STARTING AND STOPPING
	bool attackEnabled = true;
	public bool GetAttackEnabled(){return attackEnabled;}
	public void SetAttackEnabled(bool nEnabled)
	{
		attackEnabled = nEnabled;
		if(nEnabled == false)
		{
			currentAttackTime = 0f;
			attacking = false;
			foreach(GameObject punchObj in punchesGameObjects)
			{
				punchObj.SetActive(false);
			}
		}
	}
	
	
	[Header("Attack Dynamics")]
	[SerializeField] float attackDistance;
	public float baseAttackSpeed;
	float attackSpeed;
	float attackTime;
	float currentAttackTime = 0f;
	
	[SerializeField] int numberOfPunches;
	[MinValue(0.1f)] [MaxValue(1f)] [SerializeField] float attackAnimationTimeMultiplier;
	float attackAnimationTime;
	float currentAttackAnimationTime = 0f;
	
	
	bool attacking = false;
	public bool IsAttacking(){return attacking;}

	
	
	[SerializeField] float initialAttackSize;
	[SerializeField] float finalAttackSize;
	

	
	public void SetDamage(float newDamage)
	{
		damage = newDamage;
	}
	
	public void SetAttackSpeed(float newAttackSpeed)
	{
		attackSpeed = newAttackSpeed;
		attackTime = 1f / attackSpeed;
		
		attackAnimationTime = attackTime * attackAnimationTimeMultiplier;
	}

    private void Awake()
	{
		punchesColliders = new Collider[2];
		punchesTriggerReporters = new TriggerReporter[2];
		for(int i = 0; i < punchesGameObjects.Length; i++)
		{
			punchesColliders[i] = punchesGameObjects[i].GetComponent<Collider>();
			punchesTriggerReporters[i] = punchesGameObjects[i].GetComponent<TriggerReporter>();
			punchesTriggerReporters[i].OnTriggerEnterAction += AttackCollisionEnter;
			
			punchesColliders[i].enabled = false;
		}
	    
    }
	
	protected void Update()
	{
		if(attackEnabled)
		{
			AttackBehavior();
		}
		
	}
	
    private void OnDisable()
	{
		foreach(TriggerReporter tr in punchesTriggerReporters)
		{
			tr.OnTriggerEnterAction -= AttackCollisionEnter;
		}
    }

    void AttackCollisionEnter(Collider _col)
    {
        if (_col.TryGetComponent(out Asteroid asteroid))
        {
            Vector3 pushVec = (_col.transform.position - transform.position).normalized;
            pushVec *= pushForce;
	        asteroid.Push(pushVec);
	        
	        asteroid.TakeDamage(damage, punchesGameObjects[currentPunchIdx].transform.position);
	        CheckAndSpawnPunchVFX(_col);
        }
        if (_col.TryGetComponent(out Health health))
        {
            if(health.GetAffiliation() == Affiliation.ENEMY)
            {
	            health.TakeDamage(damage, punchesGameObjects[currentPunchIdx].transform.position);
	            CheckAndSpawnPunchVFX(_col);
            }
        }
        if (_col.TryGetComponent(out Enemy enemy))
        {
            Vector3 pushVec = (_col.transform.position - transform.position).normalized;
            pushVec *= pushForce;
            enemy.Push(pushVec);
        }
    }
    
	void CheckAndSpawnPunchVFX(Collider hittingCollider)
	{
		Vector3 pos0 = transform.position;
		float dist = (hittingCollider.transform.position - transform.position).magnitude + hittingCollider.transform.localScale.x + 20f;
		Vector3 dir = (hittingCollider.transform.position - transform.position).normalized;
		Debug.DrawRay(pos0, dir * dist, Color.white, 0.5f);
		RaycastHit rhit;
		bool hit = Physics.Raycast(pos0, dir, out rhit, dist, ~0, QueryTriggerInteraction.Ignore);
		if(hit)
		{
			Debug.DrawLine(pos0, rhit.point, Color.green, 0.5f);
			GameObject vfx = Instantiate(punchVFX);
			Transform vfxTr = vfx.transform;
			vfxTr.parent = null;
			vfxTr.position = rhit.point;
			
			StaticAudioStarter.instance.StartAudioEmitter(rhit.point, punchSounds.GetRandomSound(), punchSounds.GetRandomPitch());
			
		}
	}
    
	
	
	int currentPunchIdx = 0;
	void AttackBehavior()
	{
		//bool attackPressed = Input.GetMouseButton(0);
		bool attackPressed = Input.GetButton("Punch");
		
		if(attackPressed)
		{
			if(!attacking)
			{
				if(true)
				{
					attacking = true;
				}
			}
		}
		
		if(attacking == true)
		{
			if(true)//NORMAL ATTACK
			{
				
				//ATTACK LOGIC
				if(currentAttackTime < attackTime)
				{
					currentAttackTime += Time.deltaTime;
				}
				else
				{
					currentAttackTime = 0f;
					currentAttackAnimationTime = 0f;
					attacking = false;
					attackIdx = 0;
					currentPunchIdx = 0;
					
					//ATTACK FINISHED, RESET PUNCHES
					for(int i = 0; i < punchesGameObjects.Length; i++)
					{
						punchesGameObjects[i].transform.position = punchesStartPositions[i].position;
						punchesGameObjects[i].transform.rotation = punchesStartPositions[i].rotation;
						punchesGameObjects[i].transform.localScale = new Vector3(initialAttackSize, initialAttackSize, initialAttackSize);
						punchesColliders[i].enabled = false;
					}
					return;
				}
				
				//ATTACK ANIMATION
				if(currentAttackAnimationTime < attackAnimationTime)
				{
					currentAttackAnimationTime += Time.deltaTime;
				}
				
				Vector3 initSize = new Vector3(initialAttackSize, initialAttackSize, initialAttackSize);
				Vector3 finalSize = new Vector3(finalAttackSize, finalAttackSize, finalAttackSize);
				currentPunchIdx = attackIdx % 2;
				if(punchesColliders[currentPunchIdx].enabled == false)
				{
					punchesColliders[currentPunchIdx].enabled = true;
				}
				
				
				float cellTime = attackAnimationTime / (float)numberOfPunches;
				
				if(currentAttackAnimationTime <= (cellTime / 2f) + (float)attackIdx * cellTime)//first part
				{
					float t = (currentAttackAnimationTime - (float)attackIdx * cellTime) / (cellTime / 2f);
					punchesGameObjects[currentPunchIdx].transform.position = Vector3.Lerp(punchesStartPositions[currentPunchIdx].position, GetFrontAttackPosition(), 1f- ( 1f - t * t));
					punchesGameObjects[currentPunchIdx].transform.localScale = Vector3.Lerp(initSize, finalSize, 1f- ( 1f - t * t));
				}
				else if(currentAttackAnimationTime > (cellTime / 2f) + (float)attackIdx * cellTime && currentAttackAnimationTime < cellTime + (float)attackIdx * cellTime)//second part
				{
					float t = (((currentAttackAnimationTime - ((float)attackIdx * cellTime))) - (cellTime / 2f)) / (cellTime / 2f);
					punchesGameObjects[currentPunchIdx].transform.position = Vector3.Lerp(GetFrontAttackPosition(), punchesStartPositions[currentPunchIdx].position, t * t);
					punchesGameObjects[currentPunchIdx].transform.localScale = Vector3.Lerp(finalSize, initSize, t * t);
				}
				else//attack animation finished
				{
					punchesColliders[currentPunchIdx].enabled = false;
					punchesColliders[currentPunchIdx].transform.position = punchesStartPositions[currentPunchIdx].position;
					punchesColliders[currentPunchIdx].transform.rotation = punchesStartPositions[currentPunchIdx].rotation;
					
					
					attackIdx += 1;
				}
				

				
			}


		}
	}
	
	Vector3 GetFrontAttackPosition()
	{
		Vector3 hForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
		Vector3 perpendicular = Vector3.Cross(hForward, Vector3.up).normalized;
		
		Quaternion punchRotation = Quaternion.AngleAxis(cameraController.GetCurrentTilt(), perpendicular);
		hForward = punchRotation * hForward;
		
		hForward *= attackDistance;
		
		hForward += transform.position;
		
		
		punchesGameObjects[currentPunchIdx].transform.rotation = punchRotation * punchesStartPositions[currentPunchIdx].rotation;
		
		return hForward;
	}

    

    
}
