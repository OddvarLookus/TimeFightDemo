using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerMaxSpeed;
    [SerializeField] float playerDashMaxSpeed;
	[SerializeField] float playerAcceleration;
	[SerializeField] float playerDashAcceleration;
    [SerializeField] float playerDeceleration;
	[SerializeField] float rotationSpeed;
	[SerializeField] float rotationForwardMultiplier;
	[SerializeField] CameraController camController;
	Rigidbody rb;
	[SerializeField] Attack punchAttack;
	[SerializeField] float speedWhileAttackingMultiplier;
    
	bool canDash = true;
	public bool CanDash(){return canDash;}
	public void SetCanDash(bool nCanDash){canDash = nCanDash;}
	
	bool dashing = false;
	public bool IsDashing(){return dashing;}
	bool prevDashing = false;
	
	bool speedDashing = false;
	public bool IsSpeedDashing(){return speedDashing;}

	bool movementEnabled = true;
	public bool GetMovementEnabled(){return movementEnabled;}
	public void SetMovementEnabled(bool nEnabled)
	{
		movementEnabled = nEnabled;
		if(nEnabled == false)
		{
			rb.isKinematic = true;
		}
	}

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
		if(movementEnabled)
		{
			Movement();
			RotationBehavior();
		}

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
        
	    inputVec.x += Input.GetAxis("MoveHorizontal");
	    inputVec.z += Input.GetAxis("MoveVertical");
        
	    bool dashPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetAxis("Dash") > 0f;
        

	    inputVec = inputVec.normalized;
        
	    dashing = dashPressed && inputVec.magnitude > 0.1f;

        relativeInput = Quaternion.LookRotation(camController.GetCameraForward(), camController.GetCameraUp()) * inputVec;
        relativeInput = relativeInput.normalized;

        if (inputVec != Vector3.zero)
        {
	        if (!canDash || !dashPressed || dashPressed && punchAttack.IsAttacking())//MOVEMENT WITHOUT DASH
            {
            	float realMaxSpeed = playerMaxSpeed;
		        if(punchAttack.IsAttacking())
            	{
            		realMaxSpeed *= speedWhileAttackingMultiplier;
            	}
		        
            	
		        rb.velocity = Vector3.Lerp(rb.velocity, relativeInput * realMaxSpeed, playerAcceleration * Time.fixedDeltaTime);
            }
	        else if (canDash && dashPressed && !punchAttack.IsAttacking())//MOVEMENT WITH DASH
            {
		        rb.velocity = Vector3.Lerp(rb.velocity, relativeInput * playerDashMaxSpeed, playerDashAcceleration * Time.fixedDeltaTime);
            }
        }
        else
        {
	        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, playerDeceleration * Time.fixedDeltaTime);
        }
        
	    ManageDashSounds();
	    prevDashing = dashing;
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
	
	[Header("DASH SOUNDS")]
	[SerializeField] AudioSource dashLoopAudio;
	
	void ManageDashSounds()
	{
		
		if(!dashing && prevDashing == true)
		{
			dashLoopAudio.Stop();
		}
		
		if(dashing && !dashLoopAudio.isPlaying)
		{
			dashLoopAudio.Play();
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
