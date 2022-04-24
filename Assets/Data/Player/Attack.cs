using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] GameObject attackCollider;
    Collider col;
	TriggerReporter attackTriggerReporter;
	[SerializeField] CameraController cameraController;
    [SerializeField] float pushForce;
	public float baseDamage;
	float damage;
	
	
	[Header("Attack Dynamics")]
	[SerializeField] float attackDistance;
	[SerializeField] float attackSpeed;
	float attackTime;
	float currentAttackTime = 0f;
	bool attacking = false;
	
	[SerializeField] float attackSize;
	
	
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
        
	    attackTime = 1f / attackSpeed;
    }
	
	protected void Update()
	{
		AttackBehavior();
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
        }
        if (_col.TryGetComponent(out Health health))
        {
            if(health.GetAffiliation() == Affiliation.ENEMY)
            {
                health.TakeDamage(damage);
            }
        }
        if (_col.TryGetComponent(out Enemy enemy))
        {
            Vector3 pushVec = (_col.transform.position - transform.position).normalized;
            pushVec *= pushForce;
            enemy.Push(pushVec);
        }
    }
    
	void AttackBehavior()
	{
		bool attackPressed = Input.GetMouseButton(0);
		if(attackPressed)
		{
			if(!attacking)
			{
				attacking = true;
				attackCollider.SetActive(true);
				col.enabled = true;
				
			}
		}
		
		if(attacking == true)
		{
			if(currentAttackTime < attackTime)
			{
				currentAttackTime += Time.deltaTime;
			}
			
			
			if(currentAttackTime <= attackTime / 2f)//first part
			{
				float t = currentAttackTime / (attackTime / 2f);
				attackCollider.transform.position = Vector3.Lerp(transform.position, GetFrontAttackPosition(), 1f- ( 1f - t * t));
				attackCollider.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(attackSize, attackSize, attackSize), 1f- ( 1f - t * t));
			}
			else if(currentAttackTime > attackTime / 2f && currentAttackTime < attackTime)//second part
			{
				float t = (currentAttackTime - (attackTime / 2f)) / (attackTime / 2f);
				attackCollider.transform.position = Vector3.Lerp(GetFrontAttackPosition(), transform.position, t * t);
				attackCollider.transform.localScale = Vector3.Lerp(new Vector3(attackSize, attackSize, attackSize), Vector3.zero, t * t);
			}
			else//attack finished
			{
				currentAttackTime = 0f;
				attacking = false;
				col.enabled = false;
				attackCollider.SetActive(false);
			}
		}
	}
	
	Vector3 GetFrontAttackPosition()
	{
		Vector3 hForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
		Vector3 perpendicular = Vector3.Cross(hForward, Vector3.up).normalized;
		hForward = Quaternion.AngleAxis(cameraController.GetCurrentTilt(), perpendicular) * hForward;
		
		hForward *= attackDistance;
		hForward += transform.position;
		
		return hForward;
	}

    

    IEnumerator DisableCoroutine(float _time)
    {
	    yield return new WaitForSeconds(1f);
    }

    
}
