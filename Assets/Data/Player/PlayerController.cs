using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerMaxSpeed;
	[SerializeField] float playerAcceleration;
	[SerializeField] float playerDeceleration;
	[SerializeField] float ascensionSpeed;
	[SerializeField] float rotationSpeed;
	[SerializeField] float rotationForwardMultiplier;
	[SerializeField] CameraController camController;
	Rigidbody rb;
	PlayerShield playerShield;
	[SerializeField] Attack punchAttack;
	[SerializeField] float speedWhileAttackingMultiplier;
    
	[Header("Dash")]
	[SerializeField] float dashPower;
	[SerializeField] float dashReleaseTime;
	float currentDashPressedTime = 0f;
	[SerializeField] float dashDuration;
	float currentDashTime = 0f;
	
	[Header("Speed Dash")]
	[SerializeField] float speedDashSpeed;
	[SerializeField] float speedDashAcceleration;
    
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
	    playerShield = GetComponent<PlayerShield>();
	    
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
	    if(movementEnabled)
	    {
		    InputBehavior();
		    DashBehavior();
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
    
	Vector3 inputVec;
	Vector3 relativeInput;
	bool ascensionPressed = false;
	bool dashJustPressed = false;
	bool dashPressed = false;
	bool dashReleased = false;
	void InputBehavior()
	{
		inputVec = Vector3.zero;
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
		ascensionPressed = Input.GetKey(KeyCode.Space);
		
		inputVec = inputVec.normalized;
		
		relativeInput = Quaternion.LookRotation(camController.GetCameraForward(), camController.GetCameraUp()) * inputVec;
		relativeInput = relativeInput.normalized;
		
		dashJustPressed = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetButtonDown("Dash");
		dashPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Dash");
		dashReleased = Input.GetKeyUp(KeyCode.LeftShift) || Input.GetButtonUp("Dash");
		
	}
    
	[HideInInspector] public float agilityBonus = 0f;
    void Movement()
    {
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
        	
        	if(!dashing)
        	{
        		if(speedDashing)//run
        		{
	        		float realMaxSpeed = speedDashSpeed + agilityBonus;
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
	        			if(inputVec.magnitude > 0f)//INPUT > 0
	        			{
	        				rb.velocity = Vector3.Lerp(rb.velocity, relativeInput * realMaxSpeed, speedDashAcceleration * Time.fixedDeltaTime);
	        			}
	        			else//NO INPUT
	        			{
		        			Vector3 transformFwXZ = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
		        			Vector3 perpendicular = Vector3.Cross(transformFwXZ, Vector3.up);
		        			transformFwXZ = Quaternion.AngleAxis(camController.GetCurrentTilt(), perpendicular) * transformFwXZ;
		        			transformFwXZ = transformFwXZ.normalized;
	        				rb.velocity = Vector3.Lerp(rb.velocity, transformFwXZ * realMaxSpeed, speedDashAcceleration * Time.fixedDeltaTime);
	        			}
		        		
	        		}
        		}
        		else if(!speedDashing)
        		{
	        		float realMaxSpeed = playerMaxSpeed + agilityBonus;
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

        	}

            
        }
        else if(inputVec == Vector3.zero && !dashing)
        {
	        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, playerDeceleration * Time.fixedDeltaTime);
        }
        
    }


	Vector3 prevRelativeInput = Vector3.zero;
	bool canRotate = true;
	float preventRotationTime = 0.7f;
	float currentPreventRotationTime = 0f;
    void RotationBehavior()
	{
		float addRot = rb.velocity.magnitude * rotationForwardMultiplier;
		addRot = Mathf.Clamp(addRot, 0f, 45f);
		Quaternion addQuat = Quaternion.Euler(addRot, 0f, 0f);
    	
		if(canRotate)
		{
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
		else if(!canRotate)
		{
			currentPreventRotationTime += Time.fixedDeltaTime;
			if(currentPreventRotationTime >= preventRotationTime)
			{
				canRotate = true;
				currentPreventRotationTime = 0f;
			}
		}

        
	}

    
	void DashBehavior()
	{
		if(!punchAttack.IsAttacking())
		{
			bool shouldDash = false;
			//CALCULATE DASH TIMES
			if(!dashing)
			{
				if(dashPressed)
				{
					currentDashPressedTime += Time.deltaTime;
			
					if(currentDashPressedTime > dashReleaseTime && !speedDashing && inputVec.magnitude > 0f)
					{
						speedDashing = true;
					}
				}
				if(dashReleased)
				{
					if(currentDashPressedTime <= dashReleaseTime)
					{
						shouldDash = true;
					}
					currentDashPressedTime = 0f;
					speedDashing = false;
				}
			}
			else//I'M DASHING
			{
				currentDashTime += Time.deltaTime;
				
				if(currentDashTime >= dashDuration)
				{
					dashing = false;
					currentDashPressedTime = 0f;
					currentDashTime = 0f;
					speedDashing = false;
				}
			}

		
		
			//DO DASH
			//only do dash while not attacking
		
			if(shouldDash && !dashing && !speedDashing)//not dashing
			{
				dashing = true;
				Vector3 dashDir = Vector3.zero;
				if(inputVec.magnitude > 0)
				{
					dashDir = relativeInput.normalized;
				}
				else
				{
					Vector3 transformFwXZ = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
					Vector3 perpendicular = Vector3.Cross(transformFwXZ, Vector3.up);
					transformFwXZ = Quaternion.AngleAxis(camController.GetCurrentTilt(), perpendicular) * transformFwXZ;
					dashDir = transformFwXZ.normalized;
				}
		
				//execute dash
				rb.AddForce(dashDir * dashPower, ForceMode.Impulse);
			
			}
			else if(speedDashing)
			{
		
			}
		}
		else//during attack reset dash
		{
			dashing = false;
			currentDashPressedTime = 0f;
			currentDashTime = 0f;
			speedDashing = false;
		}

		
		prevDashing = dashing;
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
		enablerDisabler.SetThingAtIdxEnabled(3, false);
		
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
			enablerDisabler.SetThingAtIdxEnabled(3, playerShield.GetCurrentShield() > 1);
			
			if(targetTeleportTransform != null)
			{
				Vector3 vecToEnemy = (targetTeleportTransform.position - transform.position).normalized;
				relativeInput = vecToEnemy;
				prevRelativeInput = vecToEnemy;
				rb.rotation = Quaternion.LookRotation(vecToEnemy, Vector3.up);
				canRotate = false;
			
				transform.position = targetTeleportTransform.position + targetTeleportOffset;
			}

			
			currentTeleportTime = 0f;
			alreadyInstantiatedLines = false;
			
			OnTeleportFinished?.Invoke();
			
		}
		else if(currentTeleportTime < teleportTime)//teleport still ongoing
		{
			if(currentTeleportTime >= teleportTime / 2f && alreadyInstantiatedLines == false)
			{
				if(targetTeleportTransform != null)
				{
					GameObject nFadeLines = Instantiate(fadeLinesPrefab);
					Transform fadeLinesTr = nFadeLines.transform;
					fadeLinesTr.SetParent(null, false);
					fadeLinesTr.position = targetTeleportTransform.position + targetTeleportOffset;
					alreadyInstantiatedLines = true;
				}

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
