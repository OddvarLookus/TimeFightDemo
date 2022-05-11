using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RangedAttack : MonoBehaviour
{
    [SerializeField] float maxRange;
	[MinValue(0)][SerializeField] int maxTargetsNum;
	[MinValue(0f)] [SerializeField] float cooldown;
	float currentCooldown;

	bool attackEnabled = true;
	public bool GetAttackEnabled(){return attackEnabled;}
	public void SetAttackEnabled(bool nEnabled){attackEnabled = nEnabled;}

	[SerializeField] GameObject rangedProjectilePrefab;
    
	public int baseBulletsPerTarget = 1;
	int bulletsPerTarget;
	public void SetBulletsPerTarget(int newBulPerTarg)
	{
		bulletsPerTarget = newBulPerTarg;
	}
	
	int bulletsShot = 0;
	[SerializeField] float betweenBulletTime = 0.1f;
	float currentBetweenShotTime = 0f;
	
	

	
	protected void OnEnable()
	{
		currentCooldown = cooldown;
	}

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
	{
		if(attackEnabled)
		{
			AttackInput();
		}
        
    }
	
	Collider[] enemies = new Collider[1];
    void AttackInput()
	{
		if(currentCooldown >= cooldown)
		{
			if(Input.GetMouseButtonDown(1) || Input.GetButtonDown("RangedAttack"))
			{
				enemies = Physics.OverlapSphere(transform.position, maxRange, 1<<6, QueryTriggerInteraction.Ignore);
				
				currentCooldown = 0f;
				for(int i = 0; i < Mathf.Min(maxTargetsNum, enemies.Length); i++)
				{
					ShootProjectile(enemies[i].transform);
				}
				bulletsShot = 1;
				currentBetweenShotTime = 0f;
				
			}
		}
		else
		{
			currentCooldown += Time.deltaTime;
			
			currentBetweenShotTime += Time.deltaTime;
			if(bulletsShot < bulletsPerTarget && currentBetweenShotTime > betweenBulletTime)
			{
				for(int i = 0; i < Mathf.Min(maxTargetsNum, enemies.Length); i++)
				{
					if(enemies[i] != null)
					{
						ShootProjectile(enemies[i].transform);
					}
					
				}
				currentBetweenShotTime = 0f;
				bulletsShot += 1;
			}
			
		}

    }


    void ShootProjectile(Transform _target)
    {
        GameObject proj = Instantiate(rangedProjectilePrefab);
        proj.transform.SetParent(null, true);
        proj.transform.position = transform.position;
        proj.GetComponent<HomingShatterer>().Shoot(_target);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }
}
