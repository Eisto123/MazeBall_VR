using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject BigBall;
    public GameObject TinyBall;
    public GameObject RoomCubePrefab;
    private GameObject theCube;
    public GameObject interactionControl;
    // Start is called before the first frame update
    void Start()
    {
        
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
        interactionControl.SetActive(false);

        yield return new WaitForSeconds(1);
        BigBall.SetActive(true);
        TinyBall.SetActive(false);
        interactionControl.SetActive(true);
        BigBall.GetComponent<Rigidbody>().isKinematic = false;
        CameraManager.Instance.SwitchTarget();
        RoomCubePrefab.SetActive(true);
        RoomCubePrefab.transform.position = new Vector3(0,4.5f,0);
        
    }

    public void SwitchToTinyBall(){
        StartCoroutine(SwitchToTinyBallProcess());
    }
    private IEnumerator SwitchToTinyBallProcess(){
        interactionControl.SetActive(false);
        BigBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        BigBall.GetComponent<Rigidbody>().isKinematic = true;
        

        yield return new WaitForSeconds(1);
        TinyBall.SetActive(true);
        BigBall.SetActive(false);
        RoomCubePrefab.SetActive(false);

        TinyBall.GetComponent<Rigidbody>().isKinematic = false;
        interactionControl.SetActive(true);
        CameraManager.Instance.SwitchTarget();
        
    }
}
