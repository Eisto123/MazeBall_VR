using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckPoint : MonoBehaviour
{
    public UnityEvent CheckpointEvent;

    private void Start()
    {
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ball"|| other.gameObject.tag == "TinyBall"){
            CheckpointEvent.Invoke();
            this.gameObject.SetActive(false);
            
        }
    }
}
