using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Asteroid : MonoBehaviour
{
    [SerializeField] float minInitSpeed;
    [SerializeField] float maxInitSpeed;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 initSpeed = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        initSpeed = initSpeed.normalized;
        initSpeed *= Random.Range(minInitSpeed, maxInitSpeed);
        rb.AddForce(initSpeed, ForceMode.Impulse);
    }
    
}
