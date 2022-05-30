using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerMaxSpeed;
    [SerializeField] float playerDashMaxSpeed;
	[SerializeField] float playerAcceleration;
	[SerializeField] float playerDashAcceleration;
	[SerializeField] float playerDeceleration;
	[SerializeField] float ascensionSpeed;
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
	    enablerDisabler = GetComponent<DisablerUtility>();
	    
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
	    bool ascensionPressed = Input.GetKey(KeyCode.Space);

	    inputVec = inputVec.normalized;
        
	    dashing = dashPressed && inputVec.magnitude > 0.1f;

        relativeInput = Quaternion.LookRotation(camController.GetCameraForward(), camController.GetCameraUp()) * inputVec;
        relativeInput = relativeInput.normalized;

        if (inputVec != Vector3.zero)
        {
        	//ascension
        	Vector3 ascensionVec = Vector3.zero;
        	if(ascensionPressed)
        	{
        		Vector3 camForward = camController.GetCameraForward();
        		if(camForward.y >= 0f)//GO UP
        		{
        			ascensionVec = new Vector3(0f, ascensionSpeed, 0f);
        		}
        		else if(camForward.y < 0f)//GO DOWN
        		{
        			ascensionVec = new Vector3(0f, -ascensionSpeed, 0f);
        		}
        	}
        	
	        if (!canDash || !dashPressed || dashPressed && punchAttack.IsAttacking())//MOVEMENT WITHOUT DASH
            {
            	float realMaxSpeed = playerMaxSpeed;
		        if(punchAttack.IsAttacking())
            	{
            		realMaxSpeed *= speedWhileAttackingMultiplier;
            	}
		        
		        if(ascensionVec != Vector3.zero)
		        {
		        	rb.velocity = Vector3.Lerp(rb.velocity, ascensionVec * realMaxSpeed, playerAcceleration * Time.fixedDeltaTime);
		        }
		        else
		        {
		        	rb.velocity = Vector3.Lerp(rb.velocity, relativeInput * realMaxSpeed, playerAcceleration * Time.fixedDeltaTime);
		        }
            	
		        
            }
	        else if (canDash && dashPressed && !punchAttack.IsAttacking())//MOVEMENT WITH DASH
	        {
	        	if(ascensionVec != Vector3.zero)
	        	{
	        		rb.velocity = Vector3.Lerp(rb.velocity, ascensionVec * playerDashMaxSpeed, playerDashAcceleration * Time.fixedDeltaTime);
	        	}
	        	else
	        	{
	        		rb.velocity = Vector3.Lerp(rb.velocity, relativeInput * playerDashMaxSpeed, playerDashAcceleration * Time.fixedDeltaTime);
	        	}
		        
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
    
	#region TELEPORT
	
	[Header("Teleport")]
	[SerializeField] GameObject fadeLinesPrefab;
	[SerializeField] float teleportTime = 0.2f;
	float currentTeleportTime = 0f;
	
	DisablerUtility enablerDisabler;
	bool isTeleporting = false;
	public bool IsTeleporting(){return isTeleporting;}
	
	Transform targetTeleportTransform;
	Vector3 targetTeleportOffset;
	public void StartTeleportTo(Vector3 nTpOffset, Transform nTargetTr)
	{
		isTeleporting = true;
		targetTeleportOffset = nTpOffset;
		targetTeleportTransform = nTargetTr;
		enablerDisabler.SetThingsEnabled(false);
		
		GameObject nFadeLines = Instantiate(fadeLinesPrefab);
		Transform fadeLinesTr = nFadeLines.transform;
		fadeLinesTr.SetParent(null, false);
		fadeLinesTr.position = transform.position;
		
		StartCoroutine(TeleportCoroutine());
	}
	
	bool alreadyInstantiatedLines = false;
	public Action OnTeleportFinished;
	IEnumerator TeleportCoroutine()
	{
		yield return new WaitForEndOfFrame();
		
		if(currentTeleportTime >= teleportTime)//teleport is finished
		{
			isTeleporting = false;
			enablerDisabler.SetThingsEnabled(true);
			
			transform.position = targetTeleportTransform.position + targetTeleportOffset;
			
			currentTeleportTime = 0f;
			alreadyInstantiatedLines = false;
			
			OnTeleportFinished?.Invoke();
		}
		else if(currentTeleportTime < teleportTime)//teleport still ongoing
		{
			if(currentTeleportTime >= teleportTime / 2f && alreadyInstantiatedLines == false)
			{
				GameObject nFadeLines = Instantiate(fadeLinesPrefab);
				Transform fadeLinesTr = nFadeLines.transform;
				fadeLinesTr.SetParent(null, false);
				fadeLinesTr.position = targetTeleportTransform.position + targetTeleportOffset;
			}
			
			currentTeleportTime += Time.deltaTime;
			StartCoroutine(TeleportCoroutine());
		}
		
	}
	
	#endregion
    
	// Implement this OnDrawGizmosSelected if you want to draw gizmos only if the object is selected.
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.black;
		Gizmos.DrawRay(transform.position, new Vector3(0f, 2f, 0f));
	}


}
