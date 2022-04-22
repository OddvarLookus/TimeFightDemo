using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credit : MonoBehaviour
{
    [SerializeField] int value;
	[SerializeField] float timeToUnit;
	float totalTime;
    Transform target;
	bool isBeingSucked = false;
	CreditsSucker creditsSucker;

    float suckTime = 0f;

	public void Attract(Transform _target, CreditsSucker _creditSucker)
    {
        if (!isBeingSucked)
        {
            target = _target;

	        isBeingSucked = true;
	        creditsSucker = _creditSucker;
	        
	        float dist = (target.position - transform.position).magnitude;
	        totalTime = dist * timeToUnit;
        }

    }

    private void FixedUpdate()
    {
        CreditMovement();
    }

    void CreditMovement()
    {
        if (isBeingSucked)
        {
        	float t = suckTime / totalTime;
        	
        	transform.position = Vector3.Lerp( transform.position, target.position, t * t);
        	
        	if(suckTime < totalTime)
        	{
        		suckTime += Time.fixedDeltaTime;
        	}
        	else if(suckTime >= totalTime)
        	{
        		creditsSucker.AddCredits(value);
        		Destroy(this.gameObject);
	        	
        	}
            


        }

    }


}
