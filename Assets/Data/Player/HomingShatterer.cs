using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody))]
public class HomingShatterer : MonoBehaviour
{
    Rigidbody rb;
    Transform target;
    [SerializeField] float acceleration;
    [MinValue(0f)][SerializeField] float lateralSpeedConservation;
    [SerializeField] float initialVerticalForce;
    [SerializeField] float damage;



    public void Shoot(Transform _target)
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(0f, initialVerticalForce, 0f), ForceMode.Impulse);
        target = _target;
        deleteCrt = StartCoroutine(DeleteCoroutine());
    }

    Coroutine deleteCrt;
    bool deleteCrtRunning = false;
    public IEnumerator DeleteCoroutine()
    {
        deleteCrtRunning = true;
        yield return new WaitForSeconds(6f);
        Destroy(this.gameObject);
        deleteCrtRunning = false;
    }

    void FixedUpdate()
    {
        MovementBehavior();
    }

    void MovementBehavior()
    {
        if(target != null)//target is not null
        {
            Vector3 vecToTarget = target.position - transform.position;
            rb.velocity = Vector3.Lerp(rb.velocity, vecToTarget.normalized * rb.velocity.magnitude, lateralSpeedConservation * Time.deltaTime);
            rb.velocity += vecToTarget.normalized * acceleration * Time.fixedDeltaTime;
        }
        else//target is null
        {
            Destroy(this.gameObject, 0.5f);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.GetContact(0).otherCollider.TryGetComponent(out Health health))
        {
            if(health.GetAffiliation() == Affiliation.ENEMY)
            {
                health.TakeDamage(damage);
            }
        }

        GetComponent<Collider>().enabled = false;
        Destroy(this.gameObject, 0.2f);
    }

    void OnDisable()
    {
        if(deleteCrtRunning)
        {
            StopCoroutine(deleteCrt);
        }
    }

}
