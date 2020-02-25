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
    private Vector3 m_currentDestination;
    [SerializeField]
    protected Itinerary m_itinerary;
    private Vector3 m_itineraryOrigin;
    protected Formation m_currentFormation;

    private void Awake()
    {
        m_airplane = transform.GetComponent<Airplane>();
        SetCurrentDestination(transform.position);
        if (m_itinerary != null)
        {
            SetItinerary(m_itinerary, m_itinerary.loop);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(MustMove())
        {
            if (Vector3.Distance(transform.position, m_currentDestination) > Mathf.Epsilon)
            {
                m_airplane.MoveTo(m_currentDestination);
            }
            else if (m_itinerary != null && m_itinerary.idx < m_itinerary.waypoints.Count)
            {
                SetCurrentDestination(m_itineraryOrigin + m_itinerary.waypoints[m_itinerary.idx++], true);
                if (m_itinerary.idx == m_itinerary.waypoints.Count)
                {
                    if (m_itinerary.loop)
                    {
                        m_itinerary.idx = 0;
                    }
                    else
                    {
                        m_itinerary = null;
                    }
                }
            }
        }
        else if (m_currentFormation != null)
        {            
            SetCurrentDestination(m_currentFormation.GetDestination(this), true);
            m_airplane.MoveTo(m_currentDestination);
        }

    }

    private void SetCurrentDestination(Vector3 destination, bool faceDestination = false)
    {
        m_currentDestination = destination;

        if(faceDestination)
        {
            StopCoroutine(m_airplane.RotateSmoothlyTo(m_currentDestination));
            StartCoroutine(m_airplane.RotateSmoothlyTo(m_currentDestination));
        }
    }

    public void SetItinerary(Itinerary itinerary, bool loop = false)
    {
        m_itinerary = Itinerary.CloneItinerary(itinerary);
        m_itinerary.loop = loop;
        m_itineraryOrigin = transform.position;
    }

    public Formation StartFormation(Formation.FormationType formationType)
    {
        switch(formationType)
        {
            case Formation.FormationType.RAF_VIC:
                m_currentFormation = new VICFormation();
                break;
            default:
                throw new System.NotImplementedException();
        }

        JoinFormation(m_currentFormation);

        return m_currentFormation;
    }


    public void JoinFormation(Formation formation)
    {
        formation.AddToFormation(this);
        m_currentFormation = formation;
    }

    private bool MustMove()
    {
        return m_itinerary != null && (m_itinerary.loop || (m_itinerary.idx < m_itinerary.waypoints.Count));
    }
}
