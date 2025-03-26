using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioType
{
    Narrative,
    BGM,
    SFX
}

public class AudioCheckPoint : MonoBehaviour
{

    public AudioClip audioClip;
    public AudioMixerGroup audioType;
    private AudioSource audioSource;
    private BoxCollider boxCollider;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.outputAudioMixerGroup = audioType;
        boxCollider = GetComponent<BoxCollider>();

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ball"|| other.gameObject.tag == "TinyBall")
        {
            audioSource.Play();
            boxCollider.enabled = false;
        }
    }

    public void PlayAudio()
    {
        audioSource.Play();
        boxCollider.enabled = false;
    }
}
