using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TinyBall : MonoBehaviour
{
    private Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCheckPointIn(){
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        
    }
    public void OnCheckPointOut(){
        rb.isKinematic = false;
    }
}
