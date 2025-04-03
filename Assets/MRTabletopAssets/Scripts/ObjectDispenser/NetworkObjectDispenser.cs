using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.Events;
using XRMultiplayer;

namespace MRTTT
{
    /// <summary>
    /// Represents a networked object dispenser that can spawn and despawn interactable objects.
    /// </summary>
    public class NetworkObjectDispenser : NetworkBehaviour
    {
        const int k_NonToggleablePanelId = -1;

        /// <summary>
        /// The button used to clear the current interactables.
        /// </summary>
        [SerializeField] UnityEngine.UI.Button m_ClearButton;

        // [SerializeField] bool m_UseCapacity = false;
        /// <summary>
        /// The maximum capacity of the dispenser.
        /// </summary>
        [SerializeField] int m_Capacity;

        [SerializeField] float m_DistanceCheckTimeInterval = .5f;
        /// <summary>
        /// The text component displaying the current capacity.
        /// </summary>
        [SerializeField] TMP_Text m_CountText;

        // NetworkVariable<bool> m_IsActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        /// <summary>
        /// The network variable representing the current capacity.
        /// </summary>
        NetworkVariable<int> m_CurrentCapacityNetworked = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// The network variable representing the current panel ID.
        /// </summary>
        NetworkVariable<int> m_CurrentPanelIdNetworked = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        /// <summary>
        /// The list of currently active interactables.
        /// </summary>
        List<NetworkBaseInteractable> m_ActiveInteractables = new List<NetworkBaseInteractable>();

        // /// <summary>
        // /// This panel is persistent and does not switch on or off.
        // /// /// </summary>
        [SerializeField] DispenserPanel m_PersistentPanel;

        [SerializeField] Transform m_DefaultSpawnTransform;

        IEnumerator m_SpawnRoutine;
        IEnumerator m_DistanceCheckRoutine;

        ///<inheritdoc/>
        private void Start()
        {
            SetupPanels();
            m_CurrentCapacityNetworked.OnValueChanged += UpdateCapacity;
        }

        ///<inheritdoc/>
        public override void OnDestroy()
        {
            base.OnDestroy();
            m_CurrentCapacityNetworked.OnValueChanged -= UpdateCapacity;
        }

        void SetupPanels()
        {
            m_PersistentPanel.panelId = k_NonToggleablePanelId;
            for (int j = 0; j < m_PersistentPanel.spawnerSlots.Length; j++)
            {
                if (m_PersistentPanel.spawnerSlots[j] == null) continue;
                m_PersistentPanel.spawnerSlots[j].panelId = k_NonToggleablePanelId;
                m_PersistentPanel.spawnerSlots[j].slotId = j;
            }
        }

        IEnumerator ServerSpawnCooldownRoutine()
        {
            float deltaTime;
            while (IsServer)
            {
                deltaTime = Time.deltaTime;
                foreach (var slot in m_PersistentPanel.spawnerSlots)
                {
                    if (!slot.spawnTransform.gameObject.activeInHierarchy) continue;
                    if (slot.CanSpawn(deltaTime))
                    {
                        AddInteractableToDispenser(m_PersistentPanel, slot.slotId);
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }
        IEnumerator ServerDistanceCheckRoutine()
        {
            while (IsServer)
            {
                foreach (var slot in m_PersistentPanel.spawnerSlots)
                {
                    if (slot.CheckInteractablePosition())
                    {
                        m_ActiveInteractables.Add(slot.currentInteractable);
                        m_CurrentCapacityNetworked.Value = m_ActiveInteractables.Count;
                        slot.currentInteractable = null;
                    }
                }

                yield return new WaitForSeconds(m_DistanceCheckTimeInterval);
            }
        }

        ///<inheritdoc/>
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            m_ActiveInteractables.Clear();
            if (IsServer)
            {
                ServerSetSpawnRoutines();
                m_CurrentCapacityNetworked.Value = 0;
            }
            else if (m_CurrentPanelIdNetworked.Value != -1)
            {
                if (m_CountText != null)
                    m_CountText.text = $"Current Capacity: {m_CurrentCapacityNetworked.Value} / {m_Capacity}";
            }

            // if (m_IsActive.Value)
            //     Show();
            // else
            //     Hide();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            StopAllCoroutines();
        }

        /// <summary>
        /// Updates the capacity UI based on the current capacity value.
        /// </summary>
        /// <param name="old">The old capacity value.</param>
        /// <param name="current">The current capacity value.</param>
        void UpdateCapacity(int old, int current)
        {
            if (m_CountText != null)
            {
                m_CountText.text = $"Current Capacity: {m_CurrentCapacityNetworked.Value} / {m_Capacity}";
                m_CountText.color = m_CurrentCapacityNetworked.Value > m_Capacity ? Color.red : Color.white;
            }

            if (m_ClearButton != null)
                m_ClearButton.interactable = m_CurrentCapacityNetworked.Value > 0;
        }

        /// <summary>
        /// Clears the current interactables on the server.
        /// </summary>
        public void ClearCurrentInteractables()
        {
            if (IsServer)
            {
                for (int i = m_ActiveInteractables.Count - 1; i >= 0; i--)
                {
                    if (m_ActiveInteractables[i] != null & !m_ActiveInteractables[i].isInteracting)
                    {
                        m_ActiveInteractables[i].NetworkObject.Despawn();
                        m_ActiveInteractables.Remove(m_ActiveInteractables[i]);
                    }
                }
                m_CurrentCapacityNetworked.Value = m_ActiveInteractables.Count;
            }
        }

        [ContextMenu("Hide Dispenser")]
        public void Hide()
        {
            if (!Application.isPlaying)
                return;
            if (IsServer & !NetworkManager.ShutdownInProgress)
            {
                // m_IsActive.Value = false;
                foreach (var slot in m_PersistentPanel.spawnerSlots)
                {
                    if (slot.currentInteractable != null)
                    {
                        if (!slot.currentInteractable.isInteracting)
                        {
                            if (slot.currentInteractable.NetworkObject.IsSpawned)
                                slot.currentInteractable.NetworkObject.Despawn();
                            else
                                Destroy(slot.currentInteractable.gameObject);
                        }
                    }
                }

                StopCoroutine(m_DistanceCheckRoutine);
                StopCoroutine(m_SpawnRoutine);
            }

            foreach (var slot in m_PersistentPanel.spawnerSlots)
            {
                slot.gameObject.SetActive(false);
            }
        }

        [ContextMenu("Show Dispenser")]
        public void Show()
        {
            foreach (var slot in m_PersistentPanel.spawnerSlots)
            {
                slot.gameObject.SetActive(true);
            }

            if (IsServer)
            {
                // m_IsActive.Value = true;
                ServerSetSpawnRoutines();
            }
        }

        void ServerSetSpawnRoutines()
        {
            if (!IsServer)
                return;

            m_DistanceCheckRoutine = ServerDistanceCheckRoutine();
            m_SpawnRoutine = ServerSpawnCooldownRoutine();
            StartCoroutine(m_DistanceCheckRoutine);
            StartCoroutine(m_SpawnRoutine);
        }

        /// <summary>
        /// Picks up an object from a dispenser panel slot.
        /// </summary>
        /// <param name="panel">The dispenser panel.</param>
        /// <param name="slotId">The slot ID.</param>
        void PickupObject(UnityAction<bool> bindingAction, int panelId, int slotId)
        {
            var panel = GetPanelById(panelId);
            if (!IsServer) { Utils.Log("Trying to Spawn Object from Non-Server Client"); return; }
            NetworkBaseInteractable netInteractable = panel.spawnerSlots[slotId].currentInteractable;
            if (netInteractable == null) return;

            netInteractable.OnInteractingChanged.RemoveListener(bindingAction);

            if (netInteractable.TryGetComponent(out Rigidbody rb))
            {
                rb.constraints = RigidbodyConstraints.None;
            }
        }

        /// <summary>
        /// Adds an interactable to a dispenser panel slot on the server.
        /// </summary>
        /// <param name="panel">The dispenser panel.</param>
        /// <param name="slotId">The slot ID.</param>
        /// <param name="prefabId">The prefab ID, if default (-1) it will spawn based on the index of the prefab list.</param>
        void AddInteractableToDispenser(DispenserPanel panel, int slotId)
        {
            if (panel.spawnerSlots[slotId].currentInteractable != null)
            {
                Utils.Log($"Cannot Spawn. Interactable already exists in Panel {panel.panelId}, slot {panel.spawnerSlots[slotId].slotId}");
                return;
            }

            Transform spawnerTransform = panel.spawnerSlots[slotId].spawnTransform;
            panel.spawnerSlots[slotId].m_SpawnCooldownTimer = panel.spawnerSlots[slotId].spawnCooldown;
            NetworkBaseInteractable spawnedInteractable = panel.spawnerSlots[slotId].SpawnInteractablePrefab(spawnerTransform);

            panel.spawnerSlots[slotId].currentInteractable = spawnedInteractable;
            panel.spawnerSlots[slotId].currentInteractable.NetworkObject.Spawn();

            // Creates a UnityAction<bool> that calls PickupObject with the correct parameters
            void PickupBinding(bool arg0)
            {
                PickupObject(PickupBinding, panel.panelId, slotId);
            }

            spawnedInteractable.OnInteractingChanged.AddListener(PickupBinding);

            if (panel.spawnerSlots[slotId].freezeOnSpawn && spawnedInteractable.TryGetComponent(out Rigidbody rb))
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        DispenserPanel GetPanelById(int Id)
        {
            return m_PersistentPanel;
        }

        [ContextMenu("Spawn Random Object")]
        void SpawnRandomObject()
        {
            SpawnRandomRpc(m_DefaultSpawnTransform.position, m_DefaultSpawnTransform.rotation);
        }

        [Rpc(SendTo.Server)]
        public void SpawnRandomRpc(Vector3 spawnPosition, Quaternion spawnRotation)
        {
            var spawnObject = m_PersistentPanel.spawnerSlots[UnityEngine.Random.Range(0, m_PersistentPanel.spawnerSlots.Length)].spawnInteractablePrefab;
            if (UnityEngine.Random.value < .85f)
            {
                int randomSlot = UnityEngine.Random.Range(0, m_PersistentPanel.spawnerSlots.Length);
                spawnObject = m_PersistentPanel.spawnerSlots[randomSlot].spawnInteractablePrefab;
            }

            NetworkPhysicsInteractable spawnedObject = Instantiate(spawnObject.gameObject, spawnPosition, spawnRotation).GetComponent<NetworkPhysicsInteractable>();
            spawnedObject.spawnLocked = false;
            spawnedObject.NetworkObject.Spawn();

            m_ActiveInteractables.Add(spawnedObject.GetComponent<NetworkBaseInteractable>());
            m_CurrentCapacityNetworked.Value = m_ActiveInteractables.Count;
        }

        public void OnGameModeStart()
        {
        }

        public void OnGameModeEnd()
        {
        }

    }

    [Serializable]
    /// <summary>
    /// Represents a dispenser panel.
    /// </summary>
    public struct DispenserPanel
    {
        /// <summary>
        /// The type of physics used by this panel.
        /// </summary>
        public string panelName;

        /// <summary>
        /// The array of dispenser slots used by the object dispenser.
        /// </summary>
        [SerializeField] public InteractableSpawner[] spawnerSlots;

        [HideInInspector] public int panelId;
    }
}
