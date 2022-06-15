using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Explosion : MonoBehaviour
{
	[SerializeField] SoundsPack explosionSounds;
	
	
	public void StartExplosion(Vector3 npos, float nsize, float ndamage, int layerMask = ~0)
	{
		GetComponent<ParticleSystem>().Play();
		
		transform.position = npos;
		transform.localScale = new Vector3(nsize, nsize, nsize);
		
		StaticAudioStarter.instance.StartAudioEmitter(transform.position, explosionSounds.GetRandomSound(), explosionSounds.GetRandomPitch());
		
		Collider[] hitColliders = Physics.OverlapSphere(npos, nsize, layerMask, QueryTriggerInteraction.Ignore);
		for(int i = 0; i < hitColliders.Length; i++)
		{
			Collider col = hitColliders[i];
			if (col.TryGetComponent(out Asteroid asteroid))
			{
				Vector3 pushVec = (col.transform.position - transform.position).normalized;
				asteroid.Push(pushVec);
	        
				asteroid.TakeDamage(ndamage, transform.position, NumberTypes.EXPLOSION_DAMAGE);
			}
			if (col.TryGetComponent(out Health health))
			{
				if(health.GetAffiliation() == Affiliation.ENEMY)
				{
					health.TakeDamage(ndamage, transform.position, NumberTypes.EXPLOSION_DAMAGE);
				}
			}
			if(col.TryGetComponent(out PlayerShield playerShield))
			{
				playerShield.TakeDamage((int)ndamage);
			}
		}
	}
    
    
    
}
