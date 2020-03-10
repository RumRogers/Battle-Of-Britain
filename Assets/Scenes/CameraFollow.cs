using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField]
    GameObject targetToFollow;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(targetToFollow.transform.position.x,
            targetToFollow.transform.position.y + 10f,
            transform.position.z);
    }
}