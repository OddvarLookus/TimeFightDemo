using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerMaxSpeed;
    [SerializeField] float playerDashMaxSpeed;
    [SerializeField] float playerAcceleration;
    [SerializeField] float playerDeceleration;
    [SerializeField] CameraController camController;
    Rigidbody rb;
    Attack attack;
    [SerializeField] float attackCooldown;
    float currentAttackTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        attack = GetComponent<Attack>();
    }

    private void Update()
    {
        Movement();
        RotationBehavior();
        AttackBehavior();
        
    }


    private void FixedUpdate()
    {

    }

    void Movement()
    {
        Vector3 inputVec = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            inputVec.z += 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVec.z -= 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVec.x -= 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVec.x += 1f;
        }
        bool dashPressed = Input.GetKey(KeyCode.LeftShift);


        inputVec = inputVec.normalized;
        Vector3 relativeInput = Quaternion.LookRotation(camController.GetCameraForward(), camController.GetCameraUp()) * inputVec;
        relativeInput = relativeInput.normalized;
        //relativeInput = Vector3.ProjectOnPlane(relativeInput, Vector3.up).normalized;

        if (inputVec != Vector3.zero)
        {
            if (!dashPressed)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, relativeInput * playerMaxSpeed, playerAcceleration * Time.deltaTime);
            }
            else if (dashPressed)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, relativeInput * playerDashMaxSpeed, playerAcceleration * Time.deltaTime);
            }
        }
        else
        {
            rb.velocity = Vector3.Lerp(rb.velocity, relativeInput * playerMaxSpeed, playerDeceleration * Time.deltaTime);
        }
        

        //rb.AddForce(relativeInput * playerAcceleration, ForceMode.Acceleration);
        //Vector3 hVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //if (hVel.magnitude > playerMaxSpeed)
        //{
        //    hVel = Vector3.ClampMagnitude(hVel, playerMaxSpeed);
        //    rb.velocity = new Vector3(hVel.x, rb.velocity.y, hVel.z);
        //}
        //if (inputVec == Vector3.zero)
        //{
        //    rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, playerDeceleration * Time.deltaTime);
        //}
    }

    void AttackBehavior()
    {
        bool pressingAttack = Input.GetMouseButtonDown(0);
        
        if (currentAttackTime < attackCooldown)//attack on cooldown
        {
            currentAttackTime += Time.deltaTime;
        }
        else//attack not on cooldown
        {
            if (pressingAttack)
            {
                attack.PerformAttack();
                currentAttackTime = 0f;
            }
        }
        //Debug.Log(currentAttackTime);

    }

    void RotationBehavior()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(rb.velocity.normalized, Vector3.up), transform.up);
    }

}
