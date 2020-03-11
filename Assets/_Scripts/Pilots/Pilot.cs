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

    public Itinerary itinerary { get { return m_itinerary; } }

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
            Pilot unitLeader = m_currentFormation.GetUnitLeader(this);

            Vector3 dest = m_currentFormation.GetDestination(this);

            float distance = Vector3.Distance(dest, transform.position);
            bool faceDestination = distance > 2f;
            SetCurrentDestination(dest, faceDestination);
            
            if(unitLeader == null)
            {
                return;
            }

            SetSpeed(2 * m_airplane.MaxSpeed);

            if(!faceDestination)
            {                
                RotateTo(transform.position + unitLeader.transform.forward);
                //SetSpeed(1000);
                //unitLeader.SetSpeed(1000);
                //transform.rotation = m_currentFormation.GetUnitLeader(this).transform.rotation;
            }
            else if(unitLeader != null && distance > 3f)
            {
                //unitLeader.SetSpeed(m_airplane.GetSpeed() / 2f, true);
                //SetSpeed(1000);
            }
            
            m_airplane.MoveTo(m_currentDestination);
        }

    }

    private void SetCurrentDestination(Vector3 destination, bool faceDestination = false)
    {
        m_currentDestination = destination;

        if(faceDestination)
        {
            RotateTo(destination);
        }
    }

    private void RotateTo(Vector3 destination)
    {
        StopCoroutine(m_airplane.RotateSmoothlyTo(destination));
        StartCoroutine(m_airplane.RotateSmoothlyTo(destination));
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

    public void SetSpeed(float targetSpeed, bool smooth = false)
    {
        //targetSpeed = Mathf.Clamp(targetSpeed, 0, m_airplane.MaxSpeed);

        if (smooth)
        {
            StopCoroutine(m_airplane.ReachSpeed(targetSpeed));
            StartCoroutine(m_airplane.ReachSpeed(targetSpeed));
        }
        else
        {
            m_airplane.SetSpeed(targetSpeed);
        }

    }

    public float GetDistanceOffset()
    {
        if (this == m_currentFormation.GetFormationLeader())
        {
            return 1.0f;
        }

        else
        {
            float leaderDist = (m_currentFormation.GetFormationLeader().itinerary.waypoints[0] - m_currentFormation.GetFormationLeader().transform.position).magnitude;

            float planeDist = (m_currentDestination - transform.position).magnitude;

            //Function for calculating percentage speed reduction here

            return 1.0f;
        }

    }
}
