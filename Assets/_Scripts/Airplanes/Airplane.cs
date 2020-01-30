using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Airplane : MonoBehaviour
{
    // sound fx (bullets)
    // controls first
    // maneuverability
    // airplanes might start from airport
    // click A to B
    // click-drag-path

    /// <summary>
    /// The models of airplane that exist within the game.
    /// </summary>
    public enum ModelID
    {
        RAF_SUPERMARINE_SPITFIRE,
        RAF_HAWKER_HURRICANE,
        RAF_BOULTON_PAUL_DEFIANT,
        RAF_BRISTOL_BLENHEIM,
        NAZI_HEINKELL_111P,
        NAZI_MESSERSCHMITT_BF109_E4,
        NAZI_MESSERSCHMITT_BF110,
        NAZI_JUNKERS_JU_87_STUKA, // Sturzkampfflugzeug
        NAZI_DORNIER_DO_17, // Fliegender Bleistift
        NAZI_JUNKERS_JU_87
    }

    public enum State
    {
        CRUISING,
        DIVING,
        CLIMBING
        // more to come...
    }

    [SerializeField]
    protected ModelID m_modelID;
    [SerializeField]
    protected float m_health;
    [SerializeField]
    private Core.Common.Altitude m_altitude;
    private Transform m_shadow;
    private Vector3 m_shadowOffset;
    private Vector3 m_originalScale;
    private State m_state = State.CRUISING;

    protected virtual void Awake()
    {
        foreach(Transform transform in transform)
        {
            if(transform.CompareTag("Shadow"))
            {
                m_shadow = transform;
                break;
            }
        }

        SetAltitude(m_altitude);
    }

    protected virtual void Update()
    {
        //print(m_shadow.position - transform.position);
        m_shadow.position = transform.position + m_shadowOffset;
    }

    /// <summary>
    /// Overridable by each child class, normally just subtracts damage points from the health variable.
    /// </summary>
    /// <param name="damage">The amount of damage the plane is taking.</param>
    protected virtual void TakeDamage(float damage)
    {
        m_health -= damage;
    }

    /// <summary>
    /// Changes the y-coordinate of the airplane.
    /// </summary>
    /// <param name="altitude">The new altitude, specified as an enum type.</param>
    public void SetAltitude(Core.Common.Altitude altitude)
    {
        m_altitude = altitude;
        Vector3 pos = transform.position;
        pos.y = Core.Common.altitudeMap[altitude];
        transform.position = pos;
        //m_shadow.localPosition = Core.Common.shadowOffsetsMap[altitude];
        m_shadowOffset = Core.Common.shadowOffsetsMap[altitude];
    }

    /// <summary>
    /// Prints the airplane's model.
    /// </summary>
    public override string ToString()
    {
        return "Model: " + Core.Common.airplaneModelMap[m_modelID];
    }

    public void Dive(Core.Common.Altitude targetAltitude)
    {
        StartCoroutine(ChangeAltitude(targetAltitude));
    }

    public void Climb(Core.Common.Altitude targetAltitude)
    {
        StartCoroutine(ChangeAltitude(targetAltitude));
    }

    IEnumerator ChangeAltitude(Core.Common.Altitude targetAltitude)
    {
        float speed = .75f;
        float sourceY = transform.position.y;
        float destY = Core.Common.altitudeMap[targetAltitude];
        float t = 0f;
        Vector3 tmp;
        Vector3 sourceShadowOffset = m_shadowOffset;
        Vector3 targetShadowOffset = Core.Common.shadowOffsetsMap[targetAltitude];
        Vector3 sourceScale = transform.localScale;
        Vector3 targetScale = Core.Common.scaleMap[targetAltitude];

        while(transform.position.y != destY)
        {
            t += Time.deltaTime * speed;
            tmp = transform.position;
            tmp.y = Mathf.Lerp(sourceY, destY, t);
            m_shadowOffset = Vector3.Lerp(sourceShadowOffset, targetShadowOffset, t);            
            transform.position = tmp;
            transform.localScale = Vector3.Lerp(sourceScale, targetScale, t);

            yield return new WaitForEndOfFrame();
        }

        m_altitude = targetAltitude;
        gameObject.layer = LayerMask.NameToLayer(Core.Common.layerMasksMap[targetAltitude]);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 20), "DIVE MEDIUM"))
        {
            Dive(Core.Common.Altitude.MEDIUM);
        }
        else if (GUI.Button(new Rect(210, 0, 200, 20), "DIVE LOW"))
        {
            Dive(Core.Common.Altitude.LOW);
        }        
        else if (GUI.Button(new Rect(0, 30, 200, 20), "CLIMB MEDIUM"))
        {
            Climb(Core.Common.Altitude.MEDIUM);
        }
        else if (GUI.Button(new Rect(210, 30, 200, 20), "CLIMB HIGH"))
        {
            Climb(Core.Common.Altitude.HIGH);
        }

    }
}
