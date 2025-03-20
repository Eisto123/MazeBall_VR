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
    private bool isPlayed = false;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.outputAudioMixerGroup = audioType;

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ball"|| other.gameObject.tag == "TinyBall")
        {
            audioSource.Play();
            isPlayed = true;
        }
    }

    void Update()
    {
        if(isPlayed)
        {
            if(!audioSource.isPlaying){
                this.gameObject.SetActive(false);
            }
        }
    }

    public void PlayAudio()
    {
        audioSource.Play();
        isPlayed = true;
    }
}
