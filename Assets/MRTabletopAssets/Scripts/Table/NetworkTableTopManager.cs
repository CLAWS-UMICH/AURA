using System;
using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;

public class NetworkTableTopManager : NetworkBehaviour
{
    public NetworkList<NetworkedSeat> networkedSeats;

    [SerializeField]
    TableSeatSystem m_SeatSystem;

    [SerializeField]
    TableTop m_TableTop;

    [SerializeField]
    TableTopSeatButton[] m_SeatButtons;

    void Awake()
    {
        networkedSeats = new NetworkList<NetworkedSeat>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            networkedSeats.Clear();
            for (int i = 0; i < m_SeatButtons.Length; i++)
            {
                networkedSeats.Add(new NetworkedSeat { isOccupied = false, playerID = 0 });
            }
        }

        UpdateNetworkedSeatsVisuals();
        networkedSeats.OnListChanged += OnOccupiedSeatsChanged;
        RequestAnySeatFromHost();

        if (IsServer)
        {
            XRINetworkGameManager.Instance.playerStateChanged += OnPlayerStateChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        foreach (var seatButton in m_SeatButtons)
        {
            seatButton.RemovePlayerFromSeat();
        }
        networkedSeats.OnListChanged -= OnOccupiedSeatsChanged;
        XRINetworkGameManager.Instance.playerStateChanged -= OnPlayerStateChanged;
        m_SeatSystem.TeleportToSeat(0);
        TableTop.k_CurrentSeat = -2;
    }

    private void OnOccupiedSeatsChanged(NetworkListEvent<NetworkedSeat> changeEvent)
    {
        UpdateNetworkedSeatsVisuals();
    }

    void OnPlayerStateChanged(ulong playerID, bool connected)
    {
        if (!connected)
        {
            for (int i = 0; i < networkedSeats.Count; i++)
            {
                if (networkedSeats[i].playerID == playerID)
                {
                    ServerRemoveSeat(i);
                }
            }

            UpdateNetworkedSeatsVisuals();
        }
    }

    void UpdateNetworkedSeatsVisuals()
    {
        for (int i = 0; i < networkedSeats.Count; i++)
        {
            if (!networkedSeats[i].isOccupied)
            {
                m_SeatButtons[i].SetOccupied(false);
            }
            else
            {
                if (XRINetworkGameManager.Instance.TryGetPlayerByID(networkedSeats[i].playerID, out var player))
                {
                    m_SeatButtons[i].AssignPlayerToSeat(player);
                }
                else
                {
                    Debug.LogError($"Player with id {networkedSeats[i].playerID} not found");
                }
            }
        }
    }

    public void RequestAnySeatFromHost()
    {
        RequestSeatServerRpc(NetworkManager.Singleton.LocalClientId, TableTop.k_CurrentSeat);
    }

    public void RequestSeat(int newSeatChoice)
    {
        RequestSeatServerRpc(NetworkManager.Singleton.LocalClientId, TableTop.k_CurrentSeat, newSeatChoice);
    }

    [Rpc(SendTo.Server)]
    void RequestSeatServerRpc(ulong localPlayerID, int currentSeatID, int newSeatID = -2)
    {
        if (newSeatID <= -2)    // Request any available seat
            newSeatID = GetAnyAvailableSeats();

        if (!IsSeatOccupied(newSeatID))
            ServerAssignSeat(currentSeatID, newSeatID, localPlayerID);
        else
            Debug.Log("User tried to join an occupied seat");
    }

    int GetAnyAvailableSeats()
    {
        int availableSeat = -1;
        for (int i = 0; i < networkedSeats.Count; i++)
        {
            if (!networkedSeats[i].isOccupied)
            {
                availableSeat = i;
                return availableSeat;
            }
        }

        return availableSeat;
    }

    bool IsSeatOccupied(int seatID)
    {
        return seatID >= 0 && networkedSeats[seatID].isOccupied;
    }

    void ServerAssignSeat(int currentSeatID, int newSeatID, ulong localPlayerID)
    {
        if (currentSeatID >= 0)
        {
            ServerRemoveSeat(currentSeatID);
        }
        if (newSeatID >= 0)
        {
            networkedSeats[newSeatID] = new NetworkedSeat { isOccupied = true, playerID = localPlayerID };
        }

        UpdateNetworkedSeatsVisuals();

        AssignSeatRpc(newSeatID, localPlayerID);
    }

    void ServerRemoveSeat(int seatID)
    {
        networkedSeats[seatID] = new NetworkedSeat { isOccupied = false, playerID = 0 };
        UpdateNetworkedSeatsVisuals();
        RemovePlayerFromSeatRpc(seatID);
    }

    [Rpc(SendTo.Everyone)]
    void RemovePlayerFromSeatRpc(int seatID)
    {
        m_SeatButtons[seatID].RemovePlayerFromSeat();
    }

    [Rpc(SendTo.Everyone)]
    void AssignSeatRpc(int seatID, ulong playerID)
    {
        if (XRINetworkGameManager.Instance.TryGetPlayerByID(playerID, out var player))
        {
            m_SeatButtons[seatID].AssignPlayerToSeat(player);
            if (playerID == NetworkManager.Singleton.LocalClientId)
            {
                m_SeatSystem.TeleportToSeat(seatID);
            }
        }
        else
        {
            Debug.LogError($"Player with id {playerID} not found");
        }
    }

    public void TeleportToSpectatorSeat()
    {
        RequestSeat(-1);
    }
}

[Serializable]
public struct NetworkedSeat : INetworkSerializable, IEquatable<NetworkedSeat>
{
    public bool isOccupied;
    public ulong playerID;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref isOccupied);
        serializer.SerializeValue(ref playerID);
    }

    public readonly bool Equals(NetworkedSeat other)
    {
        return isOccupied == other.isOccupied && playerID == other.playerID;
    }
}
