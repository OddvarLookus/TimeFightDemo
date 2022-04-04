using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ENEMY HAS THE GENERIC THINGS FOR ALL ENEMIES. 
public class Enemy : MonoBehaviour
{
    protected Rigidbody rb;
    protected Health health;

    [SerializeField] protected Renderer mainRenderer;

    [Header("Size")]
    [SerializeField] float minSize;
    [SerializeField] float maxSize;

    public Renderer GetRenderer()
    {
        return mainRenderer;
    }

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();

        InitializeEnemy();
    }

    void InitializeEnemy()
    {
        float factor = Random.Range(0f, 1f);
        health.Initialize(factor);
        float size = Mathf.Lerp(minSize, maxSize, factor);
        transform.localScale = new Vector3(size, size, size);
    }

    public void Push(Vector3 _pushForce)
    {
        rb.AddForce(_pushForce, ForceMode.Impulse);
    }


    //GIZMOS
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxSize);
    }
}
