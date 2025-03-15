using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Engine1 : MonoBehaviour
{
    
    public float rotationSpeed = 50f; // Speed of rotation
    public bool isRotating = false;
    public bool isEngine2 = false;
    void Update()
    {
        if (isRotating&&!isEngine2)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
        else if (isRotating&&isEngine2)
        {
            transform.Rotate(-rotationSpeed * Time.deltaTime, 0, 0);
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
