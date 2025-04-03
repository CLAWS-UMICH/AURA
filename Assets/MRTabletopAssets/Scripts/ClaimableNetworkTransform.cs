using System;
using Unity.Netcode.Components;
using UnityEngine;
using XRMultiplayer;

namespace UnityLabs.SmartUX.Network
{
    /// <summary>
    /// Used for syncing a transform with client side changes. This includes host. Pure server as owner isn't supported by this. Please use NetworkTransform
    /// for transforms that'll always be owned by the server.
    /// </summary>
    [DisallowMultipleComponent]
    public class ClaimableNetworkTransform : CustomNetworkTransform
    {
        [Header("Optional Claimable Behaviour")]
        [Tooltip("Filling in this reference will alter the behaviour of the NetworkTransform. It will be fully enabled/disable based on whether it is claimed.")]
        [SerializeField]
        ClaimableNetworkBehaviour m_ClaimableNetworkBehaviour;

        void Start()
        {
            if (m_ClaimableNetworkBehaviour != null)
            {
                m_ClaimableNetworkBehaviour.claimedOwnership.OnValueChanged += OnClaimableValueChanged;
                enabled = false;
            }
        }

        void OnClaimableValueChanged(bool previousValue, bool newValue)
        {
            enabled = newValue;
            CanCommitToTransform = IsOwner;
            // If claiming to move, force reset and sync of state to its current local state
            ResetLocalAuthoritativeStateToCurrentTransform();
            ResetInterpolatedStateToCurrentAuthoritativeState();
            TryCommit(true);
        }

        public override void OnGainedOwnership()
        {
            if (m_ClaimableNetworkBehaviour != null)
                return;

            CanCommitToTransform = true;
            base.OnGainedOwnership();
        }

        public override void OnLostOwnership()
        {
            if (m_ClaimableNetworkBehaviour != null)
                return;

            CanCommitToTransform = false;
            base.OnLostOwnership();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (m_ClaimableNetworkBehaviour != null)
                return;

            CanCommitToTransform = IsOwner;
        }

        protected override void Update()
        {
            if (m_CachedNetworkManager != null && (m_CachedNetworkManager.IsConnectedClient || m_CachedNetworkManager.IsListening))
            {
                base.Update();
                if (CanCommitToTransform)
                {
                    TryCommitTransformToServer(transform, m_CachedNetworkManager.LocalTime.Time);
                }
            }
        }
    }
}
