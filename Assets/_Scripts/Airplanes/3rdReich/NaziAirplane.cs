using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaziAirplane : Airplane
{
    private void Start()
    {
        RadarParticlesSpawner.Instance.AddEnemy(this);
    }
    protected override void Update()
    {
        base.Update();
    }
}
