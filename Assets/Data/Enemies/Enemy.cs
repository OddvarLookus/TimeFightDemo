using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ENEMY HAS THE GENERIC THINGS FOR ALL ENEMIES. 
public class Enemy : MonoBehaviour
{
    protected Rigidbody rb;
    protected Health health;

    [SerializeField] protected Renderer mainRenderer;

    public Renderer GetRenderer()
    {
        return mainRenderer;
    }

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();

        health.Initialize();
    }

    public void Push(Vector3 _pushForce)
    {
        rb.AddForce(_pushForce, ForceMode.Impulse);
    }

}
