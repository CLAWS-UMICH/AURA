using System.Collections.Generic;
using Chess;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.XR.CoreUtils.Bindings;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Content.Utils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;
using UnityLabs.SmartUX.Interaction;
using UnityLabs.SmartUX.Network;

namespace UnityLabs.Slices.Games.Chess
{
    public class ChessPiece : MonoBehaviour
    {
        [SerializeField]
        ChessBoard m_ChessBoard = null;

        [SerializeField]
        GameObject m_RenderRoot;

        [SerializeField]
        ChessPieceType m_ChessPieceType;

        [SerializeField]
        string m_InitialTileCode = string.Empty;

        [SerializeField]
        ChessColor m_ChessColor;

        [SerializeField]
        Transform m_VisualRoot = null;

        [SerializeField]
        float m_VerticalOverlapOffset = 0.112f;

        [Header("Interaction components")]
        [SerializeField]
        XRBaseInteractable m_InteractionReceiver = null;

        [SerializeField]
        ClaimableNetworkBehaviour m_ClaimableNetworkBehaviour = null;

        [Header("New Affordance System")]
        [SerializeField]
#pragma warning disable CS0618 // Type or member is obsolete

        BaseAffordanceStateProvider m_AffordanceStateProvider = null;


        [SerializeField]
        Vector3AffordanceReceiver m_ScaleAffordanceReceiver = null;

        [Header("Events")]
        [SerializeField]
        UnityEvent m_OnMoveConfirmed = new UnityEvent();

        [SerializeField]
        UnityEvent m_OnMoveFailed = new UnityEvent();

        public string initialTileCode => m_InitialTileCode;

        /// <summary>
        /// Current square of piece dynamically calculated by checking distance to nearest GameObject.
        /// </summary>
        public ChessSquare squareFromPosition => parentChessBoard.GetSquareFromPos(transform.localPosition);

        /// <summary>
        /// Current square set by sync to backend;
        /// </summary>
        public ChessSquare currentSquare
        {
            get => m_CurrentSquare;
            set
            {
                m_CurrentSquare = value;

                if (m_AffordanceStateProvider.currentAffordanceStateData.Value.stateIndex != AffordanceStateShortcuts.selected)
                {
                    SetSquareAppearance();
                }
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete

        public ChessSquare startSquare { get; set; }

        public Move pendingMove { get; set; }

        public ChessPiece currentOverlap { get; set; }


        public ChessColor color => m_ChessColor;

        public ChessPieceType PieceType => m_ChessPieceType;

        public bool isInteractable => parentChessBoard.GameMode.Value != ChessGameMode.NotStarted &&
                                      parentChessBoard.playerLost == ChessColor.None &&
                                      !parentChessBoard.showingOptions.Value &&
                                      parentChessBoard.currentTurn == color &&
                                      !parentChessBoard.PlayerIsAI(color) &&
                                      currentSquare.IsValid;

        public Vector3 localPositionTarget { get => m_LocalPositionTarget; set => m_LocalPositionTarget = value; }

        ChessBoard parentChessBoard => m_ChessBoard;

#pragma warning disable CS0618 // Type or member is obsolete
        /// <summary>
        /// Tweens the RendererRoot in local space. Setting target to Vector.zero will center
        /// the visual representation of the piece on the square it is placed.
        /// </summary>
        Vector3TweenableVariable m_VisualOffsetAttribute = new Vector3TweenableVariable();

        Vector3TweenableVariable m_ScaleAttribute = new Vector3TweenableVariable();

        readonly BindingsGroup m_AwakeBindingGroup = new BindingsGroup();
        readonly BindingsGroup m_OnEnableBindingGroup = new BindingsGroup();
        readonly HashSet<ChessSquare> m_HighlightedSquaresList = new HashSet<ChessSquare>();
        Vector3 m_LocalPositionTarget = Vector3.zero;
        bool m_IsOverlapped = false;
        ChessSquare m_CurrentSquare = ChessSquare.Zero;

        void Awake()
        {
            m_InteractionReceiver.selectEntered.AddListener(OnSelectEntered);
            m_InteractionReceiver.selectExited.AddListener(OnSelectExited);

            m_AwakeBindingGroup.AddBinding(parentChessBoard.GameMode.SubscribeAndUpdate(isStarted =>
            {
                m_VisualOffsetAttribute.Value = Vector3.zero;
            }));

            m_AwakeBindingGroup.AddBinding(m_AffordanceStateProvider.currentAffordanceStateData.Subscribe(newState =>
            {
                switch (newState.stateIndex)
                {
                    case AffordanceStateShortcuts.selected:
                        SetHighlighting(true);
                        parentChessBoard.SetPieceSelected(this, currentSquare.IsValid);
                        break;
                    case AffordanceStateShortcuts.hovered:
                        SetHighlighting(true);
                        parentChessBoard.SetPieceSelected(this, false);
                        break;
                    case AffordanceStateShortcuts.idle:
                    case AffordanceStateShortcuts.disabled:
                        SetHighlighting(false);
                        parentChessBoard.SetPieceSelected(this, false);
                        break;
                }

                parentChessBoard.UpdateHighlightedPieces();
            }));

            m_VisualOffsetAttribute.Initialize(Vector3.zero);
            m_VisualOffsetAttribute.Subscribe(newPos => m_VisualRoot.transform.localPosition = newPos);
            m_ScaleAttribute.Initialize(transform.localScale);
            m_ScaleAttribute.Subscribe(newScale => m_VisualRoot.transform.localScale = newScale);

            SetPieceVisibility(false);

            m_ClaimableNetworkBehaviour.onNetworkSpawn += OnNetworkSpawn;
        }
#pragma warning restore CS0618 // Type or member is obsolete

        void OnDestroy()
        {
            m_InteractionReceiver.selectEntered.RemoveListener(OnSelectEntered);
            m_InteractionReceiver.selectExited.RemoveListener(OnSelectExited);
            m_AwakeBindingGroup.Clear();
        }

        void OnEnable()
        {
            // m_OnEnableBindingGroup.AddBinding(ThreadedUpdateQueue.SubscribeToUpdateThread(ThreadedUpdate));
        }

        void OnDisable()
        {
            m_OnEnableBindingGroup.Clear();
        }

        void OnNetworkSpawn(ClaimableNetworkBehaviour behaviour)
        {
            SetSquareAppearance();
        }

        void OnSelectEntered(SelectEnterEventArgs args)
        {
            OnSelectChanged(true);
        }

        void OnSelectExited(SelectExitEventArgs args)
        {
            OnSelectChanged(false);
        }

        public void ResetVisualOffset()
        {
            m_VisualOffsetAttribute.Initialize(Vector3.zero);
        }

        void SetHighlighting(bool isHighlighting)
        {
            if (!isHighlighting)
            {
                foreach (var targetSquare in m_HighlightedSquaresList)
                {
                    parentChessBoard.SetPieceLocationHighlighted(targetSquare, false);
                }

                m_HighlightedSquaresList.Clear();
            }
            else
            {
                if (m_HighlightedSquaresList.Add(currentSquare)) parentChessBoard.SetPieceLocationHighlighted(currentSquare, true);
                if (m_HighlightedSquaresList.Add(startSquare)) parentChessBoard.SetPieceLocationHighlighted(startSquare, true);
            }

            parentChessBoard.UpdateHighlightedPieces();
        }

        void OnSelectChanged(bool isSelecting)
        {
            if (isSelecting)
            {
                // UndoPendingMove will clear start square, but if we are
                // reselecting piece that had a startsquare already, we want to keep it.
                var tempSquare = startSquare;
                m_ChessBoard.UndoPendingMove();
                startSquare = tempSquare;

                if (pendingMove.IsInvalid)
                {
                    startSquare = currentSquare;
                }
            }
            else
            {
                pendingMove = m_ChessBoard.TryMovement(startSquare, squareFromPosition, this);

                if (pendingMove.IsValid)
                {
                    m_OnMoveConfirmed?.Invoke();
                }
                else
                {
                    startSquare = ChessSquare.Zero;
                    m_OnMoveFailed?.Invoke();
                }

                SetSquareAppearance();
            }
        }

        public void UpdateGraveyardOffset(Transform graveyardRoot, float pieceGap, int rowIndex, int totalPieces, bool invertColumnOffset)
        {
            const int piecesPerRow = 8;
            int columnTotal = (totalPieces - 1) / piecesPerRow;
            int columnIndex = rowIndex / piecesPerRow;
            if (rowIndex >= piecesPerRow)
            {
                rowIndex -= piecesPerRow;
            }
            Vector3 totalOffset = graveyardRoot.rotation * Vector3.right * ((pieceGap / 2f) * columnTotal);
            Vector3 columnOffset = graveyardRoot.rotation * Vector3.right * (pieceGap * columnIndex);
            Vector3 newPos = graveyardRoot.position +
                             (graveyardRoot.rotation * Vector3.forward * (pieceGap * rowIndex)) +
                             (invertColumnOffset ? -columnOffset : columnOffset) +
                             (invertColumnOffset ? totalOffset : -totalOffset);

            m_LocalPositionTarget = m_ChessBoard.boardGenerator.transform.InverseTransformPoint(newPos);
            ResetVisualOffset();
        }

        public void SetOverlapped(bool isOverlapped)
        {
            if (m_IsOverlapped == isOverlapped)
                return;

            m_IsOverlapped = isOverlapped;
            m_VisualOffsetAttribute.target = isOverlapped ? Vector3.up * m_VerticalOverlapOffset : Vector3.zero;
        }

        /// <summary>
        /// Disable/Enable the renderers. Lets you hide pieces without affecting NetworkBehaviours.
        /// </summary>
        public void SetPieceVisibility(bool state)
        {
            m_RenderRoot.gameObject.SetActive(state);
        }

        void Update()
        {
            if (!m_ClaimableNetworkBehaviour.isOwnershipClaimed && !m_InteractionReceiver.isSelected && !m_ChessBoard.attachPiecesToSquares)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, m_LocalPositionTarget, Time.deltaTime * 5f);
            }

            m_VisualOffsetAttribute.HandleTween(Time.deltaTime * 8f);
            if (m_CurrentSquare == ChessSquare.BlackGraveyard ||
                m_CurrentSquare == ChessSquare.WhiteGraveyard)
            {
                m_ScaleAttribute.HandleTween(Time.deltaTime * 8f);
            }
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public void OnNetworkTick()
        {
            if (!m_InteractionReceiver.isSelected && !m_InteractionReceiver.isHovered && !m_ClaimableNetworkBehaviour.isOwnershipClaimed)
            {
                var stateIndex = isInteractable ? AffordanceStateShortcuts.idle : AffordanceStateShortcuts.disabled;

                var state = new AffordanceStateData(stateIndex, 1f);
                m_AffordanceStateProvider.UpdateAffordanceState(state);
                m_InteractionReceiver.enabled = isInteractable;
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete

        void SetSquareAppearance()
        {
            if (m_CurrentSquare == ChessSquare.Invalid ||
                m_CurrentSquare == ChessSquare.Zero)
            {
                if (m_ClaimableNetworkBehaviour.NetworkObject.IsSpawned)
                    gameObject.SetActive(false);
            }
            else if (m_CurrentSquare == ChessSquare.BlackGraveyard ||
                m_CurrentSquare == ChessSquare.WhiteGraveyard)
            {
                gameObject.SetActive(true);
                m_ScaleAffordanceReceiver.enabled = false;

                var variable = (BindableVariable<float3>)m_ScaleAffordanceReceiver.currentAffordanceValue;
                variable.Value = (new Vector3(.5f, .5f, .5f));
            }
            else
            {
                gameObject.SetActive(true);
                m_ScaleAffordanceReceiver.enabled = true;

                // Unsure why this was here - it results in disappearing pieces.
                // When the affordance receiver is activated it automatically grabs the state it should have anyway
                // m_ScaleAffordanceReceiver.OnAffordanceValueUpdate(Vector3.zero);
            }
        }
    }
}
