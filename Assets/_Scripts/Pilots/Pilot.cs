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
    protected bool m_mustRotate = false;

    private void Awake()
    {
        m_airplane = transform.GetComponent<Airplane>();
        m_itinerary = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MustMove())
        {
            FollowItinerary();
        }
    }

    public void SetItinerary(List<Vector3> itinerary)
    {
        m_itinerary = new List<Vector3>(itinerary);
        m_mustRotate = true;
    }

    protected void FollowItinerary()
    {      
        if(Vector3.Distance(transform.position, m_itinerary[0]) <= Mathf.Epsilon)
        {
            m_itinerary.RemoveAt(0);
            if(m_itinerary.Count == 0)
            {
                return;
            }
            m_mustRotate = true;
        }

        Vector3 currentGoal = m_itinerary[0];
        if(m_mustRotate)
        {
            m_mustRotate = false;
            transform.LookAt(currentGoal);
        }
        
        m_airplane.MoveTo(currentGoal);
    }

    private bool MustMove()
    {
        return m_itinerary.Count > 0;
    }
}
