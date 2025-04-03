using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityLabs.Slices.Transitions;
using UnityLabs.SmartUX.Interaction.Widgets;

namespace UnityLabs.Slices.Games.Chess
{
    /// <summary>
    /// Entry point and coordinator of all things related to Chessboard UI
    /// </summary>
    public class ChessBoardUI : MonoBehaviour
    {
        [Header("Logic")]
        [SerializeField]
        ChessBoard m_Board = null;

        [SerializeField]
        bool m_AutoStartAIGame = false;

        [Header("UI")]
        [SerializeField]
        ChessOptionsUIController m_ChessOptionsUIController_Black = null;

        [SerializeField]
        ChessOptionsUIController m_ChessOptionsUIController_White = null;

        [SerializeField]
        XRToggle m_ToggleReceiver = null;

        [SerializeField]
        GameObject m_TurnCompleteIndicator = null;

        [SerializeField]
        XRBaseInteractable m_PressClockReceiver = null;

        [SerializeField]
        XRBaseInteractable m_WConfirmReceiver = null;

        [SerializeField]
        XRBaseInteractable m_BConfirmReceiver = null;

        [SerializeField]
        XRBaseInteractable m_WResetReceiver = null;

        [SerializeField]
        XRBaseInteractable m_BResetReceiver = null;

        [SerializeField]
        XRBaseInteractable[] m_StartReceiver = null;

        [SerializeField]
        float[] m_StartYAngles = null;

        [SerializeField]
        TextMeshProUGUI m_WWinMessage = null;

        [SerializeField]
        TextMeshProUGUI m_BWinMessage = null;

        [Header("Transitions")]
        [SerializeField]
        TransitionStack m_TransitionStack = null;

        [SerializeField]
        ChessSetupTransition m_ChessSetupTransition = null;

        [SerializeField]
        ChessBoardTransition m_ChessBoardTransition = null;

        [SerializeField]
        ChessOptionsTransition m_ChessOptionsTransition = null;

        [SerializeField]
        ChessWinTransition m_ChessWinTransition = null;

        readonly BindingsGroup m_BindingGroup = new BindingsGroup();

        OptionState m_InitialOptionState;
        ChessColor m_LastUndoColor = ChessColor.None;

        void Start()
        {
            m_BResetReceiver.enabled = false;
            m_WResetReceiver.enabled = false;
            m_BResetReceiver.gameObject.SetActive(false);
            m_WResetReceiver.gameObject.SetActive(false);
            m_BResetReceiver.enabled = m_Board.optionState.pressConfirm;
            m_WResetReceiver.enabled = m_Board.optionState.pressConfirm;
            m_BConfirmReceiver.gameObject.SetActive(m_Board.optionState.pressConfirm);
            m_WConfirmReceiver.gameObject.SetActive(m_Board.optionState.pressConfirm);
            UndoAvailable(ChessColor.None);
        }

        void OnEnable()
        {
            if (m_ChessOptionsUIController_Black == null || m_ChessOptionsUIController_White == null)
            {
                enabled = false;
                Debug.LogError("Missing chess UI." + this);
                return;
            }

            m_BindingGroup.AddBinding(m_Board.showingOptions.Subscribe(SetIsShowingOptions));
            m_BindingGroup.AddBinding(m_Board.GameMode.Subscribe(OnGameStartedChanged));
            m_BindingGroup.AddBinding(m_Board.gameConnected.Subscribe(OnGameConnected));

            if (m_ToggleReceiver != null)
                m_BindingGroup.AddBinding(m_ToggleReceiver.onToggleSelectChanged.Subscribe(OptionsToggleClicked));

            for (int i = 0; i < m_StartReceiver.Length; ++i)
            {
                float yAngle = m_StartYAngles[i];
                m_StartReceiver[i].selectEntered.AddListener(args => StartGame(yAngle));
            }
            m_WResetReceiver.selectEntered.AddListener(args => ResetGame(true));
            m_BResetReceiver.selectEntered.AddListener(args => ResetGame(true));

            m_WConfirmReceiver.selectEntered.AddListener(args => m_Board.ConfirmMove(true));
            m_BConfirmReceiver.selectEntered.AddListener(args => m_Board.ConfirmMove(true));
            m_PressClockReceiver.selectEntered.AddListener(args => m_Board.ConfirmMove(true));

            m_Board.onPlayerLost += ShowWin;
            m_Board.onUndoAvailable += UndoAvailable;
        }

        void OnDisable()
        {
            m_BindingGroup.Clear();

            for (int i = 0; i < m_StartReceiver.Length; ++i)
            {
                m_StartReceiver[i].selectEntered.RemoveAllListeners();
            }
            m_WResetReceiver.selectEntered.RemoveAllListeners();
            m_BResetReceiver.selectEntered.RemoveAllListeners();
            m_WConfirmReceiver.selectEntered.RemoveAllListeners();
            m_BConfirmReceiver.selectEntered.RemoveAllListeners();
            m_PressClockReceiver.selectEntered.RemoveAllListeners();

            m_Board.onPlayerLost -= ShowWin;
            m_Board.onUndoAvailable -= UndoAvailable;
        }

        public void SetStartButtonsActive(bool state)
        {
            for (int i = 0; i < m_StartReceiver.Length; ++i)
            {
                m_StartReceiver[i].enabled = state;
            }
        }

        void OnGameStartedChanged(ChessGameMode gameMode)
        {
            if (gameMode != ChessGameMode.NotStarted)
            {
                UndoAvailable(m_Board.shouldShowUndo ? m_Board.currentTurn : ChessColor.None);

                // Pushing multiple transitions is sort of a crude way to ensure the stack ends up correctly on player join midgame.
                // Doesn't cause issue because it prevents duplicates, but should be a more elegant way I think to deal with this.
                m_TransitionStack.PushTransition(m_ChessSetupTransition);
                m_TransitionStack.PushTransition(m_ChessBoardTransition);

                // show options in case it was hidden from resign or win reset.
                if (m_ToggleReceiver != null)
                    m_ToggleReceiver.enabled = true;

                m_WWinMessage.text = String.Empty;
                m_BWinMessage.text = String.Empty;
            }
            else
            {
                UndoAvailable(ChessColor.None);

                m_TransitionStack.PopTransition(m_ChessBoardTransition);
            }
        }

        void OnGameConnected(bool isGameConnected)
        {
            if (isGameConnected)
            {
                m_TransitionStack.PushTransition(m_ChessSetupTransition);
            }
            else
            {
                // todo popping immediately for now until transitions in and out chess are dealt with
                m_TransitionStack.PopTransitionImmediate(m_ChessSetupTransition);
            }

            if (m_AutoStartAIGame)
            {
                m_Board.ClaimBoardStateOwnership();
                m_Board.StartGame(ChessGameMode.AIvsAI, true);
            }
        }

        void SetIsShowingOptions(bool isShowingOptions)
        {
            if (isShowingOptions)
            {
                UndoAvailable(ChessColor.None);

                // Pushing multiple transitions is sort of a crude way to ensure the stack ends up correctly on player join midgame.
                // Doesn't cause issue because it prevents duplicates, but should be a more elegant way I think to deal with this.
                m_TransitionStack.PushTransition(m_ChessSetupTransition);
                m_TransitionStack.PushTransition(m_ChessBoardTransition);
                m_TransitionStack.PushTransition(m_ChessOptionsTransition);

                // Update after one frame delay so that on first open everything can run through Awake/Start
                StartCoroutine(UpdateUIWithSettings());
            }
            else
            {
                UndoAvailable(m_Board.shouldShowUndo ? m_LastUndoColor : ChessColor.None);

                m_TransitionStack.PopTransition(m_ChessOptionsTransition);
            }

            // Must update toggle state in case this is being synced over network. Toggle only claims ownership
            // when user actually touches it and fires SelectChanged, so this can be set here without firing network events.
            if (m_ToggleReceiver != null)
                m_ToggleReceiver.isToggled.Value = isShowingOptions;
        }

        void UndoAvailable(ChessColor color)
        {
            // m_LastColor is really to hide undo and then be able to bring it back to same state after
            // unhiding (hiding/showing options) so only update if its an actual color.
            if (color != ChessColor.None) m_LastUndoColor = color;

            m_WConfirmReceiver.enabled = color == ChessColor.White && m_Board.optionState.pressConfirm;
            m_BConfirmReceiver.enabled = color == ChessColor.Black && m_Board.optionState.pressConfirm;
            if (m_PressClockReceiver != null)
                m_PressClockReceiver.enabled = color != ChessColor.None;

            m_TurnCompleteIndicator.SetActive(m_PressClockReceiver.enabled);
        }

        public void Resign()
        {
            m_Board.ClaimBoardStateOwnership();
            if (m_ToggleReceiver != null)
                m_ToggleReceiver.enabled = false;
            m_Board.SetShowingOptions(false);
            m_Board.ResetGame();
        }

        /// <summary>
        /// Called by restart buttons on win scene.
        /// </summary>
        void ResetGame(bool selected)
        {
            if (selected)
            {
                m_Board.ClaimBoardStateOwnership();
                if (m_ToggleReceiver != null)
                    m_ToggleReceiver.enabled = false;
                m_Board.ResetGame();
            }
        }

        void StartGame(float rotateYAngle)
        {
            m_Board.ClaimBoardStateOwnership();
            m_Board.boardRotation.Value = rotateYAngle;

            if (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClients.Count == 1)
            {

                Debug.Log("Starting game with one player. Starting Human vs AI.");
                m_Board.StartGame(ChessGameMode.HumanVsAI, true);
            }
            else
            {
                Debug.Log("Starting game with two players. Starting Human vs AI.");
                m_Board.StartGame(ChessGameMode.HumanVsHuman, true);
            }
        }

        /// <summary>
        /// Used only for testing.
        /// </summary>
        [ContextMenu("ShowWin")]
        void ShowWin()
        {
            UndoAvailable(ChessColor.None);
            ShowWin(ChessColor.Black, "Test Win!");
        }

        /// <summary>
        /// Called from toggle OnSelectxit UnityEvent. The toggle calls ClaimCompleteOwnership from OnSelectEnter.
        /// </summary>
        void OptionsToggleClicked(bool selected)
        {
            if (!selected)
            {
                m_Board.ClaimBoardStateOwnership();
                if (m_ToggleReceiver != null)
                {
                    if (!m_ToggleReceiver.isToggled.Value) ApplySettings();
                    m_Board.SetShowingOptions(m_ToggleReceiver.isToggled.Value);
                }
            }
        }

        void ShowWin(ChessColor losingSide, string message)
        {
            // Pushing multiple transitions is sort of a crude way to ensure the stack ends up correctly on player join midgame.
            // Doesn't cause issue because it prevents duplicates, but should be a more elegant way I think to deal with this.
            m_TransitionStack.PushTransition(m_ChessSetupTransition);
            m_TransitionStack.PushTransition(m_ChessBoardTransition);
            m_TransitionStack.PushTransition(m_ChessWinTransition);

            m_WWinMessage.text = message;
            m_BWinMessage.text = message;
        }

        bool m_IsCachingSettings = false;

        IEnumerator UpdateUIWithSettings()
        {
            yield return null;
            yield return null;
            m_InitialOptionState = m_Board.optionState;
            m_ChessOptionsUIController_Black.optionsState = m_InitialOptionState;
            m_ChessOptionsUIController_White.optionsState = m_InitialOptionState;
            m_IsCachingSettings = true;
        }

        void ApplySettings()
        {
            if (!m_IsCachingSettings)
                return;

            m_IsCachingSettings = false;
            var blackOptionsState = m_ChessOptionsUIController_Black.optionsState;
            var whiteOptionsState = m_ChessOptionsUIController_White.optionsState;

            if (!Equals(blackOptionsState, m_InitialOptionState))
            {
                ApplyModifiedOptionsState(blackOptionsState);
            }
            else if (!Equals(whiteOptionsState, m_InitialOptionState))
            {
                ApplyModifiedOptionsState(whiteOptionsState);
            }
        }

        void ApplyModifiedOptionsState(OptionState newOptionsState)
        {
            m_BConfirmReceiver.gameObject.SetActive(newOptionsState.pressConfirm);
            m_WConfirmReceiver.gameObject.SetActive(newOptionsState.pressConfirm);

            m_Board.UpdateOptionsState(newOptionsState);
        }
    }
}
