using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> BGMClips;
    private float bgmStartVolume;
    public List<AudioClip> PlayerSFX;
    private float SFXStartVolume;
    public List<AudioClip> enviromentSFX;

    public AudioSource BGM;
    public AudioSource SFX;
    public AudioSource Enviroment;

    public static AudioManager Instance;

    private void Awake()
    {
        if(Instance == null){
            Instance = this;
            //DontDestroyOnLoad(gameObject);

            bgmStartVolume = BGM.volume;
            SFXStartVolume = SFX.volume;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void playBGM(int i){
        if(BGM.clip == BGMClips[i]){
            return;
        }
        BGM.clip = BGMClips[i];
        BGM.Play();
    }

    public IEnumerator PlayBGM(int i, float fadeTime, bool restart=false){

        if(BGM.clip == BGMClips[i] && !restart){
            yield return null;
        }

        BGM.volume = 0;
        BGM.clip = BGMClips[i];
        BGM.Play();

        while (BGM.volume < bgmStartVolume) {
            BGM.volume += bgmStartVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        BGM.volume = bgmStartVolume;
        
    }

    public void PlaySFX(int i){
        SFX.clip = PlayerSFX[i];
        SFX.volume = SFXStartVolume;
        SFX.Play();
    }

    public IEnumerator PlaySFX(int i, float time)
    {
        yield return new WaitForSeconds(time);
        SFX.volume = SFXStartVolume;
        SFX.clip = PlayerSFX[i];
        SFX.Play();
    }

    public void PlayEnviroment(int i){
        Enviroment.clip = enviromentSFX[i];
        Enviroment.Play();
    }

    public IEnumerator PlayEnviroment(int i, float time)
    {
        yield return new WaitForSeconds(time);
        Enviroment.clip = enviromentSFX[i];
        Enviroment.Play();
    }

    public void FadeOutBGM(){
        StartCoroutine(FadeOut(BGM,2f));
    }

    public void FadeOutSFX(){
        StartCoroutine(FadeOut(SFX,1f));
    }
    public void FadeOutEnviroment(){
        StartCoroutine(FadeOut(Enviroment,1f));
    }

    private IEnumerator FadeOut(AudioSource audioSource, float fadeTime){
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.Stop ();
        audioSource.volume = startVolume;
    }

    public void DisableAudioSource(params string[] audioName)
    {
        foreach (string name in audioName)
        {
            switch (name)
            {
                case "BGM":
                    BGM.enabled = false;
                    break;
                case "SFX":
                    SFX.enabled = false;
                    break;
                case "Environment":
                    Enviroment.enabled = false;
                    break;
            }
        }
    }

}

