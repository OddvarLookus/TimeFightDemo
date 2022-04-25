using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerMaxSpeed;
    [SerializeField] float playerDashMaxSpeed;
    [SerializeField] float playerAcceleration;
    [SerializeField] float playerDeceleration;
	[SerializeField] float rotationSpeed;
	[SerializeField] float rotationForwardMultiplier;
	[SerializeField] CameraController camController;
	Rigidbody rb;
	[SerializeField] Attack punchAttack;
	[SerializeField] float speedWhileAttackingMultiplier;
    
	bool speedDashing = false;
	public bool IsSpeedDashing(){return speedDashing;}


    private void Awake()
    {
	    rb = GetComponent<Rigidbody>();
	    
	    if(punchAttack == null)
	    {
	    	punchAttack = GetComponent<Attack>();
	    }

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        
	    

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }


    private void FixedUpdate()
	{
		Movement();
		RotationBehavior();
    }

    Vector3 relativeInput;
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

        relativeInput = Quaternion.LookRotation(camController.GetCameraForward(), camController.GetCameraUp()) * inputVec;
        relativeInput = relativeInput.normalized;

        if (inputVec != Vector3.zero)
        {
	        if (!dashPressed || dashPressed && punchAttack.IsAttacking())//MOVEMENT WITHOUT DASH
            {
            	float realMaxSpeed = playerMaxSpeed;
		        if(punchAttack.IsAttacking())/* && !punchAttack.IsChargedAttack()*/
            	{
            		realMaxSpeed *= speedWhileAttackingMultiplier;
            	}
		        //else if(punchAttack.IsAttacking() && punchAttack.IsChargedAttack() && punchAttack.IsChargedAttackFinished())
		        //{
		        //	realMaxSpeed *= speedWhileAttackingMultiplier;
		        //}
            	
	            rb.velocity = Vector3.Lerp(rb.velocity, relativeInput * realMaxSpeed, playerAcceleration * Time.deltaTime);
            }
	        else if (dashPressed && !punchAttack.IsAttacking())//MOVEMENT WITH DASH
            {
                rb.velocity = Vector3.Lerp(rb.velocity, relativeInput * playerDashMaxSpeed, playerAcceleration * Time.deltaTime);
            }
        }
        else
        {
	        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, playerDeceleration * Time.deltaTime);
        }
        
        
	    speedDashing = rb.velocity.magnitude > 150f;
    }


    Vector3 prevRelativeInput = Vector3.zero;
    void RotationBehavior()
	{
		float addRot = rb.velocity.magnitude * rotationForwardMultiplier;
		addRot = Mathf.Clamp(addRot, 0f, 45f);
		Quaternion addQuat = Quaternion.Euler(addRot, 0f, 0f);
    	
        if (relativeInput.sqrMagnitude > 0.3f)
        {
	        Quaternion targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(relativeInput.normalized, Vector3.up), Vector3.up) * addQuat;
	        rb.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
            
            prevRelativeInput = relativeInput;
        }
        else
        {
	        Quaternion targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(prevRelativeInput.normalized, Vector3.up), Vector3.up) * addQuat;
	        rb.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        }
        
		
		
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }
    
	// Implement this OnDrawGizmosSelected if you want to draw gizmos only if the object is selected.
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.black;
		Gizmos.DrawRay(transform.position, new Vector3(0f, 2f, 0f));
	}


}
