using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

[RequireComponent(typeof(XRGrabInteractable))]
public class TableManipulator : MonoBehaviour
{
    [SerializeField]
    protected GameObject m_TableVisualsObject;

    [SerializeField]
    protected TableTop m_TableTop;
    protected XROrigin m_XROrigin;
    protected TeleportationProvider m_TeleportationProvider;
    protected Transform m_Head;

    protected XRGrabInteractable m_GrabInteractable;

    protected TableSeatSystem m_TableSeatSystem;

    Rigidbody m_Rigidbody;

    Vector3 m_InitialTablePosition;
    Quaternion m_InitialTableRotation;
    Vector3 m_InitialHeadPosition;
    float m_InitialSeatOffset;
    Vector3 m_InitialTableLocalPos;

    void Awake()
    {
        m_InitialTableLocalPos = transform.localPosition;
    }

    protected virtual void Start()
    {
        m_GrabInteractable = GetComponent<XRGrabInteractable>();
        m_TableSeatSystem = GetComponentInParent<TableSeatSystem>();
        m_Rigidbody = GetComponent<Rigidbody>();

        m_XROrigin = FindFirstObjectByType<XROrigin>();
        m_Head = m_XROrigin.Camera.transform;

        m_TeleportationProvider = m_XROrigin.GetComponentInChildren<TeleportationProvider>();
        m_TableVisualsObject.SetActive(false);

        m_GrabInteractable.firstSelectEntered.AddListener(StartSelection);
        m_GrabInteractable.lastSelectExited.AddListener(EndSelection);
    }

    void OnDestroy()
    {
        if (m_GrabInteractable == null)
            return;
        m_GrabInteractable.firstSelectEntered.RemoveListener(StartSelection);
        m_GrabInteractable.lastSelectExited.RemoveListener(EndSelection);
    }

    Matrix4x4 m_InitialTableTransform;
    Matrix4x4 m_InitialPlayerTransform;

    public void StartSelection(SelectEnterEventArgs args)
    {
        m_InitialTableTransform = transform.localToWorldMatrix;
        m_InitialPlayerTransform = m_XROrigin.transform.localToWorldMatrix;
        m_TableVisualsObject.SetActive(true);
    }

    public void EndSelection(SelectExitEventArgs args)
    {
        if (m_GrabInteractable.isSelected)
            return;

        m_TableVisualsObject.SetActive(false);

        _ = MovePlayer();
    }

    public async Awaitable MovePlayer()
    {
        // Wait for the next fixed update for the table to be updated
        await Awaitable.FixedUpdateAsync();

        // Compute the final table transform
        Matrix4x4 finalTableTransform = transform.localToWorldMatrix;

        // Compute the table's transform delta
        Matrix4x4 tableTransformDelta = finalTableTransform * m_InitialTableTransform.inverse;

        // Compute the inverse of the table's transform delta
        Matrix4x4 inverseTableTransformDelta = tableTransformDelta.inverse;

        // Apply the inverse of the table's transform delta to the player's transform
        Matrix4x4 newPlayerTransform = inverseTableTransformDelta * m_InitialPlayerTransform;

        // Update seat offset if needed
        UpdateSeatOffset();

        // Reset the table's position and rotation
        transform.localPosition = m_InitialTableLocalPos;
        transform.localRotation = Quaternion.identity;

        // Move rigitbody to match the transform
        m_Rigidbody.MovePosition(transform.position);
        m_Rigidbody.MoveRotation(transform.rotation);

        // Update the player's position and rotation
        m_XROrigin.transform.position = newPlayerTransform.GetColumn(3);
        m_XROrigin.transform.rotation = Quaternion.LookRotation(
            newPlayerTransform.GetColumn(2),
            newPlayerTransform.GetColumn(1)
        );
    }

    void UpdateSeatOffset()
    {
        // Get the current seat's forward direction
        Vector3 seatForward = m_TableTop.GetSeat(TableTop.k_CurrentSeat).forward;

        // Calculate the vector from the table center to the head
        Vector3 tableToHead = m_Head.position - transform.position;

        // Project this vector onto the plane defined by the table's up vector
        //Vector3 projectedTableToHead = Vector3.ProjectOnPlane(tableToHead, Vector3.up);

        // Calculate the signed distance along the seat's forward direction
        //float signedDistance = Vector3.Dot(projectedTableToHead, seatForward);

        // Calculate the new seat offset
        float newSeatOffset = Vector3.Project(-tableToHead, seatForward).magnitude - m_TableTop.seatDistance;

        // Update the table top's seat offset
        m_TableTop.seatOffset = newSeatOffset;
    }
}
