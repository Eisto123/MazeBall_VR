using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject BigBall;
    public GameObject TinyBall;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchToBigBall(){
        BigBall.SetActive(true);
        TinyBall.SetActive(false);
    }
    public void SwitchToTinyBall(){
        TinyBall.SetActive(true);
        BigBall.SetActive(false);
    }
}
