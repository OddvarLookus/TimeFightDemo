using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credit : MonoBehaviour
{
    [SerializeField] int value;
    [SerializeField] float baseSpeed;

    public void Attract(Vector3 _toPos)
    {
        Vector3 vecToPos = _toPos - transform.position;
        float distance = vecToPos.magnitude;
        distance = Mathf.Clamp(distance, 0.1f, 10f);
        float realSpeed = baseSpeed / distance;
        transform.position += vecToPos.normalized * realSpeed * Time.deltaTime;
    }

    public void Delete()
    {
        Destroy(this.gameObject);
    }


}
