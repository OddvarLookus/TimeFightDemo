using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

//ENEMY HAS THE GENERIC THINGS FOR ALL ENEMIES. 
public class Enemy : MonoBehaviour
{
    protected Rigidbody rb;

    protected Health health;
    public Affiliation GetHealthAffiliation()
    {
        return health.GetAffiliation();
    }

    [SerializeField] protected Renderer mainRenderer;

    [Header("Size")]
    [SerializeField] float minSize;
	[SerializeField] float maxSize;
    
	protected EnemyAggroState aggroState = EnemyAggroState.NEUTRAL;

    public Renderer GetRenderer()
    {
        return mainRenderer;
    }

    protected virtual void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();

        InitializeEnemy();
    }

    void InitializeEnemy()
    {
        float factor = Random.Range(0f, 1f);
        health.Initialize(factor);
        float size = Mathf.Lerp(minSize, maxSize, factor);
        transform.localScale = new Vector3(size, size, size);
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
	
    public void Push(Vector3 _pushForce)
    {
        rb.AddForce(_pushForce, ForceMode.Impulse);
    }


    
    //GIZMOS
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxSize);
    }
}

public enum EnemyAggroState {NEUTRAL = 0, AGGRO = 1}

