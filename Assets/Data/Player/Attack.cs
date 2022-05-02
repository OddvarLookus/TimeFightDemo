using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Attack : MonoBehaviour
{
    [SerializeField] GameObject attackCollider;
    Collider col;
	TriggerReporter attackTriggerReporter;
	[SerializeField] CameraController cameraController;
	[SerializeField] PlayerController playerController;
    [SerializeField] float pushForce;
	public float baseDamage;
	float damage;
	[AssetsOnly] [SerializeField] GameObject punchVFX;
	
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
			isChargedAttack = false;
			col.enabled = false;
			attackCollider.SetActive(false);
		}
	}
	
	
	[Header("Attack Dynamics")]
	[SerializeField] float attackDistance;
	public float baseAttackSpeed;
	float attackSpeed;
	float attackTime;
	float currentAttackTime = 0f;
	bool attacking = false;
	public bool IsAttacking(){return attacking;}
	bool isChargedAttack = false;
	public bool IsChargedAttack(){return isChargedAttack;}
	bool isChargedAttackFinished = false;
	public bool IsChargedAttackFinished(){return isChargedAttackFinished;}
	
	
	[SerializeField] float initialAttackSize;
	[SerializeField] float finalAttackSize;
	
	[Header("Charged Attack Dynamics")]
	[SerializeField] float chargedAttackDistance;
	[SerializeField] float chargedAttackTime;
	[SerializeField] float chargedAttackFinishedTime;
	[SerializeField] float initialChargedAttackSize;
	[SerializeField] float finalChargedAttackSize;
	
	public void SetDamage(float newDamage)
	{
		damage = newDamage;
	}
	
	public void SetAttackSpeed(float newAttackSpeed)
	{
		attackSpeed = newAttackSpeed;
		attackTime = 1f / attackSpeed;
	}

    private void Awake()
    {
        col = attackCollider.GetComponent<Collider>();
	    col.enabled = false;
        attackTriggerReporter = attackCollider.GetComponent<TriggerReporter>();
	    attackTriggerReporter.OnTriggerEnterAction += AttackCollisionEnter;
	    attackCollider.SetActive(false);
        
	    
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
        attackTriggerReporter.OnTriggerEnterAction -= AttackCollisionEnter;
    }

    void AttackCollisionEnter(Collider _col)
    {
        if (_col.TryGetComponent(out Asteroid asteroid))
        {
            Vector3 pushVec = (_col.transform.position - transform.position).normalized;
            pushVec *= pushForce;
            asteroid.Push(pushVec);
	        asteroid.TakeDamage(damage);
	        CheckAndSpawnPunchVFX();
        }
        if (_col.TryGetComponent(out Health health))
        {
            if(health.GetAffiliation() == Affiliation.ENEMY)
            {
	            health.TakeDamage(damage);
	            CheckAndSpawnPunchVFX();
            }
        }
        if (_col.TryGetComponent(out Enemy enemy))
        {
            Vector3 pushVec = (_col.transform.position - transform.position).normalized;
            pushVec *= pushForce;
            enemy.Push(pushVec);
        }
    }
    
	void CheckAndSpawnPunchVFX()
	{
		Vector3 pos0 = transform.position;
		float dist = (attackCollider.transform.position - transform.position).magnitude + attackCollider.transform.localScale.x + 20f;
		Vector3 dir = (attackCollider.transform.position - transform.position).normalized;
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
		}
	}
    
	
	
    
	void AttackBehavior()
	{
		bool attackPressed = Input.GetMouseButton(0);
		if(attackPressed)
		{
			if(!attacking)
			{
				if(true)
				{
					attacking = true;
					isChargedAttack = false;
					attackCollider.SetActive(true);
					col.enabled = true;
				}
				//else if(playerController.IsSpeedDashing())
				//{
				//	//EXECUTE CHARGED ATTACK
				//	attacking = true;
				//	isChargedAttack = true;
				//	attackCollider.SetActive(true);
				//	col.enabled = true;
				//}
			}
		}
		
		if(attacking == true)
		{
			if(true)//NORMAL ATTACK
			{
				if(currentAttackTime < attackTime)
				{
					currentAttackTime += Time.deltaTime;
				}
				Vector3 initSize = new Vector3(initialAttackSize, initialAttackSize, initialAttackSize);
				Vector3 finalSize = new Vector3(finalAttackSize, finalAttackSize, finalAttackSize);
			
				if(currentAttackTime <= attackTime / 2f)//first part
				{
					float t = currentAttackTime / (attackTime / 2f);
					attackCollider.transform.position = Vector3.Lerp(transform.position, GetFrontAttackPosition(), 1f- ( 1f - t * t));
					attackCollider.transform.localScale = Vector3.Lerp(initSize, finalSize, 1f- ( 1f - t * t));
				}
				else if(currentAttackTime > attackTime / 2f && currentAttackTime < attackTime)//second part
				{
					float t = (currentAttackTime - (attackTime / 2f)) / (attackTime / 2f);
					attackCollider.transform.position = Vector3.Lerp(GetFrontAttackPosition(), transform.position, t * t);
					attackCollider.transform.localScale = Vector3.Lerp(finalSize, initSize, t * t);
				}
				else//attack finished
				{
					currentAttackTime = 0f;
					attacking = false;
					isChargedAttack = false;
					col.enabled = false;
					attackCollider.SetActive(false);
				}
			}
			//else if(isChargedAttack == true)//CHARGED ATTACK
			//{
			//	if(currentAttackTime < chargedAttackTime)
			//	{
			//		if(currentAttackTime >= chargedAttackFinishedTime)
			//		{
			//			isChargedAttackFinished = true;
			//		}
					
			//		currentAttackTime += Time.deltaTime;
			//	}
			//	Vector3 initSize = new Vector3(initialChargedAttackSize, initialChargedAttackSize, initialChargedAttackSize);
			//	Vector3 finalSize = new Vector3(finalChargedAttackSize, finalChargedAttackSize, finalChargedAttackSize);
			
			//	if(currentAttackTime <= chargedAttackTime / 2f)//first part
			//	{
			//		float t = currentAttackTime / (chargedAttackTime / 2f);
			//		attackCollider.transform.position = Vector3.Lerp(transform.position, GetFrontAttackPosition(), 1f- ( 1f - t * t));
			//		attackCollider.transform.localScale = Vector3.Lerp(initSize, finalSize, 1f- ( 1f - t * t));
			//	}
			//	else if(currentAttackTime > chargedAttackTime / 2f && currentAttackTime < chargedAttackTime)//second part
			//	{
			//		float t = (currentAttackTime - (chargedAttackTime / 2f)) / (chargedAttackTime / 2f);
			//		attackCollider.transform.position = Vector3.Lerp(GetFrontAttackPosition(), transform.position, t * t);
			//		attackCollider.transform.localScale = Vector3.Lerp(finalSize, initSize, t * t);
			//	}
			//	else//attack finished
			//	{
			//		currentAttackTime = 0f;
			//		attacking = false;
			//		isChargedAttack = false;
			//		isChargedAttackFinished = false;
			//		col.enabled = false;
			//		attackCollider.SetActive(false);
			//	}
			//}

		}
	}
	
	Vector3 GetFrontAttackPosition()
	{
		Vector3 hForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
		Vector3 perpendicular = Vector3.Cross(hForward, Vector3.up).normalized;
		hForward = Quaternion.AngleAxis(cameraController.GetCurrentTilt(), perpendicular) * hForward;
		
		if(!isChargedAttack)
		{
			hForward *= attackDistance;
		}
		else if(isChargedAttack)
		{
			hForward *= chargedAttackDistance;
		}
		
		hForward += transform.position;
		
		return hForward;
	}

    

    
}
