using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class TableSeatSystem : MonoBehaviour
{
    public TableTop tableTop => m_TableTop;

    [SerializeField]
    protected TableTop m_TableTop;

    [SerializeField]
    float m_DefaultSeatHeight = -.5f;

    [SerializeField]
    UnityEvent<int> m_OnSeatChanged;

    XROrigin m_XROrigin;

    void Awake()
    {
        FindReferences();
    }

    void FindReferences()
    {
        m_XROrigin = FindFirstObjectByType<XROrigin>();
    }

    public void TeleportToSeat(int seatNum)
    {
        // Check for spectator seat or initial seat
        if (TableTop.k_CurrentSeat < 0)
        {
            TableTop.k_CurrentSeat = 0;
        }

        int prevSeat = TableTop.k_CurrentSeat;
        TableTop.k_CurrentSeat = seatNum;

        float currentAngle = GetRotationAngleBasedOnSeatNum(prevSeat);
        float newAngle = GetRotationAngleBasedOnSeatNum(seatNum);
        float rotationAmount = newAngle - currentAngle;
        m_XROrigin.transform.RotateAround(transform.position, transform.up, rotationAmount);
        m_OnSeatChanged.Invoke(seatNum);

        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    float GetRotationAngleBasedOnSeatNum(int seatNum)
    {
        float angle = 0;
        switch (seatNum)
        {
            case 1:
                angle = 180;
                break;
            case 2:
                angle = 270;
                break;
            case 3:
                angle = 90;
                break;
        }
        return angle;
    }

    public void ResetSeatRotation()
    {
        Vector3 headForward = new Vector3(m_XROrigin.transform.forward.x, 0, m_XROrigin.transform.forward.z);
        Vector3 seatForward = new Vector3(m_TableTop.GetSeat(TableTop.k_CurrentSeat).forward.x, 0, m_TableTop.GetSeat(TableTop.k_CurrentSeat).forward.z);
        float angle = Vector3.SignedAngle(headForward, seatForward, Vector3.up);

        m_XROrigin.transform.RotateAround(transform.position, transform.up, angle);
    }

    public void ResetToSeatDefault()
    {
        var seat = m_TableTop.GetSeat(TableTop.k_CurrentSeat);

        var seatPosition = seat.position;

        seatPosition.y -= m_DefaultSeatHeight;

        if (m_XROrigin == null)
            FindReferences();

        var targetPosition = seatPosition - seat.forward * m_TableTop.seatOffset;
        var targetRotation = seat.rotation;
        m_XROrigin.transform.SetPositionAndRotation(targetPosition, targetRotation);

        m_TableTop.seatOffset = 0;
    }
}
