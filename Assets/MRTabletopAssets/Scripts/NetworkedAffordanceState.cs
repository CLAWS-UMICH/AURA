using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;

#pragma warning disable CS0618 // Type or member is obsolete
namespace UnityLabs.SmartUX.Network
{
    [RequireComponent(typeof(XRInteractableAffordanceStateProvider))]

    [RequireComponent(typeof(NetworkObject))]
    public class NetworkedAffordanceState : NetworkBehaviour
    {
        [SerializeField]
        XRInteractableAffordanceStateProvider m_AffordanceHandler = null;

        readonly NetworkVariable<NetworkAffordanceMessage> m_ReplicatedNetworkState = new();

        struct NetworkAffordanceMessage : INetworkSerializable
        {
            byte m_AffordanceStateIndex;
            byte m_InteractionStrengthIncrement;

            public byte interactionStrengthIncrement
            {
                get => m_InteractionStrengthIncrement;
                set => m_InteractionStrengthIncrement = value;
            }

            public byte affordanceStateIndex
            {
                get => m_AffordanceStateIndex;
                set => m_AffordanceStateIndex = value;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref m_InteractionStrengthIncrement);
                serializer.SerializeValue(ref m_AffordanceStateIndex);
            }

            public bool Equals(NetworkAffordanceMessage message)
            {
                return interactionStrengthIncrement == message.interactionStrengthIncrement &&
                       affordanceStateIndex == message.affordanceStateIndex;
            }
        }

        void Initialize()
        {
            if (IsOwner)
            {
                m_ReplicatedNetworkState.SetDirty(true);
            }
            else
            {
                ApplyNetworkState(m_ReplicatedNetworkState.Value);
            }
        }

        void OnValidate()
        {
            if (m_AffordanceHandler == null)
                m_AffordanceHandler = GetComponent<XRInteractableAffordanceStateProvider>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_ReplicatedNetworkState.OnValueChanged += OnNetworkStateChanged;
            // TODO I don't like this always being subscribed to the tick because it doesn't necessarily need to
            // but subscribe/unscibe makes garbage, implement tick through bindingroup?
            NetworkManager.NetworkTickSystem.Tick += OnNetworkTick;
            Initialize();
        }

        public override void OnNetworkDespawn()
        {
            m_ReplicatedNetworkState.OnValueChanged -= OnNetworkStateChanged;

            if (NetworkManager != null && NetworkManager.NetworkTickSystem != null)
                NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
        }

        void OnNetworkTick()
        {
            if (!IsOwner || !enabled) return;

            AffordanceStateData currentState = m_AffordanceHandler.currentAffordanceStateData.Value;
            NetworkAffordanceMessage message = new()
            {
                interactionStrengthIncrement = currentState.stateTransitionIncrement,
                affordanceStateIndex = currentState.stateIndex
            };
            if (!m_ReplicatedNetworkState.Value.Equals(message))
            {
                CommitMessageServerRpc(message);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        void CommitMessageServerRpc(NetworkAffordanceMessage message)
        {
            m_ReplicatedNetworkState.Value = message;
        }

        void OnNetworkStateChanged(NetworkAffordanceMessage oldState, NetworkAffordanceMessage newState)
        {
            if (!IsOwner)
            {
                // Debug.Log($"{name} OnNetworkStateChanged");
                ApplyNetworkState(newState);
            }
        }

        void ApplyNetworkState(NetworkAffordanceMessage newState)
        {
            m_AffordanceHandler.UpdateAffordanceState(new AffordanceStateData(newState.affordanceStateIndex, newState.interactionStrengthIncrement));
        }
    }
}

#pragma warning restore CS0618 // Type or member is obsolete
