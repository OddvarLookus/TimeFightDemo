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
        CameraLookBehavior();
        CameraDistanceBySpeed();
        CameraPositionBehavior();
        KeepCameraDistance();
        cameraTransform.LookAt(cameraLookTarget, cameraLookTarget.up);

        
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

    Transform[] lockableEnemies;
    Transform lockedEnemy;
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
            if (Vector3.Dot(vecToCol, transform.forward) > 0f)
            {
                enemiesInFront.Add(colliders[i].transform);
            }
        }

        //filter the colliders if they have a Enemy component


        //filter the colliders inside the camera frustum


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
}
