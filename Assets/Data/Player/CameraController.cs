using System.Collections;
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

    #region MONOBEHAVIOR
    private void Awake()
    {
        targetCameraPos = transform.position;
    }
    private void Update()
    {
        LockInputCheck();

        if (cameraMode == CameraMode.FREELOOK)
        {
            CameraLookBehavior();
            CameraDistanceBySpeed();
            CameraPositionBehavior();
            KeepCameraDistance();
            cameraTransform.LookAt(cameraLookTarget, cameraLookTarget.up);
        }
        else if (cameraMode == CameraMode.ENEMYLOCK)
        {
            CameraLockBehavior();
            cameraTransform.LookAt(lockedEnemy, cameraLookTarget.up);
        }
        
    }

    private void FixedUpdate()
    {

    }


    #endregion
    #region CAMERA ROTATION
    Vector3 normalizedCameraPos;
    void CameraRotationBehavior()
    {

        //X ROTATION
        if (!invertX == false)
        {
            targetRot += Input.GetAxisRaw("Mouse X") * cameraSpeed;
        }
        else
        {
            targetRot -= Input.GetAxisRaw("Mouse X") * cameraSpeed;
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
            targetTilt += Input.GetAxisRaw("Mouse Y") * cameraSpeed;
        }
        else
        {
            targetTilt -= Input.GetAxisRaw("Mouse Y") * cameraSpeed;
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

        targetCameraPos = finalRot * Vector3.forward * maxCameraDist + cameraLookTarget.position;

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
        if (Input.GetMouseButtonDown(2))
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

        Vector3 vecToLockedEnemy = lockedEnemy.position - playerController.transform.position;
        Vector3 negative = playerController.transform.position + ((-vecToLockedEnemy).normalized * 8f);

        negative += new Vector3(0f, 2f, 0f);

        cameraTransform.position = negative;
        enemyLocker.position = lockedEnemy.position + new Vector3(0f, 1f, 0f);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lockingDistance);
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

    float ClampAngle(float _angle)
    {
        float a = _angle;
        if (a < 0f)
        {
            a += 360f;
        }
        else if (a > 360f)
        {
            a -= 360f;
        }
        return a;
    }
}
