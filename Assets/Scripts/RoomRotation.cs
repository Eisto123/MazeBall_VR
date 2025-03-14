using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction{
    xplus,
    xminus,
    yplus,
    yminus,
    zplus,
    zminus
}
public class RoomRotation : MonoBehaviour
{
    public Dictionary<int, Vector3> wallInfo = new Dictionary<int, Vector3>();
    public GameObject roomCube;
    public GameObject roomClockWise;
    private bool enableRotation = false;
    private bool isEnding = false;
    private bool needFlip = false;
    public float rotationDuration = 2.0f; // Time to complete the rotation
    [SerializeField] private float rotationSpeed = 5f; // Adjust speed in Unity Inspector


    void Start()
    {
    }
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
    

    public void RotateRoomXPlus()
    {
        DisableRotation();
        StartCoroutine(RotateRoomCoroutine(Direction.xplus));
    }

    public void RotateRoomXMinus()
    {
        DisableRotation();
        StartCoroutine(RotateRoomCoroutine(Direction.xminus));
    }
    public void RotateRoomYPlus()
    {
        DisableRotation();
        StartCoroutine(RotateRoomCoroutine(Direction.yplus));
    }
    public void RotateRoomYMinus()
    {
        DisableRotation();
        StartCoroutine(RotateRoomCoroutine(Direction.yminus));
    }
    public void RotateRoomZPlus()
    {
        DisableRotation();
        StartCoroutine(RotateRoomCoroutine(Direction.zplus));
    }
    public void RotateRoomZMinus()
    {
        DisableRotation();
        StartCoroutine(RotateRoomCoroutine(Direction.zminus));
    }

    private IEnumerator RotateRoomCoroutine(Direction direction)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.identity;
        if (direction == Direction.xplus){
            targetRotation = Quaternion.Euler(transform.eulerAngles.x + 90, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        else if (direction == Direction.xminus){
            targetRotation = Quaternion.Euler(transform.eulerAngles.x - 90, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        else if (direction == Direction.yplus){
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y+90, transform.eulerAngles.z);
        }
        else if (direction == Direction.yminus){
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y-90, transform.eulerAngles.z);
        }
        else if (direction == Direction.zplus){
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z+90);
        }
        else if (direction == Direction.zminus){
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z-90);
        }
        if(needFlip){
            needFlip = false;
            roomClockWise.transform.eulerAngles = new Vector3(roomClockWise.transform.eulerAngles.x, roomClockWise.transform.eulerAngles.y, 0);
            
            targetRotation.eulerAngles = new Vector3(0, 90, 0);
        }
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.rotation = targetRotation; // Ensure final rotation is precise
        roomCube.transform.eulerAngles = new Vector3(0, 0, 0);
        if(needFlip){
            
        }

        if(!isEnding)
            EnableRotation();
        else
            isEnding = false;
        yield return null;
    }

    public void resetZ(){
        needFlip = true;
        
    }
    public void OnEnding(){
        isEnding = true;
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
