using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTest : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    bool m_mouseDown;
    [SerializeField]
    bool m_mouseDrag;
    [SerializeField]
    List<Vector3> m_waypoints = new List<Vector3>();
    Vector3 m_lastWaypoint;
    [SerializeField]
    Vector3 m_mousePos;

    // Update is called once per frame
    void Update()
    {
        ManageInput();
        RenderPath();

    }

    void ManageInput()
    {
        m_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_mousePos.y = transform.position.y;

        bool mouseDown = Input.GetMouseButton(0);
        
        if (mouseDown)
        {
            if(!m_mouseDown) // User has just pressed mouse.
            {
                StopCoroutine(MoveTarget());
                m_waypoints.Clear();
                m_lastWaypoint = m_mousePos;   
            }
            else
            {
                if(m_lastWaypoint != m_mousePos) // User is dragging the mouse around.
                {
                    m_mouseDrag = true;
                    m_waypoints.Add(m_lastWaypoint);
                    m_lastWaypoint = m_mousePos;
                }
            }
        }
        else
        {
            if(m_mouseDown) // User just released mouse button.
            {
                m_waypoints.Add(m_lastWaypoint);
                if(m_mousePos != m_lastWaypoint)
                {
                    m_waypoints.Add(m_mousePos);
                }

                m_mouseDrag = false;

                StartCoroutine(MoveTarget());
            }
        }

        m_mouseDown = mouseDown;
    }

    void RenderPath()
    {
        if(m_waypoints.Count > 1)
        {
            for(int i = 1; i < m_waypoints.Count; ++i)
            {                    
                Debug.DrawLine(m_waypoints[i - 1], m_waypoints[i], Color.red);    
            }
        }
    }

    IEnumerator MoveTarget()
    {
        m_waypoints = Smooth.MakeSmoothCurve(m_waypoints, 1f);
        if (m_waypoints.Count > 1)
        {
            for (int i = 1; i < m_waypoints.Count; ++i)
            {
                //Vector3 dir = (m_waypoints[i] - m_waypoints[i - 1]).normalized;
                target.position = m_waypoints[i];
                yield return new WaitForSeconds(Time.deltaTime * .5f);
            }
        }
    }
}