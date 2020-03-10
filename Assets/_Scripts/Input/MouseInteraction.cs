#define DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
    [SerializeField]
    private float m_mouseDistanceThreshold = 2f;
    [SerializeField]
    private bool m_mouseDown;
    [SerializeField]
    private bool m_mouseDrag;
    [SerializeField]
    private Vector3 m_mousePosition;
    [SerializeField]
    private Pilot m_focusedPilot;
    [SerializeField]
    private GameObject m_waypointMarkerPrefab;

    private List<Pilot> m_playerPilots = new List<Pilot>();
    private Vector3 m_lastMousePosition;
    List<Vector3> m_waypoints = new List<Vector3>();
    List<GameObject> m_waypointMarkers = new List<GameObject>();
    [SerializeField]
    Itinerary m_itinerary;

    // Update is called once per frame
    void Update()
    {
        ManageInput();
#if DEBUG
        PreviewPath();
#endif
    }

    private void Awake()
    {
        UpdateLists();
    }
    void ManageInput()
    {
        m_mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_mousePosition.y = transform.position.y;

        bool mouseDown = Input.GetMouseButton(0);

        if (mouseDown)
        {
            if (!m_mouseDown) // User has just pressed mouse.
            {
                HandleMouseDown(); 
            }
            else
            {
                if (m_lastMousePosition != m_mousePosition) // User is dragging the mouse around.
                {
                    HandleMouseDrag();
                }
            }
        }
        else
        {
            if (m_mouseDown) // User just released mouse button.
            {
                HandleMouseUp();
            }
        }

        m_mouseDown = mouseDown;
    }

    void HandleMouseDown()
    {
        CleanUp();

        if (m_focusedPilot == null)
        {
            m_focusedPilot = GetPilotByPosition(m_mousePosition);
        }
        else
        {
            // manage click for A to B flight
        }

        m_lastMousePosition = m_mousePosition;
    }

    void HandleMouseDrag()
    {
        m_mouseDrag = true;
        
        if(m_focusedPilot != null)
        {
            float minDistanceBetweenPoints = 2f;

            if (m_waypoints.Count == 0 || (m_mousePosition - m_waypoints[m_waypoints.Count - 1]).magnitude >= minDistanceBetweenPoints)
            {               
                m_waypoints.Add(m_lastMousePosition);
                BuildPath();
            }
        }

        m_lastMousePosition = m_mousePosition;
    }

    void HandleMouseUp()
    {
        if(m_focusedPilot != null) // The user has previously pressed LBM on an airplane
        {
            // If he didn't drag the mouse, there are two possible situations
            if (!m_mouseDrag) 
            {
                // If the mouse position is onto an airplane, it means the user just stopped
                // clicking on an airplane. No path is to be followed.
                if(GetPilotByPosition(m_mousePosition) != null)
                {
                    // Do nothing for now, this might change if clicking on an enemy airplane
                }
                else
                {
                    BuildPath();
                    Itinerary it = ScriptableObject.CreateInstance<Itinerary>();
                    it.SetWaypoints(new List<Vector3> { m_mousePosition }, m_focusedPilot.transform.position);
                    //it.waypoints.Add(m_mousePosition);
                    m_focusedPilot.SetItinerary(it);
                    m_focusedPilot = null;
                }
            }
            else
            {
                m_waypoints.Add(m_lastMousePosition);
                if (m_mousePosition != m_lastMousePosition)
                {
                    m_waypoints.Add(m_mousePosition);
                    
                    //BuildPath();
                }

                BuildPath();
                Itinerary it = ScriptableObject.CreateInstance<Itinerary>();
                it.SetWaypoints(m_waypoints, m_focusedPilot.transform.position);

                m_focusedPilot.SetItinerary(it);
                if(m_itinerary != null)
                {
                    m_itinerary.SetWaypoints(m_waypoints, m_focusedPilot.transform.position);
                    //m_itinerary.waypoints = m_waypoints;
                }
                m_focusedPilot = null;
            }
        }

        m_mouseDrag = false;
    }

    void CleanUp()
    {
        foreach (GameObject waypointMarker in m_waypointMarkers)
        {
            Destroy(waypointMarker);
        }
        m_waypointMarkers.Clear();
        m_waypoints.Clear();
    }

    Pilot GetPilotByPosition(Vector3 pos)
    {
        foreach (Pilot p in m_playerPilots)
        {
            pos.y = p.transform.position.y;
            if(Vector3.Distance(p.transform.position, pos) < m_mouseDistanceThreshold)
            {
                return p;
            }            
        }

        return null;
    }

    void InstantiateWaypointMarker(Vector3 pos)
    {
        GameObject waypointMarker = Instantiate(m_waypointMarkerPrefab);
        waypointMarker.transform.position = pos;
        m_waypointMarkers.Add(waypointMarker);
    }

    void BuildPath()
    {
        if(m_waypoints.Count == 0)
        {
            return;
        }

        if (m_waypoints.Count == 1)
        {
            InstantiateWaypointMarker(m_waypoints[0]);
        }
        else
        {
            int count = m_waypoints.Count;
            Vector3 penultimateWaypoint = m_waypoints[count - 2];
            Vector3 lastWaypoint = m_waypoints[count - 1];

            List<Vector3> intermediateWaypoints = GetIntermediateWaypointsBetween(penultimateWaypoint, lastWaypoint, .5f);


            foreach (Vector3 pos in intermediateWaypoints)
            {
                InstantiateWaypointMarker(pos);
            }
        }
    }

    List<Vector3> GetIntermediateWaypointsBetween(Vector3 waypointA, Vector3 waypointB, float sizeOfEachLine)
    {
        List<Vector3> res = new List<Vector3>();
        Vector3 v = waypointB - waypointA;
        int partitionSize = (int)(v.magnitude / sizeOfEachLine);
        Vector3 oneOverPartitionSize = v / partitionSize;

        for (int i = 1; i < partitionSize; i++)
        {
            res.Add(waypointA + (i * oneOverPartitionSize));
        }

        return res;
    }

    void PreviewPath()
    {
        if (m_waypoints.Count > 1)
        {
            for (int i = 1; i < m_waypoints.Count; ++i)
            {
                Debug.DrawLine(m_waypoints[i - 1], m_waypoints[i], Color.red);
            }
        }
    }

    public void UpdateLists()
    {
        GameObject[] playerAirplanes = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject gameObject in playerAirplanes)
        {
            Pilot pilot = gameObject.GetComponent<Pilot>();
            if (!m_playerPilots.Contains(pilot))
            {
                m_playerPilots.Add(pilot);
            }
        }
    }
}
