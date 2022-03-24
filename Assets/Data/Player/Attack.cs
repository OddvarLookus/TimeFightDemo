using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] GameObject attackCollider;
    Collider col;
    TriggerReporter attackTriggerReporter;
    [SerializeField] Animator attackAnimator;
    [SerializeField] float pushForce;
    [SerializeField] float damage;

    private void Awake()
    {
        col = attackCollider.GetComponent<Collider>();
        attackTriggerReporter = attackCollider.GetComponent<TriggerReporter>();
        attackTriggerReporter.OnTriggerEnterAction += AttackCollisionEnter;
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
            health.TakeDamage(damage);
        }
        if (_col.TryGetComponent(out Enemy enemy))
        {
            Vector3 pushVec = (_col.transform.position - transform.position).normalized;
            pushVec *= pushForce;
            enemy.Push(pushVec);
        }
    }

    public void PerformAttack()
    {
        col.enabled = true;
        attackAnimator.Play("AttackAnim", 0, 0f);
        float attackTime = attackAnimator.GetCurrentAnimatorClipInfo(0).Length;
        StartCoroutine(DisableCoroutine(attackTime));
    }

    IEnumerator DisableCoroutine(float _time)
    {
        yield return new WaitForSeconds(_time);
        col.enabled = false;

    }

    
}
