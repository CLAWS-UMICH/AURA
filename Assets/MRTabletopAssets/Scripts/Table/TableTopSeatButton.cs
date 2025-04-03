using UnityEngine;
using TMPro;
using UnityEngine.UI;
using XRMultiplayer;

public class TableTopSeatButton : MonoBehaviour
{
    public Color[] seatColors => m_SeatColors;
    [SerializeField]
    Color[] m_SeatColors;

    [SerializeField]
    Image[] m_SeatImages;

    [SerializeField]
    TMP_Text m_SeatNumberText;

    [SerializeField]
    TMP_Text m_SeatNameText;

    [SerializeField]
    TMP_Text m_PlayerInSeatText;

    [SerializeField]
    GameObject m_SeatUnoccupiedObject;

    [SerializeField]
    GameObject m_SeatOccupiedObject;

    [SerializeField]
    GameObject m_OwnedSetUI;

    [SerializeField]
    GameObject m_TakenSeatUI;

    [SerializeField]
    GameObject m_HoveredObject;

    [SerializeField]
    GameObject[] m_WorldSpaceSeatHoverObjects;

    [Header("Editor Variables")]
    [SerializeField]
    bool m_IsSpectator = false;

    [SerializeField, Range(0, 3)]
    int m_SeatID;

    [SerializeField]
    bool m_IsHovered = false;

    [SerializeField]
    bool m_IsOccupied = false;

    [SerializeField]
    bool m_IsLocalPlayer = false;

    [SerializeField]
    string m_AvailableSeatText = "<color=#7B7B7B><i>Available</i></color>";

    string m_PlayerNameInSeat = "Player Name";

    XRINetworkPlayer m_PlayerInSeat;
    [SerializeField] Button m_MuteButton;
    [SerializeField] Toggle m_HideAvatarToggle;
    [SerializeField] Image m_VoiceChatFillImage;
    [SerializeField] Image m_MicIcon;
    [SerializeField] Image m_SquelchedIcon;
    [SerializeField] Sprite m_MutedSprite;
    [SerializeField] Sprite m_UnmutedSprite;

    void OnValidate()
    {
        if (!m_IsSpectator)
        {
            m_SeatID = Mathf.Clamp(m_SeatID, 0, m_SeatColors.Length - 1);
            foreach (var icon in m_SeatImages)
                icon.color = m_SeatColors[m_SeatID];

            m_SeatNumberText.text = (m_SeatID + 1).ToString();
            m_SeatNameText.text = "Seat " + (m_SeatID + 1);
        }
        else
        {
            // Spectator is always true because we remove the player from the list if nt
            m_IsOccupied = true;
        }

        SetOccupied(m_IsOccupied);
    }

    void Update()
    {
        if (m_PlayerInSeat != null)
            m_VoiceChatFillImage.fillAmount = m_PlayerInSeat.playerVoiceAmp;
    }

    public void SetPlayerName(string name)
    {
        m_PlayerNameInSeat = name;
        m_PlayerInSeatText.text = m_IsOccupied ? (m_IsLocalPlayer ? "You" : m_PlayerNameInSeat) : m_AvailableSeatText;
    }

    public void SetLocalPlayer(bool local, bool updateOccupied = true)
    {
        m_IsLocalPlayer = local;
        if (updateOccupied)
            SetOccupied(m_IsOccupied);
    }

    public void AssignPlayerToSeat(XRINetworkPlayer player)
    {
        if (m_PlayerInSeat != null)
            RemovePlayerFromSeat();

        m_PlayerInSeat = player;

        SetPlayerName(m_PlayerInSeat.playerName);

        m_PlayerInSeat.onNameUpdated += SetPlayerName;

        m_PlayerInSeat.selfMuted.OnValueChanged += UpdateSelfMutedState;
        m_PlayerInSeat.squelched.Subscribe(UpdateSquelchedState);

        m_MuteButton.onClick.AddListener(SquelchPressed);
        m_SquelchedIcon.enabled = m_PlayerInSeat.squelched.Value;

        m_HideAvatarToggle.onValueChanged.AddListener(SetPlayerAvatarHidden);
        if (m_PlayerInSeat.TryGetComponent(out PlayerColocation playerColocation))
        {
            m_HideAvatarToggle.SetIsOnWithoutNotify(!playerColocation.isShowingAvatar);
        }

        if (m_PlayerInSeat.IsLocalPlayer)
            XRINetworkGameManager.LocalPlayerColor.Value = m_SeatColors[m_SeatID];

        SetLocalPlayer(m_PlayerInSeat.IsLocalPlayer, false);
        SetOccupied(true);
    }

    public void RemovePlayerFromSeat()
    {
        if (m_PlayerInSeat == null)
        {
            Debug.LogWarning("Trying to remove player from seat but no player is assigned to this seat.");
            return;
        }
        m_PlayerInSeat.onNameUpdated -= SetPlayerName;
        m_PlayerInSeat.selfMuted.OnValueChanged -= UpdateSelfMutedState;
        m_PlayerInSeat.squelched.Unsubscribe(UpdateSquelchedState);

        m_HideAvatarToggle.onValueChanged.RemoveListener(SetPlayerAvatarHidden);
        m_HideAvatarToggle.SetIsOnWithoutNotify(false);

        m_MuteButton.onClick.RemoveListener(SquelchPressed);

        m_VoiceChatFillImage.fillAmount = 0;
        m_SquelchedIcon.enabled = false;
        m_PlayerInSeat = null;
        SetLocalPlayer(false);
        SetOccupied(false);
    }

    void SetPlayerAvatarHidden(bool hidden)
    {
        if (m_PlayerInSeat != null && m_PlayerInSeat.TryGetComponent(out PlayerColocation playerColocation))
            playerColocation.SetAvatarActive(!hidden);
    }
    void SquelchPressed()
    {
        m_PlayerInSeat.ToggleSquelch();
    }

    public void UpdateSelfMutedState(bool old, bool current)
    {
        m_MicIcon.sprite = current ? m_MutedSprite : m_UnmutedSprite;
    }

    void UpdateSquelchedState(bool squelched)
    {
        m_SquelchedIcon.enabled = squelched;
    }

    public void SetOccupied(bool occupied)
    {
        m_IsOccupied = occupied;
        if (!m_IsSpectator)
            m_SeatImages[1].enabled = m_IsOccupied;
        m_PlayerInSeatText.text = m_IsOccupied ? (m_IsLocalPlayer ? "You" : m_PlayerNameInSeat) : m_AvailableSeatText;

        SetHover(m_IsHovered);
    }

    public void SetHover(bool hover)
    {
        m_IsHovered = hover;
        if (!m_IsHovered)
        {
            m_HoveredObject.SetActive(false);
            ShowWorldSpaceHover(false);

            if (!m_IsSpectator)
                m_PlayerInSeatText.gameObject.SetActive(true);
            return;
        }

        m_HoveredObject.SetActive(true);

        ShowWorldSpaceHover(!m_IsOccupied);

        if (!m_IsSpectator)
            m_PlayerInSeatText.gameObject.SetActive(false);

        if (m_IsOccupied)
        {
            m_SeatUnoccupiedObject.SetActive(false);
            m_SeatOccupiedObject.SetActive(true);
            m_OwnedSetUI.SetActive(m_IsLocalPlayer);
            m_TakenSeatUI.SetActive(!m_IsLocalPlayer);
        }
        else
        {
            m_SeatUnoccupiedObject.SetActive(true);
            m_SeatOccupiedObject.SetActive(false);
        }
    }

    void ShowWorldSpaceHover(bool show)
    {
        foreach (var obj in m_WorldSpaceSeatHoverObjects)
        {
            if (obj != null)
                obj.SetActive(show);
        }
    }
}
