using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomRotation : MonoBehaviour
{
    public GameObject roomCube;
    public GameObject roomClockWise;
    private bool enableRotation = false;
    private bool onTransition = false;
    public float rotationDuration = 2.0f; // Time to complete the rotation
    [SerializeField] private float rotationSpeed = 5f; // Adjust speed in Unity Inspector
    void FixedUpdate()
    {
        if(enableRotation)
            RotateRoom();
    }

    private void RotateRoom()
    {
        Quaternion targetRotation = Quaternion.Euler(roomClockWise.transform.eulerAngles.x, roomClockWise.transform.eulerAngles.y, roomCube.transform.eulerAngles.z);
        roomClockWise.transform.rotation = Quaternion.Slerp(roomClockWise.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    

    public void RotateRoomForward()
    {
        DisableRotation();
        StartCoroutine(RotateRoomCoroutine());
    }

    private IEnumerator RotateRoomCoroutine()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x + 90, transform.eulerAngles.y, transform.eulerAngles.z);
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation; // Ensure final rotation is precise
        EnableRotation();
    }

    public void EnableRotation()
    {
        enableRotation = true;
    }

    public void DisableRotation()
    {
        enableRotation = false;
        roomClockWise.transform.eulerAngles = new Vector3(roomClockWise.transform.eulerAngles.x, roomClockWise.transform.eulerAngles.y, 0);
    }
}
