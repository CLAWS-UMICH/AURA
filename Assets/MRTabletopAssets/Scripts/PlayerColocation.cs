using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;

public class PlayerColocation : NetworkBehaviour
{
    public bool isShowingAvatar = true;

    [SerializeField]
    XRINetworkPlayer m_NetworkPlayer;

    [SerializeField]
    GameObject[] m_AvatarObjects;

    List<ulong> m_ColocatedPlayers = new List<ulong>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        m_ColocatedPlayers = new List<ulong>
        {
            NetworkManager.Singleton.LocalClientId
        };
    }

    public void SetAvatarActive(bool active)
    {
        isShowingAvatar = active;
        foreach (var avatar in m_AvatarObjects)
        {
            avatar.SetActive(active);
        }
    }

    public void SetColocationToPlayer(bool toggle)
    {
        if (toggle)
        {
            Debug.Log($"Player {NetworkManager.Singleton.LocalClientId} is requesting Colocation from Player {NetworkObject.OwnerClientId}");
            RequestColocationRpc(RpcTarget.Single(NetworkObject.OwnerClientId, RpcTargetUse.Temp));
        }
        else
        {
            Debug.Log($"Player {NetworkManager.Singleton.LocalClientId} is removing from colocation group");
            NativeArray<ulong> colocatedPlayers = new NativeArray<ulong>(m_ColocatedPlayers.ToArray(), Allocator.Temp);
            RemoveColocationRpc(NetworkManager.Singleton.LocalClientId, RpcTarget.Group(colocatedPlayers, RpcTargetUse.Temp));
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void RemoveColocationRpc(ulong playerID, RpcParams rpcParams = default)
    {
        if (playerID == NetworkManager.Singleton.LocalClientId)
        {
            foreach (var colocatedPlayerID in m_ColocatedPlayers)
            {
                if (colocatedPlayerID == NetworkManager.Singleton.LocalClientId)
                    continue;

                UpdatePlayerUIAndVisuals(colocatedPlayerID, false);
            }

            m_ColocatedPlayers.Clear();
            m_ColocatedPlayers.Add(NetworkManager.Singleton.LocalClientId);
        }

        Debug.Log($"RemoveColocationRpc from {rpcParams.Receive.SenderClientId}, to Remove Player {playerID}");
        if (m_ColocatedPlayers.Contains(playerID))
        {
            UpdatePlayerUIAndVisuals(playerID, false, m_ColocatedPlayers.ToArray());
            m_ColocatedPlayers.Remove(playerID);
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void RequestColocationRpc(RpcParams rpcParams = default)
    {
        Debug.Log($"RequestColocationRpc from {rpcParams.Receive.SenderClientId}");
        // m_ColocatedPlayers.Add(rpcParams.Receive.SenderClientId);
        List<ulong> newGroup = new List<ulong>(m_ColocatedPlayers)
        {
            rpcParams.Receive.SenderClientId
        };
        NativeArray<ulong> colocatedPlayersArray = new NativeArray<ulong>(newGroup.ToArray(), Allocator.Temp);

        // Rpc to all other players in the group
        AddPlayerToExistingGroupRpc(newGroup.ToArray(), RpcTarget.Group(colocatedPlayersArray, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void AddPlayerToExistingGroupRpc(ulong[] playerIDs, RpcParams rpcParams = default)
    {
        Debug.Log($"ConfirmColocationRequest from {rpcParams.Receive.SenderClientId}, to add to colocation group.");
        foreach (var ID in playerIDs)
        {
            if (!m_ColocatedPlayers.Contains(ID))
            {
                m_ColocatedPlayers.Add(ID);
            }
        }

        foreach (var ID in playerIDs)
        {
            UpdatePlayerUIAndVisuals(ID, true, playerIDs);
        }
    }

    void UpdatePlayerUIAndVisuals(ulong playerID, bool isColocated, ulong[] playerIDsInGroup = null)
    {
        if (XRINetworkGameManager.Instance.TryGetPlayerByID(playerID, out var networkPlayer))
        {
            Debug.Log($"Setting player {playerID} in group {isColocated}");
            PlayerColocation playerColocation = networkPlayer.GetComponent<PlayerColocation>();

            if (playerIDsInGroup != null)
                playerColocation.m_ColocatedPlayers = new List<ulong>(playerIDsInGroup);
        }
    }
}
