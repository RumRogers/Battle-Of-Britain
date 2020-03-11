using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerUnitsSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> m_playerUnits;
    [SerializeField]
    private Formation.FormationType m_nextSpawnType = Formation.FormationType.RAF_VIC;
    MouseInteraction m_mouseInteraction;
    List<Formation> m_spawnedFormations = new List<Formation>();

    private void Awake()
    {
        m_mouseInteraction = GameObject.FindGameObjectWithTag("MouseInteraction").GetComponent<MouseInteraction>();
    }

    private void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        Formation.FormationSetup formationSetup = null;

        int idx = -1;

        switch(m_nextSpawnType)
        {
            case Formation.FormationType.RAF_VIC:
                {
                    idx = 0;
                    formationSetup = (GameObject formation) => 
                    {
                        Pilot formationLeader = formation.transform.GetChild(0).GetComponent<Pilot>();
                        Pilot formationUnit1 = formation.transform.GetChild(1).GetComponent<Pilot>();
                        Pilot formationUnit2 = formation.transform.GetChild(2).GetComponent<Pilot>();

                        Formation f = formationLeader.StartFormation(Formation.FormationType.RAF_VIC);

                        formationUnit1.JoinFormation(f);
                        formationUnit2.JoinFormation(f);                        

                        m_spawnedFormations.Add(f);
                    };
                }
            break;
            default:
            break;
        }

        if(idx != -1)
        {
            GameObject formationInstance = Instantiate(m_playerUnits[idx], null);
            formationInstance.transform.position = transform.position;

            formationSetup(formationInstance);
            m_mouseInteraction.UpdateLists();
        }
        
    }
}
