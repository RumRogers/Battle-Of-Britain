using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDetectViewport : MonoBehaviour
{
    [SerializeField]
    private Transform m_topLeft;
    [SerializeField]
    private Transform m_bottomRight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        m_topLeft.position = new Vector3(v.x, 0, v.z);
        print(v);
        v = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, Screen.height));
        m_bottomRight.position = new Vector3(v.x, 0, v.z);
        print(v);
    }
}
