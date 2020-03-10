using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitsSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> m_playerUnits;
    [SerializeField]
    private Transform m_spawnPosition;
    [SerializeField]
    private Formation.FormationType m_nextSpawnType = Formation.FormationType.RAF_VIC;
    MouseInteraction m_mouseInteraction;

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
        int idx = -1;

        switch(m_nextSpawnType)
        {
            case Formation.FormationType.RAF_VIC:
                {
                    idx = 0;
                }
            break;
            default:
            break;
        }

        if(idx != -1)
        {
            GameObject formation = Instantiate(m_playerUnits[idx], null);
            formation.transform.position = m_spawnPosition.position;

            m_mouseInteraction.UpdateLists();
        }
        
    }
}
