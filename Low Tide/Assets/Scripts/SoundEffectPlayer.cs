using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomClip()
    {
        var randomID = Mathf.FloorToInt(clips.Length * Random.value);
        var randomClip = clips[randomID];
        audioSource.clip = randomClip;
        audioSource.Play();
    }
}
