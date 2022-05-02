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
    
	[SerializeField] SoundsPack gatherSound;

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
        		StaticAudioStarter.instance.StartAudioEmitter(transform.position, gatherSound.GetRandomSound(), gatherSound.GetRandomPitch());
        		Destroy(this.gameObject);
	        	
        	}
            


        }

    }


}
