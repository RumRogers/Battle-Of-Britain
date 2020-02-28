using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDetectViewport : MonoBehaviour
{
    enum LineID
    {
        TL_TR,
        BL_BR,
        TL_BL,
        TR_BR
    };

    [SerializeField]
    private Transform m_topLeft;
    [SerializeField]
    private Transform m_bottomRight;
    [SerializeField]
    private Transform m_center;
    [SerializeField]
    private Transform m_target;
    [SerializeField]
    private Transform m_intersectionMarker;
    [SerializeField]
    private Vector3 tl, tr, bl, br, c;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        tl = new Vector3(v.x, 0f, v.z);
        //m_topLeft.position = new Vector3(v.x, 0, v.z);
        print(v);
        v = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, Screen.height));
        br = new Vector3(v.x, 0, v.z);

        tr = new Vector3(br.x, 0, tl.z);
        bl = new Vector3(tl.x, 0, br.z);
        print(v);
        Debug.DrawLine(tl, tr, Color.blue);
        Debug.DrawLine(tl, bl, Color.blue);
        Debug.DrawLine(tr, br, Color.blue);
        Debug.DrawLine(bl, br, Color.blue);
        //float viewportLengthX = Mathf.Abs(m_topLeft.position.x - m_bottomRight.position.x);
        //float viewportLengthZ = Mathf.Abs(m_topLeft.position.z - m_bottomRight.position.z);
        float viewportLengthX = Mathf.Abs(tl.x - br.x);
        float viewportLengthZ = Mathf.Abs(tl.z - br.z);

        //m_center.position = new Vector3(m_topLeft.position.x + viewportLengthX / 2, 0f, m_topLeft.position.z - viewportLengthZ / 2); // <- minus because Unity is left handed
        c = new Vector3(tl.x + viewportLengthX / 2, 0f, tl.z - viewportLengthZ / 2);
        Debug.DrawLine(m_target.position, c, Color.red);
    }

    private Vector3 FindIntersectionWithViewportBounds(Vector3 endpointA, Vector3 endpointB, LineID which)
    {
        Vector3 res = Vector3.zero;

        float m = (endpointB.z - endpointA.z) / (endpointB.x - endpointA.x);
        float b = (endpointB.x * endpointA.z - endpointA.x * endpointB.z) / (endpointB.x * endpointA.z);

        // y = mx + b
        // x = (y - b)/m
        switch(which)
        {
            case LineID.TL_TR:
                res.z = m_topLeft.position.z;
                res.x = (res.z - b) / m;
                break;
            case LineID.BL_BR:
                res.z = m_bottomRight.position.z;
                res.x = (res.z - b) / m;
                break;
            case LineID.TL_BL:
                res.x = m_topLeft.position.x;
                res.z = m * res.x + b;
                break;
            case LineID.TR_BR:
                res.x = m_bottomRight.position.x;
                res.z = m * res.x + b;
                break;
        }
        
        return res;
    }
}
