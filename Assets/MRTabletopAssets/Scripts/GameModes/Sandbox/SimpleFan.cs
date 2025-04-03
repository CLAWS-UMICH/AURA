using System.Collections.Generic;
using UnityEngine;
using XRMultiplayer;

public class SimpleFan : MonoBehaviour
{
    [SerializeField]
    float m_ForceEachFrame = 10f;

    [SerializeField]
    SubTrigger m_SubTrigger;

    private HashSet<Rigidbody> m_RigidbodiesInCollider = new HashSet<Rigidbody>();

    void OnEnable() => m_SubTrigger.OnTriggerAction += Triggered;
    void OnDisable() => m_SubTrigger.OnTriggerAction -= Triggered;

    private void Triggered(Collider other, bool entered)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            if (entered)
                m_RigidbodiesInCollider.Add(rb);
            else
                m_RigidbodiesInCollider.Remove(rb);
        }
    }

    void FixedUpdate()
    {
        foreach (Rigidbody rb in m_RigidbodiesInCollider)
        {
            if (rb == null)
            {
                m_RigidbodiesInCollider.Remove(rb);
                return;
            }
            rb.AddForce(transform.forward * m_ForceEachFrame);
        }
    }
}
