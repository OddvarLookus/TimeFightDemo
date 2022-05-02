using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundsPack
{
    
	[SerializeField] AudioClip[] possibleSounds;
	[SerializeField] float minPitch, maxPitch;
	
	public AudioClip GetRandomSound()
	{
		int rndSnd = Random.Range(0, possibleSounds.Length);
		return possibleSounds[rndSnd];
	}
    
	public float GetRandomPitch()
	{
		return Random.Range(minPitch, maxPitch);
	}
    
}
