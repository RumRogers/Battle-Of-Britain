using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMarker : MonoBehaviour
{
    [SerializeField]
    private float m_rotSpeed = 3f;

    void Update()
    {
        transform.Rotate(0f, m_rotSpeed * Time.deltaTime, 0f);      
    }
}
