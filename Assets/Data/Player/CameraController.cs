using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{


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

    float ClampAngle(float _angle)
    {
        float a = _angle;
        if (a < 0f)
        {
            a += 360f;
        }
        else if(a > 360f)
        {
            a -= 360f;
        }
        return a;
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
