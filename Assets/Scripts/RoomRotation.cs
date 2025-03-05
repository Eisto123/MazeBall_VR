using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomRotation : MonoBehaviour
{
    public GameObject mazeBall;
    public GameObject roomCube;
    private bool enableRotation = false;
    [SerializeField] private float rotationSpeed = 5f; // Adjust speed in Unity Inspector
    void FixedUpdate()
    {
        if(enableRotation)
            RotateRoom();
    }

    private void RotateRoom()
    {
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, roomCube.transform.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    public void EnableRotation()
    {
        enableRotation = true;
    }

    public void DisableRotation()
    {
        enableRotation = false;
        this.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }
}
