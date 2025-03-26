using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class MazeBall : MonoBehaviour
{
    [SerializeField] private HandGrabInteractor _handGrabInteractorLeft;
    [SerializeField] private HandGrabInteractor _handGrabInteractorRight;
    [SerializeField] private HandGrabInteractor _controllerGrabInteractorLeft;
    [SerializeField] private HandGrabInteractor _controllerGrabInteractorRight;

    public GameObject UI;
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_handGrabInteractorLeft.IsGrabbing || _handGrabInteractorRight.IsGrabbing){
            UI.SetActive(false);
            if(audioSource.enabled == true){
                audioSource.Play();
                audioSource.enabled = false;
            }
            
        }
        if (_controllerGrabInteractorLeft.IsGrabbing || _controllerGrabInteractorRight.IsGrabbing){
            UI.SetActive(false);
            if(audioSource.enabled == true){
                audioSource.Play();
                audioSource.enabled = false;
            }
        }
    }
}
