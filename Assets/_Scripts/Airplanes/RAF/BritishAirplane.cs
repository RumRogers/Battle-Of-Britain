//#define DEBUG_DRAW_RANGES

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BritishAirplane : Airplane
{
    [SerializeField]
    protected float m_visibleRangeRadius;
    [SerializeField]
    protected float m_weaponsRangeRadius;

#if DEBUG_DRAW_RANGES
    private LineRenderer m_lineRenderer;
    [SerializeField]
    private Material m_lineRendererMaterial;
#endif

    protected override void Awake()
    {
        base.Awake();
#if DEBUG_DRAW_RANGES
        m_lineRenderer = gameObject.AddComponent<LineRenderer>() as LineRenderer;
        m_lineRenderer.useWorldSpace = true;
        m_lineRenderer.startWidth = 1;
        m_lineRenderer.endWidth = 1;
        m_lineRenderer.positionCount = 360;
        m_lineRenderer.material = m_lineRendererMaterial;
#endif
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
#if DEBUG_DRAW_RANGES
        DebugDrawRanges(m_visibleRangeRadius);
#endif
    }

    /// <summary>
    /// Abstract method, to be implemented differently for each model of airplane.
    /// </summary>
    /// <param name="target">The game entity the airplane should attack.</param>
    public abstract void Attack(GameObject target);

    /// <summary>
    /// Abstract method, to be implemented differently for each model of airplane.
    /// </summary>
    public abstract void Evade();

    /// <summary>
    /// Returns true if the input point is within shooting range.
    /// </summary>
    /// <param name="pos">The space coordinates that are being checked.</param>
    /// <returns></returns>
    public bool IsPointInWeaponsRange(Vector3 pos)
    {        
        return pos.y == transform.position.y && (Vector3.Distance(transform.position, pos) < m_weaponsRangeRadius);
    }

    /// <summary>
    /// Returns true if the input point is NOT within shooting range but is in visible range.
    /// </summary>
    /// <param name="pos">The space coordinates that are being checked.</param>
    /// <returns></returns>
    public bool IsPointInVisibleRange(Vector3 pos)
    {        
        return pos.y == transform.position.y && (Vector3.Distance(transform.position, pos) < m_visibleRangeRadius);
    }

    /// <summary>
    /// Helper function, refer to IsPlaneInWeaponsRange(Vector3 pos).
    /// </summary>
    /// <param name="plane">The plane (or generic object) that is being checked.</param>
    /// <returns></returns>
    public bool IsPlaneInWeaponRange(GameObject plane)
    {
        return IsPointInWeaponsRange(plane.transform.position);
    }

    /// <summary>
    /// Helper function, refer to IsPlaneInVisibleRange(Vector3 pos).
    /// </summary>
    /// <param name="plane">The plane (or generic object) that is being checked.</param>
    /// <returns></returns>
    public bool IsPlaneInVisibleRange(GameObject plane)
    {
        return IsPointInVisibleRange(plane.transform.position);
    }

    /// <summary>
    /// Helper function, refer to IsPlaneInWeaponsRange(Vector3 pos).
    /// </summary>
    /// <param name="t">The transform of the plane (or generic object) that is being checked.</param>
    /// <returns></returns>
    public bool IsPlaneInWeaponsRange(Transform t)
    {
        return IsPointInWeaponsRange(t.position);
    }

    /// <summary>
    /// Helper function, refer to IsPlaneInVisibleRange(Vector3 pos).
    /// </summary>
    /// <param name="t">The transform of the plane (or generic object) that is being checked.</param>
    /// <returns></returns>
    public bool IsPlaneInVisibleRange(Transform t)
    {
        return IsPointInVisibleRange(t.position);
    }

    /// <summary>
    /// Prints what faction this airplane belongs to, and its model.
    /// </summary>
    public override string ToString()
    {
        return "RAF airplane. " + base.ToString() + ".";
    }

#if DEBUG_DRAW_RANGES
    void DebugDrawRanges(float radius)
    {
        Vector3 v = Vector3.forward * radius;

        for(int i = 0; i < m_lineRenderer.positionCount; i += 2)
        {
            m_lineRenderer.SetPosition(i, transform.position - (Vector3.up * 0.5f));       
            
            m_lineRenderer.SetPosition(i + 1,  transform.position - (Vector3.up * 0.5f) + Quaternion.AngleAxis(i, Vector3.up) * v);
        }

    }
#endif
}