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

    enum SectorID
    {
        N, NE, E, SE, S, SW, W, NW
    };

    [SerializeField]
    private Transform m_target;
    [SerializeField]
    private Transform m_intersectionMarker;
    [SerializeField]
    private Transform[] m_markers;
    [SerializeField]
    private Vector3 tl, tr, bl, br, c;
    [SerializeField]
    private LineID m_viewportBound;
    private LineID[] m_lines = new LineID[4] { LineID.TL_TR, LineID.BL_BR, LineID.TL_BL, LineID.TR_BR };
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        Vector3 v = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        tl = new Vector3(v.x, 0f, v.z);
        v = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, Screen.height));
        br = new Vector3(v.x, 0, v.z);
        tr = new Vector3(br.x, 0, tl.z);
        bl = new Vector3(tl.x, 0, br.z);
        Debug.DrawLine(tl, tr, Color.blue);
        Debug.DrawLine(tl, bl, Color.blue);
        Debug.DrawLine(tr, br, Color.blue);
        Debug.DrawLine(bl, br, Color.blue);
        float viewportLengthX = Mathf.Abs(tl.x - br.x);
        float viewportLengthZ = Mathf.Abs(tl.z - br.z);

        c = new Vector3(tl.x + viewportLengthX / 2, 0f, tl.z - viewportLengthZ / 2);
        Debug.DrawLine(m_target.position, c, Color.red);

        /*for(int i = 0; i < m_lines.Length; i++)
        {
            m_markers[i].position = FindIntersectionWithViewportBounds(m_target.position, c, m_lines[i]);
        }*/

        if(IsPointInsideViewport(m_target.position))
        {

            print("INSIDE VIEWPORT");
        }
        else
        {
            switch(FindSectorFromPoint(m_target.position))
            {
                case SectorID.N: print("NORTH");
                    break;
                case SectorID.S:
                    print("SOUTH");
                    break;
                case SectorID.W:
                    print("WEST");
                    break;
                case SectorID.E:
                    print("EAST");
                    break;
                case SectorID.NW:
                    print("NORTH-WEST");
                    break;
                case SectorID.NE:
                    print("NORTH-EAST");
                    break;
                case SectorID.SW:
                    print("SOUTH-WEST");
                    break;
                case SectorID.SE:
                    print("SOUTH-EAST");
                    break;
            }
        }
    }

    private Vector3 FindIntersectionWithViewportBounds(Vector3 endpointA, Vector3 endpointB, LineID which)
    {
        Vector3 res = Vector3.zero;

        if(endpointB.x == endpointA.x)
        {
            res.x = c.x;
            switch(which)
            {
                case LineID.TL_TR:
                    res.z = tl.z;
                    break;
                case LineID.BL_BR:
                    res.z = br.z;
                    break;
            }
            return res;
        }

        
        float m = (endpointB.z - endpointA.z) / (endpointB.x - endpointA.x);
        float b = (endpointB.x * endpointA.z - endpointA.x * endpointB.z) / (endpointB.x - endpointA.x);
        
        // y = mx + b
        // x = (y - b)/m
        switch(which)
        {
            case LineID.TL_TR:
                res.z = tl.z;
                res.x = (res.z - b) / m;
                break;
            case LineID.BL_BR:
                res.z = br.z;
                res.x = (res.z - b) / m;                
                break;
            case LineID.TL_BL:
                res.x = tl.x;
                res.z = m * res.x + b;
                break;
            case LineID.TR_BR:
                res.x = br.x;
                res.z = m * res.x + b;
                break;
        }
        
        return res;
    }

    private bool IsPointInsideViewport(Vector3 p)
    {
        return p.x >= tl.x && p.x < tr.x && p.z <= tl.z && p.z > bl.z;
    }

    private SectorID FindSectorFromPoint(Vector3 p)
    {
        if(p.x > tl.x && p.x < tr.x)
        {
            if(p.z > tl.z)
            {
                return SectorID.N;
            }
            return SectorID.S;
        }

        if(p.z < tl.z && p.z > bl.z)
        {
            if(p.x < tl.x)
            {
                return SectorID.W;
            }
            return SectorID.E;
        }
        
        if(p.x < tl.x)
        {
            if(p.z > tl.z)
            {
                return SectorID.NW;
            }
            return SectorID.SW;
        }

        if(p.z > tl.z)
        {
            return SectorID.NE;
        }
        return SectorID.SE;
    }
}
