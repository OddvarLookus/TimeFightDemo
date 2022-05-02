using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class StaticAudioStarter : MonoBehaviour
{
	public static StaticAudioStarter instance;
	[AssetsOnly] [SerializeField] GameObject audioEmitterPrefab;
	
	protected void Awake()
	{
		instance = this;
	}
	
	public void StartAudioEmitter(Vector3 pos, AudioClip clip, float nPitch = 1f)
	{
		GameObject emitter = Instantiate(audioEmitterPrefab);
		emitter.transform.SetParent(this.transform);
		emitter.transform.position = pos;
		emitter.GetComponent<SoundSelfDestroy>().Initialize(clip, nPitch);
	}
	
}
