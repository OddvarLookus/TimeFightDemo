﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    enum CameraMode {FREELOOK = 0, ENEMYLOCK = 1}
    CameraMode cameraMode = CameraMode.FREELOOK;

    [Header("Camera movement configuration")]
    [SerializeField] Transform cameraTransform;
    [Range(1f, 15f)]
    [SerializeField] float maxCameraDist;
    [SerializeField] Transform cameraLookTarget;
    [Range(-70f, 0f)]
    [SerializeField] float minCameraTilt;
    [Range(0f, 70f)]
    [SerializeField] float maxCameraTilt;
    [SerializeField] LayerMask cameraRaycastLayers;
    [SerializeField] float cameraRayHitOffset;
    float cameraRot = 0f;
    float cameraTilt = 0f;
    [Header("Invert")]
    [SerializeField] bool invertX;
    [SerializeField] bool invertY;
    [Header("Smoothing")]
    [Range(0.1f, 3f)]
    [SerializeField] float cameraSpeed;
    [SerializeField] float smoothingSpeed;
    float targetRot;
    float targetTilt;

    //DISTANCE OF CAMERA BASED ON PLAYER SPEED
    [SerializeField] PlayerController playerController;
    [SerializeField] float minSpeedCameraDist, maxSpeedCameraDist;
    [SerializeField] float maxPlayerSpeed;
	//disable dash when fast next to the locked enemy
	[SerializeField] float cameraDashMinDistance;
	[SerializeField] float enemyLockerSpeed;
	//garpa size
	[SerializeField] float garpaSizeAddition;
	[SerializeField] float minGarpaDistance;
	[SerializeField] float garpaRotSpeed = 100f;
	

    #region MONOBEHAVIOR
    private void Awake()
    {
	    targetCameraPos = transform.position;
	    StartCoroutine(LateFixedUpdate());
    }
    private void Update()
    {
        LockInputCheck();
	    
	    if(!GameManager.instance.IsGamePaused())
	    {
		    if (cameraMode == CameraMode.FREELOOK)
		    {
			    CameraLookBehavior();
			    CameraDistanceBySpeed();
			    CameraPositionBehavior();
			    EnemyLockerBehavior();
			    
			    //cameraTransform.LookAt(cameraLookTarget, Vector3.up);
		    }
		    else if (cameraMode == CameraMode.ENEMYLOCK)
		    {
			    CameraLockBehavior();
			    EnemyLockerBehavior();
			    CameraPositionBehavior();
			    
			    //cameraTransform.LookAt(lockedEnemy, Vector3.up);
		    }
		    
		    
	    }
	    
    }
	
	protected void LateUpdate()
	{
		LookAtTarget();
		
	}
	
    private void FixedUpdate()
    {

	    
    }
    
	IEnumerator LateFixedUpdate()
	{
		yield return new WaitForFixedUpdate();
		
		
		
		StartCoroutine(LateFixedUpdate());
	}
	
	protected void OnDisable()
	{
		StopAllCoroutines();
	}
    
	void LookAtTarget()
	{
		Quaternion rot = transform.rotation;
		Quaternion desiredRot = Quaternion.identity;
		
		if(cameraMode == CameraMode.FREELOOK)
		{
			//Vector3 vecToTarget = (cameraLookTarget.position - cameraTransform.position).normalized;
			//Vector3 perpendicular = Vector3.Cross(vecToTarget, Vector3.up);
			//Vector3 upPerpendicular = Vector3.Cross(vecToTarget, perpendicular);
			//desiredRot = Quaternion.LookRotation(vecToTarget, upPerpendicular);
			
			cameraTransform.LookAt(cameraLookTarget, Vector3.up);
			//cameraTransform.LookAt(cameraLookTarget, Vector3.up);
		}
		else if(cameraMode == CameraMode.ENEMYLOCK)
		{
			//Vector3 vecToTarget = (cameraLookTarget.position - lockedEnemy.position).normalized;
			//Vector3 perpendicular = Vector3.Cross(vecToTarget, Vector3.up);
			//Vector3 upPerpendicular = Vector3.Cross(vecToTarget, perpendicular);
			//desiredRot = Quaternion.LookRotation(vecToTarget, upPerpendicular);
			
			
			cameraTransform.LookAt(/*enemyLocker*/lockedEnemy, Vector3.up);
		}
		
		
		//cameraTransform.rotation = Quaternion.Lerp(rot, cameraTransform.rotation, 100f * Time.fixedDeltaTime);
	}


    #endregion
    #region CAMERA ROTATION
    Vector3 normalizedCameraPos;
    void CameraRotationBehavior()
    {

        //X ROTATION
        if (!invertX == false)
        {
	        targetRot += (Input.GetAxisRaw("Mouse X") + Input.GetAxis("Horizontal")) * cameraSpeed;
        }
        else
        {
	        targetRot -= (Input.GetAxisRaw("Mouse X") + Input.GetAxis("Horizontal")) * cameraSpeed;
        }

        float t;
        if (smoothingSpeed <= 0f)
        {
            t = 1f;
        }
        else
        {
            t = smoothingSpeed * Time.deltaTime;
        }

        cameraRot = Mathf.Lerp(cameraRot, targetRot, t);

    }
    void CameraTiltBehavior()
    {
        //Vector3 playerToCam = cameraTransform.position - cameraLookTarget.position;
        if (invertY == false)
        {
	        targetTilt += (Input.GetAxisRaw("Mouse Y") + Input.GetAxis("Vertical")) * cameraSpeed;
        }
        else
        {
	        targetTilt -= (Input.GetAxisRaw("Mouse Y") + Input.GetAxis("Vertical")) * cameraSpeed;
        }

        targetTilt = Mathf.Clamp(targetTilt, minCameraTilt, maxCameraTilt);

        float t;
        if (smoothingSpeed <= 0f)
        {
            t = 1f;
        }
        else
        {
            t = smoothingSpeed * Time.deltaTime;
        }
        cameraTilt = Mathf.Lerp(cameraTilt, targetTilt, t);

    }
    Quaternion alignment = Quaternion.identity;
    Vector3 targetCameraPos;
    void CameraPositionBehavior()
    {
        alignment = Quaternion.FromToRotation(alignment * Vector3.up, Vector3.up) * alignment;

        Quaternion finalRot = alignment * Quaternion.Euler(-cameraTilt, cameraRot, 0f);

	    if(cameraMode == CameraMode.FREELOOK)
	    {
	    	targetCameraPos = finalRot * Vector3.forward * maxCameraDist + cameraLookTarget.position;
	    }

        float t;
        if (smoothingSpeed <= 0f)
        {
            t = 1f;
        }
        else
        {
            t = smoothingSpeed * Time.deltaTime;
        }
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetCameraPos, t);
    }

    //used to recalculate camera position after removing lock
    void ConvertCameraPosAfterLock()
    {
        if (transform.eulerAngles.x >= 270f)
        {
            targetRot = transform.eulerAngles.y + 180f;
            targetTilt = transform.eulerAngles.x - 360f;
        }
        else
        {
            targetRot = transform.eulerAngles.y + 180f;
            targetTilt = transform.eulerAngles.x;
        }
    }

    void KeepCameraDistance()
    {
        Vector3 playerToCam = cameraTransform.position - cameraLookTarget.position;
        playerToCam = playerToCam.normalized * maxCameraDist;

        RaycastHit rhit;
        if (Physics.Raycast(cameraLookTarget.position, playerToCam, out rhit, maxCameraDist, cameraRaycastLayers.value, QueryTriggerInteraction.Ignore))
        {
            cameraTransform.position = rhit.point + (-playerToCam.normalized * cameraRayHitOffset);
            return;
        }
        cameraTransform.position = cameraLookTarget.position + playerToCam;
    }

    void CameraDistanceBySpeed()
    {
        float playerSpeed = Mathf.Clamp(playerController.GetVelocity().magnitude, 0f, maxPlayerSpeed);
        
        float speedT = playerSpeed / maxPlayerSpeed;
        float camDist = Mathf.Lerp(minSpeedCameraDist, maxSpeedCameraDist, speedT);
        maxCameraDist = camDist;
    }

    void CameraLookBehavior()
    {
        CameraRotationBehavior();
        CameraTiltBehavior();

    }


    #endregion
    #region LOCKING

    [Header("Locking")]
    [SerializeField] float lockingDistance;
    [SerializeField] Transform enemyLocker;

    List<Transform> lockableEnemies = new List<Transform>();
    Transform lockedEnemy;
    bool isLocking = false;

    //input check
    public void LockInputCheck()
    {
	    if (Input.GetMouseButtonDown(2) || Input.GetButtonDown("Lock"))
        {
            if (cameraMode == CameraMode.FREELOOK)
            {
                CheckLockableEnemies();
                isLocking = GetLockedEnemy();
                if (isLocking)
                {
                    cameraMode = CameraMode.ENEMYLOCK;
                }
            }
            else if (cameraMode == CameraMode.ENEMYLOCK)
            {
                ConvertCameraPosAfterLock();
                lockedEnemy = null;
                isLocking = false;
	            cameraMode = CameraMode.FREELOOK;
	            playerController.SetCanDash(true);
                
            }

        }
    }

    //gets all the objects that are lockable by the camera
    public void CheckLockableEnemies()
    {
        //get the colliders inside the distance for the camera
        Collider[] colliders;
        colliders = Physics.OverlapSphere(transform.position, lockingDistance, 1<<6, QueryTriggerInteraction.Ignore);

        //filter the colliders in front of the camera (max 90 degrees range)
        List<Transform> enemiesInFront = new List<Transform>();
        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 vecToCol = colliders[i].transform.position - transform.position;
            vecToCol = vecToCol.normalized;
            if (Vector3.Dot(vecToCol, transform.forward) > 0.2f)
            {
                enemiesInFront.Add(colliders[i].transform);
            }
        }

        //filter the colliders if they have a Enemy component
        for (int i = enemiesInFront.Count - 1; i >= 0; i--)
        {
            if (!enemiesInFront[i].TryGetComponent(out Enemy enemyComp))
            {
                enemiesInFront.RemoveAt(i);
            }
        }

        //filter the colliders inside the camera frustum
        for (int i = enemiesInFront.Count - 1; i >= 0; i--)
        {
            if (!enemiesInFront[i].GetComponent<Enemy>().GetRenderer().isVisible)
            {
                enemiesInFront.RemoveAt(i);
            }
        }

        lockableEnemies.Clear();
        lockableEnemies = enemiesInFront;
    }

    public bool GetLockedEnemy()
    {
        if (lockableEnemies.Count > 0)
        {
            float distanceFactor = float.MinValue;
            int idx = int.MaxValue;

            for (int i = 0; i < lockableEnemies.Count; i++)
            {
                Vector3 vecCamToEnemy = lockableEnemies[i].position - transform.position;
                Vector3 vecPlayerToEnemy = lockableEnemies[i].position - playerController.transform.position;
                float dist = vecPlayerToEnemy.magnitude;
                float dot = Vector3.Dot(vecCamToEnemy.normalized, transform.forward);
                float dFactor = (dot * 1.8f) + (1f - (dist / lockingDistance));
                if (dFactor > distanceFactor)
                {
                    distanceFactor = dFactor;
                    idx = i;
                }
            }

            lockedEnemy = lockableEnemies[idx];
            return true;
        }
        return false;
    }

    void CameraLockBehavior()
    {
        if (lockedEnemy == null)
        {
        	playerController.SetCanDash(true);
            CheckLockableEnemies();
            isLocking = GetLockedEnemy();
            if (isLocking)
            {
                cameraMode = CameraMode.ENEMYLOCK;
            }
            else
            {
                ConvertCameraPosAfterLock();
                lockedEnemy = null;
                cameraMode = CameraMode.FREELOOK;
                
	            return;
            }
        }

	    Vector3 vecToLockedEnemy = lockedEnemy.position/*enemyLocker.position*/ - playerController.transform.position;
	    //when player is very fast and near to the locked enemy disable dash
        if (vecToLockedEnemy.magnitude <= cameraDashMinDistance)
        {
	        playerController.SetCanDash(false);
        }
        else
        {
        	playerController.SetCanDash(true);
        }


        Vector3 negative = playerController.transform.position + ((-vecToLockedEnemy).normalized * 8f);

        negative += new Vector3(0f, 2f, 0f);
	    targetCameraPos = negative;
	    //cameraTransform.position = negative;
        
    }
    
	Vector3 targetGarpaPos = Vector3.zero;
	bool garpaNextToPlayer = false;
	void EnemyLockerBehavior()
	{
		if(enemyLocker != null)
		{
			if(cameraMode == CameraMode.ENEMYLOCK)
			{
				if((enemyLocker.position - lockedEnemy.position).magnitude < 5f)
				{
					float garpaMadArea = lockedEnemy.localScale.x + garpaSizeAddition;
					Vector3 randGarpaPos = new Vector3(Random.Range(-garpaMadArea, garpaMadArea), Random.Range(-garpaMadArea, garpaMadArea), Random.Range(-garpaMadArea, garpaMadArea));
					randGarpaPos = Vector3.ClampMagnitude(randGarpaPos, garpaMadArea);
					
					enemyLocker.position = lockedEnemy.position + randGarpaPos;
				}
				else
				{
					enemyLocker.position = Vector3.Lerp(enemyLocker.position, lockedEnemy.position, enemyLockerSpeed * Time.deltaTime);
				}
				
			}
			else if(cameraMode == CameraMode.FREELOOK)
			{
				//get nearest position in a radius around the player to position the garpa
				Vector3 nearestPos = (enemyLocker.position - playerController.transform.position).normalized;
				nearestPos *= minGarpaDistance;
				nearestPos += playerController.transform.position;
				
				//when garpa is next to the player, rotate around the player
				float distGarpaToPlayer = (enemyLocker.position - playerController.transform.position).magnitude;
				if(distGarpaToPlayer <= minGarpaDistance + 0.5f)
				{
					if(garpaNextToPlayer == false)
					{
						targetGarpaPos = enemyLocker.position - playerController.transform.position;
						garpaNextToPlayer = true;
					}
					
					targetGarpaPos = targetGarpaPos.normalized * minGarpaDistance;
					targetGarpaPos = Quaternion.AngleAxis(garpaRotSpeed * Time.deltaTime, Vector3.up) * targetGarpaPos;
					nearestPos = targetGarpaPos + playerController.transform.position;
					
				}
				else
				{
					garpaNextToPlayer = false;
				}
				
				enemyLocker.position = Vector3.Lerp(enemyLocker.position, nearestPos, enemyLockerSpeed * Time.deltaTime);
			}
			
		}
		
	}

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lockingDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerController.transform.position, cameraDashMinDistance);
    }

    public Vector3 GetCameraForwardXZ()
    {
        Vector3 forwardXZ = new Vector3(cameraTransform.forward.x, 0f, cameraTransform.forward.z);
        forwardXZ = forwardXZ.normalized;
        return forwardXZ;
    }

    public Vector3 GetCameraUp()
    {
        return cameraTransform.up;
    }
    public Vector3 GetCameraForward()
    {
        return cameraTransform.forward;
    }
    
	public float GetCurrentTilt()
	{
		Vector3 fw = GetCameraForward();
		Vector3 perp = Vector3.Cross(fw, Vector3.up).normalized;
		Vector3 hfw = new Vector3(fw.x, 0f, fw.z).normalized;
		
		float tilt = Vector3.SignedAngle(hfw, fw, perp);
		return tilt;
	}
	

    
}
