using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : MonoBehaviour
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
    private float m_speed;
    [SerializeField]
    protected float m_health;
    [SerializeField]
    public Core.Common.Altitude m_altitude;
    [SerializeField]
    private float m_yawSpeed = 5f;
    private Transform m_modelSprite;
    private Transform m_shadow;
    private Vector3 m_shadowOffset;
    private State m_state = State.CRUISING;

    protected virtual void Awake()
    {
        foreach(Transform t in transform)
        {
            if(t.CompareTag("ModelSprite"))
            {
                m_modelSprite = t;
                break;
            }
        }

        foreach (Transform t in m_modelSprite)
        {
            if (t.CompareTag("Shadow"))
            {
                m_shadow = t;
                break;
            }
        }

        SetAltitude(m_altitude);
    }

    protected virtual void Update()
    {         
        m_shadow.position = m_modelSprite.position + m_shadowOffset;        
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
        m_shadowOffset = Core.Common.shadowOffsetsMap[altitude];
        transform.localScale = Core.Common.scaleMap[altitude];
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
        float t = 0f;
        Vector3 sourceShadowOffset = m_shadowOffset;
        Vector3 targetShadowOffset = Core.Common.shadowOffsetsMap[targetAltitude];
        Vector3 sourceScale = transform.localScale;
        Vector3 targetScale = Core.Common.scaleMap[targetAltitude];

        while (t < 1)
        {
            t += Time.deltaTime * speed;
            m_shadowOffset = Vector3.Lerp(sourceShadowOffset, targetShadowOffset, t);
            transform.localScale = Vector3.Lerp(sourceScale, targetScale, t);

            yield return new WaitForEndOfFrame();
        }

        m_altitude = targetAltitude;
        gameObject.layer = LayerMask.NameToLayer(Core.Common.layerMasksMap[targetAltitude]);
    }

    public IEnumerator RotateSmoothlyTo(Vector3 destination)
    {
        if(destination == transform.position)
        {
            yield break;
        }

        Quaternion sourceRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation((destination - transform.position).normalized, Vector3.up);
        
        float t = 0f;

        while(t < 1)
        {
            t += Time.deltaTime * m_yawSpeed;            
            transform.rotation = Quaternion.Lerp(sourceRotation, targetRotation, t);
            yield return new WaitForEndOfFrame();
        }
        
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 20), "FLY MEDIUM"))
        {
            Dive(Core.Common.Altitude.MEDIUM);
        }
        else if (GUI.Button(new Rect(210, 0, 200, 20), "FLY LOW"))
        {
            Dive(Core.Common.Altitude.LOW);
        }
        else if (GUI.Button(new Rect(0, 30, 200, 20), "FLY HIGH"))
        {
            Climb(Core.Common.Altitude.HIGH);
        }
        else if (GUI.Button(new Rect(210, 30, 200, 20), "LAND"))
        {
            Climb(Core.Common.Altitude.GROUNDED);
        }
    }

    public virtual void MoveTo(Vector3 destination)
    {
        Vector3 dir = (destination - transform.position).normalized;
        Vector3 v = dir * m_speed * Time.deltaTime;

        if (Vector3.Distance(transform.position + v, destination) >= Vector3.Distance(transform.position, destination))
        {
            transform.position = destination;
        }
        else
        {
            transform.position += dir * m_speed * Time.deltaTime;
        }
    }
}
