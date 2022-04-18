using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBullet : MonoBehaviour
{
	[SerializeField] float speed;
	[SerializeField] float deleteTime;
	Vector3 prevSpeed = Vector3.zero;
	[SerializeField] float knockback;
	
	
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
		
	}
    
	IEnumerator DeleteCoroutine()
	{
		yield return new WaitForSeconds(deleteTime);
		Destroy(this.gameObject);
	}
	
	public void Shoot(Vector3 dir)
	{
		rb.AddForce(dir * speed, ForceMode.Impulse);
		prevSpeed = dir * speed;
	}
    
    
	protected void OnCollisionEnter(Collision collisionInfo)
	{
		if(collisionInfo.gameObject.layer == 7)
		{
			collisionInfo.gameObject.GetComponent<Rigidbody>().AddForce(prevSpeed.normalized * knockback);
		}
	}
    
}
