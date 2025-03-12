using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Oculus.Interaction.HandGrab;
using Meta.WitAi;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject BigBall;
    private GameObject TinyBall;
    public GameObject mazeBall;
    public GameObject RoomCubePrefab;
    public HandGrabInteractor leftInteractor;
    public HandGrabInteractor rightInteractor;
    public GameObject interactionControl;
    // Start is called before the first frame update
    void Start()
    {
        RoomCubePrefab.SetActive(false);
        TinyBall = GameObject.FindGameObjectWithTag("TinyBall");
    }

    // Update is called once per frame
    void Update()
    {
        
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

        yield return new WaitForSeconds(1);
        BigBall.SetActive(true);
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
        BigBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        BigBall.GetComponent<Rigidbody>().isKinematic = true;
        leftInteractor.ForceRelease();
        rightInteractor.ForceRelease();
        interactionControl.SetActive(false);
        mazeBall.SetActive(true);
        

        yield return new WaitForSeconds(1);
        TinyBall.SetActive(true);
        BigBall.SetActive(false);
        RoomCubePrefab.SetActive(false);
        interactionControl.SetActive(true);
        TinyBall.GetComponent<Rigidbody>().isKinematic = false;
        CameraManager.Instance.SwitchTarget();
        
    }
}
