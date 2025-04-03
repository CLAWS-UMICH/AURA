using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;

public class SeatMap : MonoBehaviour
{
    [SerializeField] Color[] m_SeatColors;
    [SerializeField] Image[] m_SeatImages;
    [SerializeField] Button[] m_SeatButtons;

    [SerializeField] float m_FilledSeatAlpha = 1.0f;

    [SerializeField] float m_EmptySeatAlpha = 0.15f;

    [SerializeField] NetworkTableTopManager m_TableTopManager;

    void Awake()
    {
        if (m_TableTopManager == null)
            m_TableTopManager = FindFirstObjectByType<NetworkTableTopManager>();
    }

    void Start()
    {
        if (XRINetworkGameManager.Connected.Value)
        {
            UpdateAllSeats();
            m_TableTopManager.networkedSeats.OnListChanged += OnOccupiedSeatsChanged;
        }
        XRINetworkGameManager.Connected.Subscribe(OnConnected);
    }

    void OnDestroy()
    {
        XRINetworkGameManager.Connected.Unsubscribe(OnConnected);
    }

    void OnConnected(bool connected)
    {
        if (connected)
        {
            UpdateAllSeats();
            m_TableTopManager.networkedSeats.OnListChanged += OnOccupiedSeatsChanged;
        }
        else
            m_TableTopManager.networkedSeats.OnListChanged -= OnOccupiedSeatsChanged;
    }

    private void OnOccupiedSeatsChanged(NetworkListEvent<NetworkedSeat> changeEvent)
    {
        UpdateAllSeats();
    }

    void UpdateAllSeats()
    {
        for (int i = 0; i < m_TableTopManager.networkedSeats.Count; i++)
        {
            m_SeatImages[i].color = GetColorForSeat(i, m_TableTopManager.networkedSeats[i].isOccupied);
            m_SeatButtons[i].interactable = !m_TableTopManager.networkedSeats[i].isOccupied;
        }
    }

    Color GetColorForSeat(int seatIndex, bool isOccupied)
    {
        return new Color(m_SeatColors[seatIndex].r, m_SeatColors[seatIndex].g, m_SeatColors[seatIndex].b, isOccupied ? m_FilledSeatAlpha : m_EmptySeatAlpha);
    }
}
