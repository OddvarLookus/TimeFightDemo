using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBullet : MonoBehaviour
{
	[MinValue(0)] [SerializeField] int damage;
	[SerializeField] float speed;
	[SerializeField] float deleteTime;
	Vector3 prevSpeed = Vector3.zero;
	[SerializeField] float knockback;
	
	[SerializeField] ForceMode magnetismForceMode;
	[MinValue(0f)] [SerializeField] float magnetism;
	
	
	Transform target;
	Rigidbody rb;
	
	protected void OnEnable()
	{
		rb = GetComponent<Rigidbody>();
		
		StartCoroutine(DeleteCoroutine());
	}
	
	protected void OnDisable()
	{
		StopAllCoroutines();
	}
    
	
	protected void FixedUpdate()
	{
		if(rb.velocity.sqrMagnitude >= 0.1f)
		{
			transform.forward = rb.velocity.normalized;
		}
		MagnetismBehavior();
		
	}
    
	IEnumerator DeleteCoroutine()
	{
		yield return new WaitForSeconds(deleteTime);
		Destroy(this.gameObject);
	}
	
	public void Shoot(Vector3 dir, Transform newTarget)
	{
		target = newTarget;
		rb.AddForce(dir * speed, ForceMode.Impulse);
		prevSpeed = dir * speed;
	}
    
	public void MagnetismBehavior()
	{
		if(magnetism > 0f)
		{
			Vector3 dir = (target.position - transform.position).normalized;
			rb.AddForce(dir * magnetism, magnetismForceMode);
		}
	}
    
	protected void OnCollisionEnter(Collision collisionInfo)
	{
		if(collisionInfo.gameObject.layer == 7)
		{
			collisionInfo.gameObject.GetComponent<Rigidbody>().AddForce(prevSpeed.normalized * knockback);
			if(collisionInfo.gameObject.TryGetComponent(out PlayerShield playerShield))
			{
				playerShield.TakeDamage(damage);
			}
		}
		
		GetComponent<Collider>().enabled = false;
		StopAllCoroutines();
		Destroy(this.gameObject);
		
	}
    
}
