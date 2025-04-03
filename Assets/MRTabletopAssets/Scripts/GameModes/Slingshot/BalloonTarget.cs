using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BalloonTarget : MonoBehaviour, ITarget
{
    [SerializeField]
    float m_floatForce = 10f;

    [SerializeField]
    ParticleSystem m_DestroyParticle;

    [SerializeField]
    GameObject m_DestroyEffectObject;

    [SerializeField]
    float m_maxUpwardVelocity = 5f; // Add a new field for the maximum upward velocity

    [SerializeField]
    float m_speedModifier = 1f;

    [SerializeField]
    float m_MaxHeight = .4f;

    private Rigidbody m_rb;

    Collider m_Collider;

    public Action<Color> OnHitAction { get => m_OnHitAction; set => m_OnHitAction = value; }
    Action<Color> m_OnHitAction;

    IEnumerator Start()
    {
        TryGetComponent(out m_rb);
        m_rb.useGravity = false;

        m_Collider = GetComponentInChildren<Collider>();
        m_Collider.enabled = false;

        yield return new WaitForSeconds(2.0f);
        m_Collider.enabled = true;
    }

    void FixedUpdate()
    {
        m_rb.AddForce(m_floatForce * m_speedModifier * Time.fixedDeltaTime * Vector3.up);

        // Clamp the upward velocity
        var linearVelocity = m_rb.linearVelocity;
        if (m_rb.linearVelocity.y > m_maxUpwardVelocity)
        {
            m_rb.linearVelocity = new Vector3(linearVelocity.x, m_maxUpwardVelocity, linearVelocity.z);
        }
    }

    void LateUpdate()
    {
        if (transform.position.y > m_MaxHeight)
        {
            OnHit(Color.white);
        }
    }

    public void OnHit(Color hitColor)
    {
        OnHitAction?.Invoke(hitColor);

        var main = m_DestroyParticle.main;
        main.startColor = hitColor;

        m_DestroyEffectObject.SetActive(true);
        m_DestroyEffectObject.transform.SetParent(null);

        Destroy(m_DestroyEffectObject, 2f);
        Destroy(gameObject);
    }
}

public interface ITarget
{
    void OnHit(Color hitColor);

    Action<Color> OnHitAction { get; set; }
}
