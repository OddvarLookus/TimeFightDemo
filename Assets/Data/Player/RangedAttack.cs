using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RangedAttack : MonoBehaviour
{
    [SerializeField] float maxRange;
    [MinValue(0)][SerializeField] int maxTargetsNum;

    [SerializeField] GameObject rangedProjectilePrefab;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AttackInput();
    }

    void AttackInput()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, maxRange, 1<<6, QueryTriggerInteraction.Ignore);
            if(enemies.Length <= maxTargetsNum)//enemies in range are equal or less to the max number of targets
            {
                for(int i = 0; i < enemies.Length; i++)
                {
                    ShootProjectile(enemies[i].transform);
                }
            }
            else//enemies in range are more than max number of targets
            {
                for(int i = 0; i < maxTargetsNum; i++)
                {
                    ShootProjectile(enemies[i].transform);
                }

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
