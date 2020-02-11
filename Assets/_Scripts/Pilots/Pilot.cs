using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pilot : MonoBehaviour
{
    public enum Nationality
    {
        ENGLISH,
        POLISH
    }

    protected float m_discipline = 1f;
    protected string m_firstName = "Nameless";
    protected string m_lastName = "Pilot";
    protected Airplane m_airplane;
    protected List<Vector3> m_itinerary;
    protected bool m_mustMove = false;
    private Vector3 m_currentDestination;
    private void Awake()
    {
        m_airplane = transform.GetComponent<Airplane>();
        m_itinerary = new List<Vector3>();
        m_mustMove = false;
        SetCurrentDestination(transform.position);
        //m_airplane.SetAltitude(m_airplane.m_altitude);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_mustMove)
        {
            if (Vector3.Distance(transform.position, m_currentDestination) > Mathf.Epsilon)
            {
                m_airplane.MoveTo(m_currentDestination);
            }
            else if (m_itinerary.Count > 0)
            {
                SetCurrentDestination(m_itinerary[0]);
                m_itinerary.RemoveAt(0);
            }
        }
        
    }

    private void SetCurrentDestination(Vector3 destination)
    {
        m_currentDestination = destination;
        transform.LookAt(m_currentDestination);
    }

    public void SetItinerary(List<Vector3> itinerary)
    {
        m_itinerary = new List<Vector3>(itinerary);
        SetCurrentDestination(itinerary[0]);
        m_mustMove = true;
    }

    protected void FollowItinerary()
    {      
        if(Vector3.Distance(transform.position, m_itinerary[0]) <= Mathf.Epsilon)
        {
            m_itinerary.RemoveAt(0);
            if(m_itinerary.Count == 0)
            {
                m_mustMove = false;
                return;
            }
        }

        Vector3 currentGoal = m_itinerary[0];
        m_airplane.MoveTo(currentGoal);
    }

    private bool MustMove()
    {
        return m_itinerary.Count > 0;
    }
}
