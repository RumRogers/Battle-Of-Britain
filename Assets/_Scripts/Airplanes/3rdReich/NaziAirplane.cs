using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaziAirplane : Airplane
{
    protected override void Awake()
    {
        RadarParticlesSpawner.Instance.AddEnemy(this);
        base.Awake();
    }
    protected override void Update()
    {
        base.Update();
    }
}
