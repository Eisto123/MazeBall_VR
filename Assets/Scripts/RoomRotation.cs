using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomRotation : MonoBehaviour
{
    public GameObject mazeBall;
    void FixedUpdate()
    {
        this.transform.rotation = mazeBall.transform.rotation;
    }
}
