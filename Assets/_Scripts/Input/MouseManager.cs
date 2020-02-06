using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private List<BritishAirplane> m_playerAirplanes;
    private int m_skyHighLayer;
    private int m_skyMediumLayer;
    private int m_skyLowLayer;
    
    private void Awake()
    {
        m_skyHighLayer = LayerMask.GetMask("SkyHigh");
        m_skyMediumLayer = LayerMask.GetMask("SkyMedium");
        m_skyLowLayer = LayerMask.GetMask("SkyLow");

        m_playerAirplanes = new List<BritishAirplane>();
        GameObject[] playerAirplanes = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject gameObject in playerAirplanes)
        {
            m_playerAirplanes.Add(gameObject.GetComponent<BritishAirplane>());
        }
    }

    // TODO: keep the list sorted by altitude (descending order) at any time
    private BritishAirplane FindAirplaneByPosition(Vector3 pos)
    {
        foreach(BritishAirplane airplane in m_playerAirplanes)
        {
            if(airplane.IsPointInWeaponsRange(pos))
            {
                //print("Point is in WEAPONS range.");
                return airplane;
            }
            if(airplane.IsPointInVisibleRange(pos))
            {
                //print("Point is in VISIBLE range.");
                return airplane;
            }
        }

        return null;
    }

    Vector3 GetSkyLayerHitPoint(int layer)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, layer))
            Debug.DrawLine(ray.origin, hit.point);


        //print(hit.collider.transform.position);
        Vector3 result = hit.point;
        result.y = hit.collider.transform.position.y;
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void OnMouseDown()
    {
        print("MouseDown");
        return;
        for (int i = 0; i < 3; i++)
        {
            int layer;

            if (i == 0)
            {
                layer = m_skyHighLayer;
            }
            else if (i == 1)
            {
                layer = m_skyMediumLayer;
            }
            else
            {
                layer = m_skyLowLayer;
            }
            BritishAirplane hovering = FindAirplaneByPosition(GetSkyLayerHitPoint(layer));
            if (hovering != null)
            {
                print("Clicked on " + hovering.name + " at sky layer " + i);
                return;
            }
        }
    }
}
