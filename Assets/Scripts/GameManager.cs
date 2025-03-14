using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Oculus.Interaction.HandGrab;
using Meta.WitAi;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public enum level{
    Level1,
    Level2,
    Level3,
    Level4
}

public class GameManager : MonoBehaviour
{
    private level currentLevel = level.Level1;
    public static GameManager instance;
    public GameObject BigBall;
    private GameObject TinyBall;
    public GameObject mazeBall;
    public GameObject RoomCubePrefab;
    public HandGrabInteractor leftInteractor;
    public HandGrabInteractor rightInteractor;
    public GameObject interactionControl;

    public Volume globalVolume;

    public GameManager Instance{
        get{
            if(instance == null){
                instance = this;
            }
            return instance;
        }
    }

    public List<GameObject> BigBallStartPos;
    public List<GameObject> BigBallTransitionPos;
    public List<GameObject> TinyBallStartPos;
    public List<Vector3> MazeBallRotationForTinyBall;
    // Start is called before the first frame update
    void Start()
    {
        RoomCubePrefab.SetActive(false);
        TinyBall = GameObject.FindGameObjectWithTag("TinyBall");
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.Start)){
            ResetGame();
        }
    }

    private void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SwitchToBigBall(){
        StartCoroutine(SwitchToBigBallProcess());
    }
    private IEnumerator SwitchToBigBallProcess(){
        TinyBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        TinyBall.GetComponent<Rigidbody>().isKinematic = true;
        leftInteractor.ForceRelease();
        rightInteractor.ForceRelease();
        interactionControl.SetActive(false);
        mazeBall.SetActive(false);
        BigBall.SetActive(true);
        SetBigBallPositionBaseOnCurrentLevel();

        yield return new WaitForSeconds(1);
        TinyBall.SetActive(false);
        BigBall.GetComponent<Rigidbody>().isKinematic = false;
        CameraManager.Instance.SwitchTarget();
        interactionControl.SetActive(true);
        RoomCubePrefab.SetActive(true);
        RoomCubePrefab.transform.position = new Vector3(0,4.5f,0);
        
    }

    public void SwitchToTinyBall(){
        StartCoroutine(SwitchToTinyBallProcess());
    }


    private IEnumerator SwitchToTinyBallProcess(){
        currentLevel++;
        BigBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        BigBall.GetComponent<Rigidbody>().isKinematic = true;
        leftInteractor.ForceRelease();
        rightInteractor.ForceRelease();
        interactionControl.SetActive(false);
        mazeBall.SetActive(true);
        TinyBall.SetActive(true);
        SetTinyBallPositionBaseOnCurrentLevel();

        yield return new WaitForSeconds(1);
        
        BigBall.SetActive(false);
        RoomCubePrefab.SetActive(false);
        interactionControl.SetActive(true);
        TinyBall.GetComponent<Rigidbody>().isKinematic = false;
        CameraManager.Instance.SwitchTarget();
        
    }

    private void SetBigBallPositionBaseOnCurrentLevel(){
        switch(currentLevel){
            case level.Level1:
                break;
            case level.Level2:
                BigBall.transform.position = BigBallStartPos[0].transform.position;
                break;
            case level.Level3:
                BigBall.transform.position = BigBallStartPos[1].transform.position;
                break;
            case level.Level4:
                Debug.Log("EndAnimation");
                break;
        }
    }

    private void SetTinyBallPositionBaseOnCurrentLevel(){
        switch(currentLevel){
            case level.Level1:
                break;
            case level.Level2:
                mazeBall.transform.eulerAngles = MazeBallRotationForTinyBall[0];
                TinyBall.transform.position = TinyBallStartPos[0].transform.position;
                break;
            case level.Level3:
                mazeBall.transform.eulerAngles = MazeBallRotationForTinyBall[1];
                TinyBall.transform.position = TinyBallStartPos[1].transform.position;
                break;
            case level.Level4:
                mazeBall.transform.eulerAngles = MazeBallRotationForTinyBall[2];
                TinyBall.transform.position = TinyBallStartPos[2].transform.position;
                break;
        }
    }

    public void OnBigBallTransition(int index){
        StartCoroutine(BigBallTransitionProcess(index));
    }

    private IEnumerator BigBallTransitionProcess(int index){
        BigBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        BigBall.GetComponent<Rigidbody>().isKinematic = true;
        leftInteractor.ForceRelease();
        rightInteractor.ForceRelease();
        interactionControl.SetActive(false);
        StartCoroutine(LerpGlobalVolume(1, -0.8f, 0.5f));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(LerpGlobalVolume(0.3f, 0, 0.5f));
        yield return new WaitForSeconds(0.5f);

        SetBigBallTransitionPosition(index);
        BigBall.GetComponent<Rigidbody>().isKinematic = false;
        interactionControl.SetActive(true);
        
    }

    private IEnumerator LerpGlobalVolume(float targetValueForVignette, float targetValueForDistortion, float duration){
        float elapsedTime = 0;
        float startVignetteIntensity;
        float startDistortionIntensity;
        UnityEngine.Rendering.Universal.Vignette TargetVignette;
        LensDistortion TargetDistortion;

        UnityEngine.Rendering.VolumeProfile volumeProfile = globalVolume.profile;
        if(!volumeProfile.TryGet<Vignette>(out Vignette vignette)){
            throw new System.NullReferenceException(nameof(vignette));
        } 
        else{
            startVignetteIntensity = vignette.intensity.value;
            TargetVignette = vignette;
        }

        if(!volumeProfile.TryGet<LensDistortion>(out LensDistortion lensDistortion)){
            throw new System.NullReferenceException(nameof(lensDistortion));
        }
        else{
            startDistortionIntensity = lensDistortion.intensity.value;
            TargetDistortion = lensDistortion;
        }
        Debug.Log("StartVignetteIntensity: " + startVignetteIntensity);
        Debug.Log("StartDistortionIntensity: " + startDistortionIntensity);
        while(elapsedTime < duration){
            TargetVignette.intensity.Override(Mathf.Lerp(startVignetteIntensity, targetValueForVignette, elapsedTime/duration));
            TargetDistortion.intensity.Override(Mathf.Lerp(startDistortionIntensity, targetValueForDistortion, elapsedTime/duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        TargetVignette.intensity.value = targetValueForVignette;
        TargetDistortion.intensity.value = targetValueForDistortion;
        
    }

    public void SetBigBallTransitionPosition(int index){
        BigBall.transform.position = BigBallTransitionPos[index].transform.position;
    }


}
