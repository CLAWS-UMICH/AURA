using MRTTT;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using XRMultiplayer;

public class SlingshotArea : NetworkBehaviour, IXRSelectFilter, IXRHoverFilter
{
    [SerializeField] int m_SeatID;
    [SerializeField] XRBaseInteractable m_InteractableArea;

    IXRHoverInteractor[] m_HoverInteractors = new IXRHoverInteractor[2];

    [SerializeField] Transform[] m_HoverVisuals;
    [SerializeField] SlingshotLauncher[] m_SlingshotLaunchers;
    [SerializeField] Collider[] m_CollidersToIgnore;

    [SerializeField] BoxCollider m_BoundsCollider;

    [SerializeField] SlingshotProjectile m_SlingshotProjectilePrefab;
    SlingshotProjectile[] m_SlingshotProjectiles = new SlingshotProjectile[2];
    Transform[] m_PlayerHands = new Transform[2];
    Vector3[] m_HandPreviousFrame = new Vector3[2];

    public bool canProcess => isActiveAndEnabled;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_InteractableArea.hoverEntered.AddListener(HoverEntered);
        m_InteractableArea.hoverExited.AddListener(HoverExited);

        m_InteractableArea.selectEntered.AddListener(SelectEnteredLocal);
        m_InteractableArea.selectExited.AddListener(SelectExitedLocal);

        m_BoundsCollider.gameObject.SetActive(false);

        foreach (var visual in m_HoverVisuals)
        {
            visual.gameObject.SetActive(false);
        }

        foreach (var launcher in m_SlingshotLaunchers)
        {
            launcher.gameObject.SetActive(false);
        }
    }

    public void HideGameMode()
    {
        m_BoundsCollider.gameObject.SetActive(false);
        foreach (var visual in m_HoverVisuals)
        {
            visual.gameObject.SetActive(false);
        }

        foreach (var launcher in m_SlingshotLaunchers)
        {
            launcher.gameObject.SetActive(false);
        }

        foreach (var projectile in m_SlingshotProjectiles)
        {
            if (projectile != null)
            {
                projectile.DestroyProjectile();
            }
        }
    }

    public void FinishRound()
    {
        foreach (var visual in m_HoverVisuals)
        {
            visual.gameObject.SetActive(false);
        }

        foreach (var launcher in m_SlingshotLaunchers)
        {
            launcher.gameObject.SetActive(false);
        }

        foreach (var projectile in m_SlingshotProjectiles)
        {
            if (projectile != null)
            {
                projectile.DestroyProjectile();
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_InteractableArea.hoverEntered.RemoveListener(HoverEntered);
        m_InteractableArea.hoverExited.RemoveListener(HoverExited);
        m_InteractableArea.selectEntered.RemoveListener(SelectEnteredLocal);
        m_InteractableArea.selectExited.RemoveListener(SelectExitedLocal);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_HoverInteractors.Length; i++)
        {
            if (m_HoverInteractors[i] == null)
                continue;

            var hover = m_HoverInteractors[i];
            var attachTransform = ((NearFarInteractor)hover).attachTransform;
            var hitPos = attachTransform.position;
            ICurveInteractionDataProvider curveProvider = (ICurveInteractionDataProvider)hover;
            if (curveProvider.TryGetCurveEndPoint(out Vector3 endPoint) == EndPointType.ValidCastHit)
            {
                hitPos = endPoint;
            }

            m_HoverVisuals[hover.handedness == InteractorHandedness.Left ? 0 : 1].position = hitPos;
        }

        for (int i = 0; i < m_SlingshotProjectiles.Length; i++)
        {
            if (m_SlingshotProjectiles[i] == null || !m_SlingshotProjectiles[i].isLocalPlayerProjectile)
                continue;

            var currentHandPosition = m_PlayerHands[i].position;
            var movementDelta = currentHandPosition - m_HandPreviousFrame[i];
            m_SlingshotLaunchers[i].launchPositionTransform.position += movementDelta;
            m_HandPreviousFrame[i] = currentHandPosition;
        }
    }

    void HoverEntered(HoverEnterEventArgs args)
    {
        int hand = args.interactorObject.handedness == InteractorHandedness.Left ? 0 : 1;
        m_HoverInteractors[hand] = args.interactorObject;
        m_HoverVisuals[hand].gameObject.SetActive(true);
    }

    void SelectEnteredLocal(SelectEnterEventArgs args)
    {
        int hand = args.interactorObject.handedness == InteractorHandedness.Left ? 0 : 1;
        ICurveInteractionDataProvider curveProvider = (ICurveInteractionDataProvider)args.interactorObject;
        var attachTransform = ((NearFarInteractor)args.interactorObject).attachTransform;
        var hitPos = attachTransform.position;
        if (curveProvider.TryGetCurveEndPoint(out Vector3 endPoint) == EndPointType.ValidCastHit)
        {
            hitPos = endPoint;
        }

        m_HoverVisuals[hand].gameObject.SetActive(false);

        m_PlayerHands[hand] = hand == 0 ? XRINetworkPlayer.LocalPlayer.leftHand : XRINetworkPlayer.LocalPlayer.rightHand;
        m_HandPreviousFrame[hand] = m_PlayerHands[hand].position;
        SelectEnteredRpc(hand, hitPos, XRINetworkGameManager.LocalPlayerColor.Value);
    }

    [Rpc(SendTo.Everyone)]
    void SelectEnteredRpc(int slingshotIndex, Vector3 position, Color playerColor)
    {
        // Instantiate the ball and set its position to the slingshot position
        // Clarify for local player
        m_SlingshotProjectiles[slingshotIndex] = Instantiate(m_SlingshotProjectilePrefab, position, Quaternion.identity);
        m_SlingshotProjectiles[slingshotIndex].Setup(TableTop.k_CurrentSeat == m_SeatID, playerColor);

        var projectileCollider = m_SlingshotProjectiles[slingshotIndex].GetComponent<Collider>();
        foreach (var collider in m_CollidersToIgnore)
        {
            Physics.IgnoreCollision(projectileCollider, collider);
        }

        // Local Player Check
        if (XRINetworkGameManager.LocalPlayerColor.Value == playerColor)
        {
            m_SlingshotLaunchers[slingshotIndex].transform.position = position;
            m_SlingshotLaunchers[slingshotIndex].gameObject.SetActive(true);
        }
    }

    void SelectExitedLocal(SelectExitEventArgs args)
    {
        int hand = args.interactorObject.handedness == InteractorHandedness.Left ? 0 : 1;
        m_SlingshotLaunchers[hand].ResetLaunchPosition();
        m_SlingshotLaunchers[hand].gameObject.SetActive(false);

        if (m_BoundsCollider.gameObject.activeInHierarchy)
        {
            m_HoverVisuals[hand].gameObject.SetActive(true);
        }

        SelectExitedRpc(hand, m_SlingshotLaunchers[hand].launchForce);
    }

    [Rpc(SendTo.Everyone)]
    void SelectExitedRpc(int slingshotIndex, Vector3 force)
    {
        if (m_BoundsCollider.gameObject.activeInHierarchy)
        {
            if (m_SlingshotProjectiles[slingshotIndex] != null)
            {
                m_SlingshotProjectiles[slingshotIndex].LaunchProjectile(force);
            }
        }
        else
        {
            if (m_SlingshotProjectiles[slingshotIndex] != null)
            {
                m_SlingshotProjectiles[slingshotIndex].DestroyProjectile();
            }
        }
        m_SlingshotProjectiles[slingshotIndex] = null;
    }

    void HoverExited(HoverExitEventArgs args)
    {
        int hand = args.interactorObject.handedness == InteractorHandedness.Left ? 0 : 1;
        m_HoverVisuals[hand].gameObject.SetActive(false);
        m_HoverInteractors[hand] = null;
    }

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        return TableTop.k_CurrentSeat == m_SeatID;
    }

    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
    {
        return TableTop.k_CurrentSeat == m_SeatID;
    }
}
