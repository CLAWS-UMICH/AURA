using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityLabs.SmartUX.Network
{
    /// <summary>
    /// Implements behaviour to let a user claim ownership over the NetworkBehaviour and not
    /// allow any other client to claim ownership until the original claimant releaseses ownership.
    ///
    /// This logic is necessary because NetworkObject.RemoveOwnership() sets the ownerId back to 0, which is the Server,
    /// so if the Server is also the Host there is no way for the clients to know if the Host has attempted to claim
    /// ownership or if no one currently had ownership.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class ClaimableNetworkBehaviour : NetworkBehaviour
    {
        [Header("Interaction Config")]
        [SerializeField]
        bool m_AutoClaimOnHover = true;

        [SerializeField]
        bool m_AutoClaimOnSelect = true;

        [SerializeField]
        bool m_AutoClaimOnCollision = false;

        [SerializeField]
        [Tooltip("Timeout before ownership is released after interaction ends")]
        float m_OwnershipReleaseDelay = 0f;

        [SerializeField]
        XRBaseInteractable m_InteractionReceiver;

        public event Action<ClaimableNetworkBehaviour> onNetworkSpawn;
        public event Action<ClaimableNetworkBehaviour> onNetworkDespawn;

        bool m_LocalClaimed;
        Coroutine m_ReleaseCoroutine;

        // Only sync a bool because you can technically discern the ownerId LocalClientId
        // Could this be an Unmanaged.U1 type?
        readonly NetworkVariable<bool> m_ReplicatedClaimedOwnership = new();

        public bool isOwnershipClaimedLocally => m_LocalClaimed || (IsOwner && m_ReplicatedClaimedOwnership.Value);

        // Go off locally claimed bool to be more responsive if client is owner.
        public bool isOwnershipClaimed => isOwnershipClaimedLocally || m_ReplicatedClaimedOwnership.Value;

        public bool isOwnedByServerAndNotClaimed => !m_ReplicatedClaimedOwnership.Value && IsOwnedByServer;

        public bool isOwnedRemotely => !isOwnershipClaimedLocally && m_ReplicatedClaimedOwnership.Value;

        public bool isServerAndIsOwnedByServer => IsServer && isOwnedByServerAndNotClaimed;

        public NetworkVariable<bool> claimedOwnership => m_ReplicatedClaimedOwnership;

        List<ClaimableNetworkBehaviour> m_ClaimedBehaviorsFromCollision = new();

        /// <summary>
        /// Return if this should hold ownership based on conditions set in Inspector.
        /// I.e whether it has an InteractionReceiver and it is set to autoclaim on Hover/Select.
        ///
        /// You can also optionally set this to true and it will explicitly hold ownership until set to false.
        /// </summary>
        public bool ShouldHoldOwnership
        {

            get => m_IsClaimedByProxy || m_ShouldHoldOwnershipOverride ||
                   (m_InteractionReceiver != null &&
                    (m_AutoClaimOnSelect && m_InteractionReceiver.isSelected ||
                     m_AutoClaimOnHover && m_InteractionReceiver.isHovered));
            set => m_ShouldHoldOwnershipOverride = value;
        }

        float m_ReleaseTimer = 0f;
        float m_NetworkDeltaTime = 0f;
        bool m_ShouldHoldOwnershipOverride = false;

        void Reset()
        {
            if (m_InteractionReceiver == null)
                m_InteractionReceiver = GetComponent<XRBaseInteractable>();

            // Intended to switch ownership so you definitely don't want this!
            GetComponent<NetworkObject>().DontDestroyWithOwner = true;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            onNetworkSpawn?.Invoke(this);
            m_NetworkDeltaTime = 1f / NetworkManager.NetworkTickSystem.TickRate;
            NetworkManager.NetworkTickSystem.Tick += OnNetworkTick;
        }

        public override void OnNetworkDespawn()
        {
            if (NetworkManager != null && NetworkManager.NetworkTickSystem != null)
                NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
            onNetworkDespawn?.Invoke(this);
            base.OnNetworkDespawn();
        }

        public void ClaimOwnership()
        {
            m_ReleaseTimer = 0f;

            // Check replicated value, not locally owned,
            // in case for some reason the initial claim attempt didn't
            // go through, it will keep trying until it get synced back
            // from the server
            if (m_ReplicatedClaimedOwnership.Value)
            {
                // SmartLogger.Log($"{name} already claimed.");
                return;
            }

            m_LocalClaimed = true;

            if (NetworkObject.OwnerClientId != NetworkManager.Singleton.LocalClientId)
                ChangeOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        private float m_ReleaseTimeStamp = 0f;
        public void ReleaseOwnership()
        {
            // May have already lost ownership by a race condition
            // of another claiming it, so check first if claimed
            // If we locally unclaimed but after a second we are still the owner, we reset the local flag to release it after
            if (m_LocalClaimed || (isOwnershipClaimedLocally && (Time.unscaledTime - m_ReleaseTimeStamp) > 1))
            {
                ExecuteRelease();
            }
            ClearAllClaimedFromCollision();
        }

        void ExecuteRelease()
        {
            m_LocalClaimed = false;
            RemoveOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
            m_ReleaseTimeStamp = Time.unscaledTime;
            m_ReleaseTimer = 0f;
        }

        [ServerRpc(RequireOwnership = false)]
        void ChangeOwnershipServerRpc(ulong clientId)
        {
            NetworkObject.ChangeOwnership(clientId);
            m_ReplicatedClaimedOwnership.Value = true;
        }

        // Techically RequireOwnership should be true
        // But if you try to claim/unclaim quickly it will give an error
        [ServerRpc(RequireOwnership = false)]
        void RemoveOwnershipServerRpc(ulong clientId)
        {
            // Ensure client trying to realse is one that owns! Possible another
            // client could nab ownership quickly after prior client send release
            if (NetworkObject.OwnerClientId != clientId) return;

            if (!NetworkManager.IsServer)
                NetworkObject.RemoveOwnership();

            m_ReplicatedClaimedOwnership.Value = false;
        }

        public override void OnGainedOwnership()
        {
            // Debug.Log($"{name} OnGainedOwnership");
            base.OnGainedOwnership();

            // This is to catch a potential edge case where a client may start moving a transform
            // but then disconnect and not appropriately call ReleaseOwnership, thus transferring ownership
            // back to the server, and the server will need to clear the ClaimedOwnership NetworkVariable
            if (NetworkManager.IsServer && !m_LocalClaimed)
            {
                m_ReplicatedClaimedOwnership.Value = false;
            }
            m_ReleaseTimer = 0f;
        }

        public override void OnLostOwnership()
        {
            // Debug.Log($"{name} OnLostOwnership");
            base.OnLostOwnership();
            m_LocalClaimed = false;
        }

        void OnCollisionEnter(Collision other)
        {
            if (!m_AutoClaimOnCollision || !isOwnershipClaimed)
                return;
            if (other.gameObject.TryGetComponent(out ClaimableNetworkBehaviour claimableNetworkBehaviour))
            {
                if (!claimableNetworkBehaviour.m_AutoClaimOnCollision)
                    return;
                if (claimableNetworkBehaviour.ClaimByProxy())
                    m_ClaimedBehaviorsFromCollision.Add(claimableNetworkBehaviour);
            }
        }

        bool m_IsClaimedByProxy = false;
        bool ClaimByProxy()
        {
            if (isOwnershipClaimed)
                return false;
            ClaimOwnership();
            m_IsClaimedByProxy = true;
            return true;
        }

        void ProxyRelease()
        {
            ReleaseOwnership();
            m_IsClaimedByProxy = false;
        }

        void ClearAllClaimedFromCollision()
        {
            if (m_ClaimedBehaviorsFromCollision.Count == 0)
                return;

            for (int i = 0; i < m_ClaimedBehaviorsFromCollision.Count; i++)
            {
                if (m_ClaimedBehaviorsFromCollision[i] == this)
                    continue;
                m_ClaimedBehaviorsFromCollision[i].ProxyRelease();
            }
            m_ClaimedBehaviorsFromCollision.Clear();
        }

        void OnNetworkTick()
        {
            if (!isActiveAndEnabled)
                return;

            if (ShouldHoldOwnership)
            {
                ClaimOwnership();
                return;
            }

            if (!isOwnershipClaimedLocally)
                return;

            // Wait min 1 frame to release
            if (m_ReleaseTimer > (m_OwnershipReleaseDelay + m_NetworkDeltaTime))
            {
                // If claimed by proxy is reset, or we are not interacting with the object, release
                ReleaseOwnership();
                return;
            }

            m_ReleaseTimer += m_NetworkDeltaTime;
        }
    }
}
