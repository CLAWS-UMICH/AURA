using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;
using TMPro;
using System;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace XRMultiplayer
{
    [DefaultExecutionOrder(100)]
    public class PlayerOptions : MonoBehaviour
    {
        [SerializeField]
        XRInputButtonReader m_MenuButtonInput = new XRInputButtonReader("Menu Button");
        public XRInputButtonReader menuInput
        {
            get => m_MenuButtonInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_MenuButtonInput, value, this);
        }
        [SerializeField] AudioMixer m_Mixer;
        [SerializeField] int m_DefaultPanel = 1;
        [SerializeField] UnityEvent<bool> m_OnMenuToggle;

        [Header("Panels")]
        [SerializeField] GameObject m_MainPanelRoot;
        [SerializeField] GameObject m_HostRoomPanel;
        [SerializeField] GameObject m_ClientRoomPanel;
        [SerializeField] GameObject m_LeaveTableButtonObject;
        [SerializeField] GameObject[] m_OfflineWarningPanels;
        [SerializeField] GameObject[] m_OnlinePanels;
        [SerializeField] GameObject[] m_Panels;
        [SerializeField] Toggle[] m_PanelToggles;
        [SerializeField] GameObject m_AppearanceConfirmButton;
        [SerializeField] GameObject m_LeaveRoomPanel;
        [SerializeField] GameObject m_QuitGamePanel;

        [Header("Text Components")]
        [SerializeField] TMP_Text m_SnapTurnText;
        [SerializeField] TMP_Text[] m_RoomCodeTexts;
        [SerializeField] TMP_Text m_TimeText;
        [SerializeField] TMP_Text[] m_RoomNameText;
        [SerializeField] TMP_InputField m_RoomNameInputField;

        [Header("Voice Chat")]
        [SerializeField] Button m_MicPermsButton;
        [SerializeField] Slider m_InputVolumeSlider;
        [SerializeField] Slider m_OutputVolumeSlider;
        [SerializeField] Image m_LocalPlayerAudioVolume;
        [SerializeField] Image m_MutedIcon;
        [SerializeField] Image m_MicOnIcon;
        [SerializeField] TMP_Text m_VoiceChatStatus;

        [Header("Player Options")]
        [SerializeField] Vector2 m_MinMaxMoveSpeed = new Vector2(2.0f, 10.0f);
        [SerializeField] Vector2 m_MinMaxTurnAmount = new Vector2(15.0f, 180.0f);
        [SerializeField] float m_SnapTurnUpdateAmount = 15.0f;

        int m_CurrentPanel = 0;
        VoiceChatManager m_VoiceChatManager;
        DynamicMoveProvider m_MoveProvider;
        SnapTurnProvider m_TurnProvider;
        UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort.TunnelingVignetteController m_TunnelingVignetteController;

        PermissionCallbacks permCallbacks;

        private void Awake()
        {
            m_VoiceChatManager = FindAnyObjectByType<VoiceChatManager>();
            m_MoveProvider = FindAnyObjectByType<DynamicMoveProvider>();
            m_TurnProvider = FindAnyObjectByType<SnapTurnProvider>();
            m_TunnelingVignetteController = FindAnyObjectByType<UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort.TunnelingVignetteController>();

            XRINetworkGameManager.Connected.Subscribe(ConnectOnline);
            XRINetworkGameManager.ConnectedRoomName.Subscribe(UpdateRoomName);

            m_VoiceChatManager.selfMuted.Subscribe(MutedChanged);
            m_VoiceChatManager.connectionStatus.Subscribe(UpdateVoiceChatStatus);
            m_InputVolumeSlider.onValueChanged.AddListener(SetInputVolume);
            m_OutputVolumeSlider.onValueChanged.AddListener(SetOutputVolume);

            // ConnectOnline(false);
            HideCurrentUI();

            permCallbacks = new PermissionCallbacks();
            permCallbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
            permCallbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
        }

        internal void PermissionCallbacks_PermissionGranted(string permissionName)
        {
            Utils.Log($"{permissionName} PermissionCallbacks_PermissionGranted");
            m_MicPermsButton.gameObject.SetActive(false);
        }

        internal void PermissionCallbacks_PermissionDenied(string permissionName)
        {
            Utils.Log($"{permissionName} PermissionCallbacks_PermissionDenied");
        }

        void OnEnable()
        {
            // Enable and disable directly serialized actions with this behavior's enabled lifecycle.
            m_MenuButtonInput.EnableDirectActionIfModeUsed();

            TogglePanel(XRINetworkGameManager.Connected.Value ? 0 : m_DefaultPanel);

            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                m_MicPermsButton.gameObject.SetActive(true);
            }
            else
            {
                m_MicPermsButton.gameObject.SetActive(false);
            }
        }

        void OnDisable()
        {
            m_MenuButtonInput.DisableDirectActionIfModeUsed();
        }

        private void OnDestroy()
        {
            XRINetworkGameManager.Connected.Unsubscribe(ConnectOnline);
            XRINetworkGameManager.ConnectedRoomName.Unsubscribe(UpdateRoomName);
            m_VoiceChatManager.selfMuted.Unsubscribe(MutedChanged);

            m_VoiceChatManager.connectionStatus.Unsubscribe(UpdateVoiceChatStatus);
            m_InputVolumeSlider.onValueChanged.RemoveListener(SetInputVolume);
            m_OutputVolumeSlider.onValueChanged.RemoveListener(SetOutputVolume);
        }

        private void Update()
        {
            m_TimeText.text = $"{DateTime.Now:h:mm}<size=4><voffset=1em>{DateTime.Now:tt}</size></voffset>";

            if (m_MenuButtonInput != null)
            {
                if (m_MenuButtonInput.ReadWasPerformedThisFrame())
                {
                    ToggleMenu();
                }
            }
            if (XRINetworkGameManager.Connected.Value)
            {
                m_LocalPlayerAudioVolume.fillAmount = XRINetworkPlayer.LocalPlayer.playerVoiceAmp;
            }
            else
            {
                m_LocalPlayerAudioVolume.fillAmount = OfflinePlayerAvatar.voiceAmp.Value;
            }
        }

        void HideCurrentUI()
        {
            foreach (var go in m_OfflineWarningPanels)
            {
                go.SetActive(false);
            }

            foreach (var go in m_OnlinePanels)
            {
                go.SetActive(false);
            }

            // m_AppearanceConfirmButton.SetActive(false);
        }

        void ConnectOnline(bool connected)
        {
            foreach (var go in m_OfflineWarningPanels)
            {
                go.SetActive(!connected);
            }

            foreach (var go in m_OnlinePanels)
            {
                go.SetActive(connected);
            }

            m_AppearanceConfirmButton.SetActive(!connected);

            if (connected)
            {
                m_HostRoomPanel.SetActive(NetworkManager.Singleton.IsServer);
                m_ClientRoomPanel.SetActive(!NetworkManager.Singleton.IsServer);
                UpdateRoomName(XRINetworkGameManager.ConnectedRoomName.Value);
                m_MutedIcon.enabled = false;
                m_MicOnIcon.enabled = true;
                m_LocalPlayerAudioVolume.enabled = true;
                TogglePanel(0);
            }
            else
            {
                if (m_CurrentPanel == 0)
                {
                    TogglePanel(m_DefaultPanel);
                }
            }
        }

        public void HideAllPanels()
        {
            for (int i = 0; i < m_Panels.Length; i++)
            {
                m_Panels[i].SetActive(false);
            }
        }

        public void TogglePanel(int panelID)
        {
            m_CurrentPanel = panelID;
            for (int i = 0; i < m_Panels.Length; i++)
            {
                m_PanelToggles[i].SetIsOnWithoutNotify(panelID == i);
                m_Panels[i].SetActive(i == panelID);
            }

            if (panelID == 0)
                m_LeaveTableButtonObject.SetActive(true);
        }

        /// <summary>
        /// Toggles the menu on or off.
        /// </summary>
        /// <param name="overrideToggle"></param>
        /// <param name="overrideValue"></param>
        public void ToggleMenu(bool overrideToggle = false, bool overrideValue = false)
        {
            if (overrideToggle)
            {
                m_MainPanelRoot.SetActive(overrideValue);
            }
            else
            {
                ToggleMenu();
            }
            TogglePanel(XRINetworkGameManager.Connected.Value ? 0 : m_DefaultPanel);
        }

        public void ToggleMenu()
        {
            m_MainPanelRoot.SetActive(!m_MainPanelRoot.activeSelf);
            m_OnMenuToggle.Invoke(m_MainPanelRoot.activeSelf);
        }

        public void SetMenuActive(bool active)
        {
            bool wasActive = m_MainPanelRoot.activeSelf;
            m_MainPanelRoot.SetActive(active);

            if (wasActive != active)
            {
                m_OnMenuToggle.Invoke(active);
            }
        }

        public void ShowQuitGameConfirmationPanel()
        {
            HideAllPanels();
            m_QuitGamePanel.SetActive(true);
        }

        public void LeaveTableConfirmationPanel()
        {
            HideAllPanels();
            m_LeaveRoomPanel.SetActive(true);
        }

        public void LogOut()
        {
            XRINetworkGameManager.Instance.Disconnect();
            TogglePanel(m_DefaultPanel);
        }

        public void QuickJoin()
        {
            XRINetworkGameManager.Instance.QuickJoinLobby();
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        void UpdateVoiceChatStatus(string statusMessage)
        {
            m_VoiceChatStatus.text = $"<b>Voice Chat:</b> {statusMessage}";
        }
        public void SetVolumeLevel(float sliderValue)
        {
            m_Mixer.SetFloat("MainVolume", Mathf.Log10(sliderValue) * 20);
        }
        public void SetInputVolume(float volume)
        {
            float perc = Mathf.Lerp(-10, 10, volume);
            m_VoiceChatManager.SetInputVolume(perc);
        }

        public void SetOutputVolume(float volume)
        {
            float perc = Mathf.Lerp(-10, 10, volume);
            m_VoiceChatManager.SetOutputVolume(perc);
        }

        public void ToggleMute()
        {
            m_VoiceChatManager.ToggleSelfMute();
        }

        void MutedChanged(bool muted)
        {
            m_MutedIcon.enabled = muted;
            m_MicOnIcon.enabled = !muted;
            m_LocalPlayerAudioVolume.enabled = !muted;
            PlayerHudNotification.Instance.ShowText($"<b>Microphone: {(muted ? "OFF" : "ON")}</b>");
        }

        // Room Options
        public void UpdateRoomPrivacy(bool toggle)
        {
            XRINetworkGameManager.Instance.lobbyManager.UpdateRoomPrivacy(toggle);
        }

        public void SubmitNewRoomName(string text)
        {
            XRINetworkGameManager.Instance.lobbyManager.UpdateLobbyName(text);
        }

        void UpdateRoomName(string newValue)
        {
            foreach (var text in m_RoomCodeTexts)
            {
                text.text = $"Table Code: {XRINetworkGameManager.ConnectedRoomCode}";
            }
            foreach (var t in m_RoomNameText)
            {
                t.text = XRINetworkGameManager.ConnectedRoomName.Value;
            }
            m_RoomNameInputField.text = XRINetworkGameManager.ConnectedRoomName.Value;
        }

        // Player Options
        public void SetHandOrientation(bool toggle)
        {
            if (toggle)
            {
                m_MoveProvider.leftHandMovementDirection = DynamicMoveProvider.MovementDirection.HandRelative;
            }
        }
        public void SetHeadOrientation(bool toggle)
        {
            if (toggle)
            {
                m_MoveProvider.leftHandMovementDirection = DynamicMoveProvider.MovementDirection.HeadRelative;
            }
        }
        public void SetMoveSpeed(float speedPercent)
        {
            m_MoveProvider.moveSpeed = Mathf.Lerp(m_MinMaxMoveSpeed.x, m_MinMaxMoveSpeed.y, speedPercent);
        }

        public void UpdateSnapTurn(int dir)
        {
            float newTurnAmount = Mathf.Clamp(m_TurnProvider.turnAmount + (m_SnapTurnUpdateAmount * dir), m_MinMaxTurnAmount.x, m_MinMaxTurnAmount.y);
            m_TurnProvider.turnAmount = newTurnAmount;
            m_SnapTurnText.text = $"{newTurnAmount}°";
        }

        public void ToggleTunnelingVignette(bool toggle)
        {
            m_TunnelingVignetteController.gameObject.SetActive(toggle);
        }

        public void ToggleFlight(bool toggle)
        {
            m_MoveProvider.useGravity = !toggle;
            m_MoveProvider.enableFly = toggle;
        }

        public void ConfirmAppearance()
        {
            if (!XRINetworkGameManager.Connected.Value)
            {
                TogglePanel(2);
            }
        }
    }
}
