using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Formation
{
    public enum FormationType
    {
        RAF_VIC,
        NAZI_SCHWARM
    }

    static Dictionary<FormationType, Vector3> FormationOffsets = new Dictionary<FormationType, Vector3>()
    {
        { FormationType.RAF_VIC, new Vector3(4.8f, 0f, -3.6f) }
    };

    public Formation(FormationType formationType)
    {
        m_formationType = formationType;
    }

    protected List<Pilot> m_formationUnits = new List<Pilot>();
    protected Vector3 m_formationOffset;
    private FormationType m_formationType;

    public void AddToFormation(Pilot p)
    {
        m_formationUnits.Add(p);
    }

    public Vector3 GetDestination(Pilot p)
    {
        return GetPositionRelativeToLeader(p, GetUnitLeader(p), GetOffsetFromUnitLeader(p));
    }

    protected abstract Pilot GetUnitLeader(Pilot p);
    protected abstract Vector3 GetOffsetFromUnitLeader(Pilot p);

    protected Vector3 GetPositionRelativeToLeader(Pilot unit, Pilot leader, Vector3 offset)
    {
        if(leader == null)
        {
            return unit.transform.position;
        }

        Vector3 pos = leader.transform.position;
        pos += leader.transform.right * offset.x * unit.transform.localScale.x;
        pos += leader.transform.forward * offset.z * unit.transform.localScale.x;

        Debug.DrawLine(leader.transform.position, pos, Color.red);

        return pos;   
    }
}
