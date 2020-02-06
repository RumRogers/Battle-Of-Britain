using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTest : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    private Airplane airplane;
    [SerializeField]
    bool m_mouseDown;
    [SerializeField]
    bool m_mouseDrag;
    [SerializeField]
    List<Vector3> m_unfilteredCoordinates = new List<Vector3>();
    List<Vector3> m_worldCoordinates = new List<Vector3>();
    List<PathNode> m_unfilteredPathNodes = new List<PathNode>();
    [SerializeField]
    List<PathNode> m_waypoints = new List<PathNode>();
    Vector3 m_lastCoordinate;
    [SerializeField]
    Vector3 m_mousePos;
    const float m_minDistanceThreshold = 20;

    private void Awake()
    {
        airplane = target.GetComponent<Airplane>();
    }
    // Update is called once per frame
    void Update()
    {
        ManageInput();
        RenderPath();
        airplane.FollowPath();
    }

    void ManageInput()
    {
        //m_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //m_mousePos.y = transform.position.y;
        m_mousePos = Input.mousePosition;

        bool mouseDown = Input.GetMouseButton(0);
        
        if (mouseDown)
        {
            
            if(!m_mouseDown) // User has just pressed mouse.
            {
                HandleMouseDown();
                m_lastCoordinate = m_mousePos;
            }
            else
            {
                if(m_lastCoordinate != m_mousePos) // User is dragging the mouse around.
                {
                    m_mouseDrag = true;
                    //m_unfilteredCoordinates.Add(m_lastCoordinate);
                    m_lastCoordinate = m_mousePos;
                    HandleMouseMove();
                }
            }
        }
        else
        {
            if(m_mouseDown) // User just released mouse button.
            {
                /*m_unfilteredCoordinates.Add(m_lastCoordinate);
                if(m_mousePos != m_lastCoordinate)
                {
                    m_unfilteredCoordinates.Add(m_mousePos);
                }*/

                m_mouseDrag = false;                
                HandleMouseUp();
                //StartCoroutine(MoveTarget());
            }
        }

        m_mouseDown = mouseDown;
    }

    void RenderPath()
    {
        if(m_worldCoordinates.Count > 1)
        {
            for(int i = 1; i < m_worldCoordinates.Count; ++i)
            {                
                Debug.DrawLine(m_worldCoordinates[i - 1], m_worldCoordinates[i], Color.red);    
            }
        }
    }

    IEnumerator MoveTarget()
    {
        yield return null;
        //m_waypoints = Smooth.MakeSmoothCurve(m_waypoints, 1f);
        if (m_waypoints.Count > 1)
        {
            for (int i = 1; i < m_waypoints.Count; ++i)
            {
                //Vector3 dir = (m_waypoints[i] - m_waypoints[i - 1]).normalized;
                //target.position = m_waypoints[i];
                yield return new WaitForSeconds(Time.deltaTime * .5f);
            }
        }
    }

    private void HandleMouseDown()
    {
        StopCoroutine(MoveTarget());
        m_unfilteredCoordinates.Clear();
        m_unfilteredPathNodes.Clear();
        m_waypoints.Clear();
        m_worldCoordinates.Clear();
                
        m_unfilteredCoordinates.Add(m_mousePos);
        PathNode firstNode = new PathNode(m_mousePos, 0);
        m_unfilteredPathNodes.Add(firstNode);
        m_waypoints.Add(firstNode);
    }

    private void HandleMouseMove()
    {
        int length = m_waypoints.Count;
        PathNode lastNode = m_waypoints[length - 1];
        float partialDistance = Vector3.Distance(lastNode.Position, m_mousePos);
        float totalDistance = lastNode.Distance + partialDistance;

        PathNode pathNode = new PathNode(m_mousePos, totalDistance);

        m_unfilteredPathNodes.Add(pathNode);
        m_unfilteredCoordinates.Add(m_mousePos);

        if(partialDistance >= m_minDistanceThreshold)
        {
            m_waypoints.Add(pathNode);
        }
    }

    private void HandleMouseUp()
    {
        PathNode lastMouseMove = m_unfilteredPathNodes[m_unfilteredPathNodes.Count - 1];
        m_unfilteredPathNodes.RemoveAt(m_unfilteredPathNodes.Count - 1);
        int totalNodes = m_waypoints.Count;
        PathNode lastPathNode = m_waypoints[totalNodes - 1];

        if(lastPathNode.Position.x != lastMouseMove.Position.x || lastPathNode.Position.y != lastMouseMove.Position.y)
        {
            m_waypoints.Add(lastMouseMove);
        }

        List<PathNode> finalPath = new List<PathNode>();

        foreach(PathNode pathNode in m_waypoints)
        {
            Vector3 waypointWorldPos = Camera.main.ScreenToWorldPoint(pathNode.Position);
            waypointWorldPos.y = transform.position.y;
            m_worldCoordinates.Add(waypointWorldPos);
            finalPath.Add(new PathNode(waypointWorldPos, pathNode.Distance));
        }

        SplineDescriptor spline = new SplineDescriptor(finalPath);
        airplane.SetCurrentPath(spline);
    }
}