using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundSelfDestroy : MonoBehaviour
{
    AudioSource audioSorcio;
    public void Initialize(AudioClip _clip)
    {
        audioSorcio = GetComponent<AudioSource>();
        audioSorcio.clip = _clip;
        float duration = _clip.length;
        audioSorcio.Play();
        StartCoroutine(SourceDestroyCoroutine(duration));
    }

    IEnumerator SourceDestroyCoroutine(float _time)
    {
        yield return new WaitForSeconds(_time);
        Destroy(this.gameObject);
    }
}