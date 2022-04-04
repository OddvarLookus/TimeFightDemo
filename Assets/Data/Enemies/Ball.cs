using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Enemy
{

    [Header("Movement")]
    [SerializeField] float minMoveTime;
    [SerializeField] float maxMoveTime;
    float moveTime;
    float curMoveTime = 0f;
    [SerializeField] float minMoveForce;
    [SerializeField] float maxMoveForce;
    [SerializeField] float rotationSpeed;

    protected override void OnEnable()
    {
        base.OnEnable();
        InitializeTime();
    }


    void FixedUpdate()
    {
        if(curMoveTime < moveTime)
        {
            curMoveTime += Time.fixedDeltaTime;
        }
        else//time to move
        {
            Hop();
            InitializeTime();
        }
        RotateTowardsMovement();
    }

    void InitializeTime()
    {
        moveTime = Random.Range(minMoveTime, maxMoveTime);
        curMoveTime = 0f;
    }

    void Hop()
    {
        Vector3 force = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        force *= Mathf.Lerp(minMoveForce, maxMoveForce, Random.Range(0f, 1f));
        rb.AddForce(force, ForceMode.Impulse);
    }

    void RotateTowardsMovement()
    {
        if(rb.velocity.sqrMagnitude >= 0.1f)
        {
            Vector3 newForward = transform.forward;
            newForward = Vector3.Lerp(newForward.normalized, rb.velocity.normalized, rotationSpeed * Time.fixedDeltaTime).normalized;
            transform.forward = newForward;
        }

    }


}
