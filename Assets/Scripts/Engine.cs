using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Engine : MonoBehaviour
{
    
    public float rotationSpeed = 50f; // Speed of rotation
    public bool isRotating = false;
    void Update()
    {
        if (isRotating)
        {
            transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y , transform.rotation.eulerAngles.z - rotationSpeed * Time.deltaTime);
        }
    }

    public void EnableRotation()
    {
        isRotating = true;
    }

    public void DisableRotation()
    {
        isRotating = false;
    }
}
