using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBlocking : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
