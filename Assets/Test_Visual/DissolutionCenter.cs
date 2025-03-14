using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DissolutionCenter : MonoBehaviour
{
    // ���������ܽ�����
    public Transform target;
    // ����
    public Material material;
    void Update()
    {
        if (target && material)
        {
            material.SetVector("_Center",target.position);
        }
        
    }
}
