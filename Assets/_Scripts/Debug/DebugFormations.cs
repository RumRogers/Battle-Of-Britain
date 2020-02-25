using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugFormations : MonoBehaviour
{
    public List<Pilot> airplanes;

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 20), "VIC formation"))
        {            
            if(airplanes.Count == 0)
            {
                return;
            }

            Formation f = airplanes[0].StartFormation(Formation.FormationType.RAF_VIC);
            for(int i = 1; i < airplanes.Count; i++)
            {
                airplanes[i].JoinFormation(f);
            }
        }
    }
}
