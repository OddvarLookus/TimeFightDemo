using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerMaxSpeed;
    [SerializeField] float playerAcceleration;
    [SerializeField] float playerDeceleration;
    [SerializeField] CameraController camController;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Movement();
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

        inputVec = inputVec.normalized;
        Vector3 relativeInput = Quaternion.LookRotation(camController.GetCameraForward(), camController.GetCameraUp()) * inputVec;
        relativeInput = Vector3.ProjectOnPlane(relativeInput, Vector3.up).normalized;

        rb.AddForce(relativeInput * playerAcceleration, ForceMode.Acceleration);
        Vector3 hVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (hVel.magnitude > playerMaxSpeed)
        {
            hVel = Vector3.ClampMagnitude(hVel, playerMaxSpeed);
            rb.velocity = new Vector3(hVel.x, rb.velocity.y, hVel.z);
        }

        if (inputVec != Vector3.zero)
        {

        }
    }

}
