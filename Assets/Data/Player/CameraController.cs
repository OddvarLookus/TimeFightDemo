using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //[Header("Input")]
    //[SerializeField] InputManager inputManager;

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
    [SerializeField] bool enableSmoothing;
    [SerializeField] float smoothingSpeed;
    float targetRot;
    float targetTilt;

    #region MONOBEHAVIOR
    private void Update()
    {
        CameraLookBehavior();
    }
    #endregion
    #region CAMERA ROTATION
    void CameraRotationBehavior()
    {
        if (invertX == false)
        {
            targetRot += Input.GetAxisRaw("Mouse X") * cameraSpeed;
        }
        else
        {
            targetRot -= Input.GetAxisRaw("Mouse X") * cameraSpeed;
        }

        //targetRot = ClampAngle(targetRot);

        if (enableSmoothing)
        {
            cameraRot = Mathf.Lerp(cameraRot, targetRot, smoothingSpeed * Time.deltaTime);
            //cameraRot = ClampAngle(cameraRot);
        }
        else
        {
            targetRot = ClampAngle(targetRot);
            cameraRot = targetRot;
        }
        //cameraRot = ClampAngle(cameraRot);
        //Vector3 playerToCam = cameraTransform.position - cameraLookTarget.position;
        Vector3 newCamPos = (Quaternion.AngleAxis(cameraRot, -cameraLookTarget.up) * Vector3.forward) * maxCameraDist + cameraLookTarget.position;
        cameraTransform.position = newCamPos;
    }
    void CameraTiltBehavior()
    {
        Vector3 playerToCam = cameraTransform.position - cameraLookTarget.position;
        if (invertY == false)
        {
            targetTilt += Input.GetAxisRaw("Mouse Y") * cameraSpeed;
        }
        else
        {
            targetTilt -= Input.GetAxisRaw("Mouse Y") * cameraSpeed;
        }

        targetTilt = Mathf.Clamp(targetTilt, minCameraTilt, maxCameraTilt);

        if (enableSmoothing)
        {
            cameraTilt = Mathf.Lerp(cameraTilt, targetTilt, smoothingSpeed * Time.deltaTime);
        }
        else
        {
            cameraTilt = targetTilt;
        }
        Vector3 perpendicular = Vector3.Cross(playerToCam, cameraLookTarget.up);
        Vector3 newCamPos = (Quaternion.AngleAxis(cameraTilt, perpendicular) * playerToCam.normalized) * maxCameraDist + cameraLookTarget.position;// + cameraLookTarget.position;
        //newCamPos = LimitTilt(newCamPos, perpendicular);
        //newCamPos += cameraLookTarget.position;
        cameraTransform.position = newCamPos;
        //Debug.Log($"CameraTilt = {newCamPos}");
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
        if (Physics.Raycast(cameraLookTarget.position, playerToCam, out rhit, maxCameraDist, cameraRaycastLayers.value))
        {
            cameraTransform.position = rhit.point + (-playerToCam.normalized * cameraRayHitOffset);
            return;
        }
        cameraTransform.position = cameraLookTarget.position + playerToCam;
    }
    void CameraLookBehavior()
    {
        CameraRotationBehavior();
        CameraTiltBehavior();
        KeepCameraDistance();
        cameraTransform.LookAt(cameraLookTarget, cameraLookTarget.up);
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
