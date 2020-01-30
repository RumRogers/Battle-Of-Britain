using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupermarineSpitfire : BritishAirplane
{
    protected override void Awake()
    {
        base.Awake();
        m_modelID = ModelID.RAF_SUPERMARINE_SPITFIRE;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Attack(GameObject target)
    {
        throw new System.NotImplementedException();
    }

    public override void Evade()
    {
        throw new System.NotImplementedException();
    }
}
