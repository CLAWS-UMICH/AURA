using UnityEngine;

public class SnapBillboard : MonoBehaviour
{
    [SerializeField]
    bool m_AlwaysUpdate = true;

    [SerializeField]
    bool m_FlipForward = true;

    [SerializeField]
    float m_RotationSpeed = 5f;

    [SerializeField]
    bool m_LerpToPosition = false;

    [SerializeField]
    float m_SnapAngle = 90f;

    [SerializeField]
    bool m_UsePositionalSnap = false;

    [SerializeField]
    Vector3 m_RotationOffset;

    protected Camera m_Camera;

    private void Awake()
    {
        m_Camera = Camera.main;
    }

    void Update()
    {
        if (m_AlwaysUpdate)
            UpdateRotation();
    }

    public void UpdateRotation()
    {
        Vector3 forward = m_Camera.transform.forward;
        if (m_UsePositionalSnap)
        {
            if (m_FlipForward)
                forward = (transform.position - m_Camera.transform.position).normalized;
            else
                forward = (m_Camera.transform.position - transform.position).normalized;
        }
        float angle = Mathf.Atan2(forward.z, forward.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(angle / m_SnapAngle) * m_SnapAngle;
        float radians = snappedAngle * Mathf.Deg2Rad;
        Vector3 snappedForward = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians));

        if (m_LerpToPosition)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(snappedForward), Time.deltaTime * m_RotationSpeed);
        else
            transform.rotation = Quaternion.LookRotation(snappedForward);

        transform.Rotate(m_RotationOffset);
    }
}
