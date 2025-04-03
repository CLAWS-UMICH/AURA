using UnityEngine;

public class SmallScalePhysicsModifier : MonoBehaviour
{
    [SerializeField]
    float m_GravityModifier = 0.5f;

    [SerializeField] Rigidbody m_Rigidbody;

    void Start()
    {
        if (m_Rigidbody == null)
            TryGetComponent(out m_Rigidbody);
    }

    void FixedUpdate()
    {
        if (m_Rigidbody != null)
        {
            m_Rigidbody.useGravity = false;
            m_Rigidbody.AddForce(Physics.gravity * m_GravityModifier, ForceMode.Acceleration);
        }
    }
}
