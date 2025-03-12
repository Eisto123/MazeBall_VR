using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ResetBall : MonoBehaviour
{
    public GameObject TinyBall;
    public Transform MazeBall;
    private Transform StartPos;
    private Rigidbody rb;
    private quaternion mazeBallRotation;

    void Start()
    {
        rb = TinyBall.GetComponent<Rigidbody>();
        mazeBallRotation = MazeBall.rotation;
        StartPos = TinyBall.transform;
    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if(collision.gameObject.tag == "Ball"){
            
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            MazeBall.transform.rotation = mazeBallRotation;

            TinyBall.transform.position = StartPos.position;
            rb.isKinematic = false;
        }
    }
}
