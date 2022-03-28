using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credit : MonoBehaviour
{
    [SerializeField] int value;
    [SerializeField] float baseSpeed;
    [SerializeField] float timeSpeedMultiplier;
    Transform target;
    bool isBeingSucked = false;

    float suckTime = 0f;

    public void Attract(Transform _target)
    {
        if (!isBeingSucked)
        {
            target = _target;

            isBeingSucked = true;
        }

    }

    private void Update()
    {
        CreditMovement();
    }

    void CreditMovement()
    {
        if (isBeingSucked)
        {
            suckTime += Time.deltaTime;

            Vector3 vecToPos = target.position - transform.position;
            //float distance = vecToPos.magnitude;
            //distance = Mathf.Clamp(distance, 0.1f, 1000f);
            //float realSpeed = baseSpeed / distance;
            transform.position += vecToPos.normalized * timeSpeedMultiplier * suckTime * Time.deltaTime;
        }

    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out CreditsSucker creditsSucker))
        {
            creditsSucker.AddCredits(value);
            Destroy(this.gameObject);
        }
    }
}