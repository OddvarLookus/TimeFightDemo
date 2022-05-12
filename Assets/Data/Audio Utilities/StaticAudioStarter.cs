using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class StaticAudioStarter : MonoBehaviour
{
	public static StaticAudioStarter instance;
	[AssetsOnly] [SerializeField] GameObject audioEmitterPrefab;
	[AssetsOnly] [SerializeField] GameObject preloadedSoundPrefab;
	
	protected void Awake()
	{
		instance = this;
		
		//INITIALIZE CRUPS SOURCES. QUEUE
		for(int i = 0; i < maxCrupsAudios; i++)
		{
			GameObject newSound = Instantiate(preloadedSoundPrefab);
			newSound.transform.SetParent(transform);
			AudioSource nSource = newSound.GetComponent<AudioSource>();
			crupsSources.Clear();
			crupsSources.Enqueue(nSource);
		}
	}
	
	public void StartAudioEmitter(Vector3 pos, AudioClip clip, float nPitch = 1f)
	{
		GameObject emitter = Instantiate(audioEmitterPrefab);
		emitter.transform.SetParent(this.transform);
		emitter.transform.position = pos;
		emitter.GetComponent<SoundSelfDestroy>().Initialize(clip, nPitch);
	}
	
	//CRUPS AUDIO
	[Header("CRUPS AUDIO")]
	[MinValue(0)] [SerializeField] int maxCrupsAudios;
	Queue<AudioSource> crupsSources = new Queue<AudioSource>();
	
	public void PlayCrupsSound(Vector3 pos, AudioClip clip, float nPitch = 1f)
	{
		AudioSource audSrc = crupsSources.Dequeue();
		audSrc.transform.position = pos;
		audSrc.clip = clip;
		audSrc.pitch = nPitch;
		audSrc.Play();
		crupsSources.Enqueue(audSrc);
	}
	
}


