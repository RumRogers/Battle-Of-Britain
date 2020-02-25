using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VICFormation : Formation
{
    public VICFormation() : base(FormationType.RAF_VIC)
    {
        m_formationOffset = new Vector3(4.8f, 0f, -3.6f);
    }

    protected override Pilot GetUnitLeader(Pilot p)
    {
        int idxInFormation = m_formationUnits.IndexOf(p);

        switch (idxInFormation)
        {
            case 0:
                return null;
            case 1:
            case 2:
                return m_formationUnits[0];
            default:
                return m_formationUnits[idxInFormation - 2];
        }
    }

    protected override Vector3 GetOffsetFromUnitLeader(Pilot p)
    {
        int idxInFormation = m_formationUnits.IndexOf(p);
        Vector3 offset = m_formationOffset;

        if (idxInFormation % 2 == 1)
        {
            offset.x *= -1;
        }

        return offset;
    }
}
