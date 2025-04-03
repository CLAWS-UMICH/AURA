using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Chess;

namespace UnityLabs.Slices.Games.Chess
{
    public class NetworkChessBoard : NetworkBehaviour
    {
        // Added as a workaround for network variables not synchronizing when joining a new network session
        protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
        {
            // This handles resetting the NetworkList (NetworkVariableBase) for the scenario
            // where you have a NetworkVariable or NetworkList on an in-scene placed NetworkObject
            // and you can start and stop a network session without reloading the scene. Invoking
            // the Initialize method assures that the last time sent update is reset.
            m_ReplicatedBoardState.Initialize(this);
            m_ReplicatedBoardTurn.Initialize(this);
            m_ReplicatedTimer.Initialize(this);
            base.OnNetworkPreSpawn(ref networkManager);
        }

        [SerializeField]
        ChessBoard m_ChessBoard = null;
        NetworkVariable<ChessBoardMessage> m_ReplicatedBoardState = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        NetworkVariable<ChessColor> m_ReplicatedBoardTurn = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        NetworkVariable<Vector2> m_ReplicatedTimer = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // can these be turned into ClientNetworkList? It needs to maintain a local list
        // for immediate updates
        NetworkList<ushort> m_ReplicatedMoveList;
        List<ushort> m_LocalMoveList;

        struct ChessBoardMessage : INetworkSerializable
        {
            public bool slideChessBoard;
            public bool showingOptions;
            public ChessGameMode gameMode;
            public bool timeControl;
            public int timeAmountIndex;
            public float gameRotation;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref slideChessBoard);
                serializer.SerializeValue(ref showingOptions);
                serializer.SerializeValue(ref gameMode);
                serializer.SerializeValue(ref timeControl);
                serializer.SerializeValue(ref timeAmountIndex);
                serializer.SerializeValue(ref gameRotation);
            }

            public bool Equals(ChessBoardMessage message)
            {
                return Mathf.Approximately(gameRotation, message.gameRotation) &&
                       timeAmountIndex == message.timeAmountIndex &&
                       timeControl == message.timeControl &&
                       gameMode == message.gameMode &&
                       showingOptions == message.showingOptions &&
                       slideChessBoard == message.slideChessBoard;
            }
        }

        void Awake()
        {
            m_ReplicatedMoveList = new NetworkList<ushort>();
            m_LocalMoveList = new List<ushort>();
        }

        public void ClaimOwnership()
        {
            ClaimOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        void ClaimOwnershipServerRpc(ulong id)
        {
            if (NetworkObject.OwnerClientId != id)
                NetworkObject.ChangeOwnership(id);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            // These seem to be in here because they may not necessarily be instantiated by Start on the client.
            // m_ReplicatedBoardState.Init(this);
            // m_ReplicatedBoardTurn.Init(this);
            // m_ReplicatedTimer.Init(this);

            NetworkManager.NetworkTickSystem.Tick += OnNetworkTick;
            m_ReplicatedBoardState.OnValueChanged += OnBoardStateValueChanged;
            m_ReplicatedBoardTurn.OnValueChanged += OnCurrentTurnValueChanged;
            m_ReplicatedTimer.OnValueChanged += OnTimerValueChanged;
            m_ReplicatedMoveList.OnListChanged += OnReplicatedMoveListChanged;

            m_ChessBoard.OnGameConnectChanged(true);

            //Replicate initial state for mid-game join
            OnBoardStateValueChanged(m_ReplicatedBoardState.Value, m_ReplicatedBoardState.Value);
            OnTimerValueChanged(m_ReplicatedTimer.Value, m_ReplicatedTimer.Value);

            // Moves must be replicated after game start is set
            for (int i = 0; i < m_ReplicatedMoveList.Count; ++i)
            {
                m_LocalMoveList.Add(m_ReplicatedMoveList[i]);
                Move move = new(m_ReplicatedMoveList[i]);
                m_ChessBoard.MoveListModelAdded(move, i);
            }

            OnCurrentTurnValueChanged(m_ReplicatedBoardTurn.Value, m_ReplicatedBoardTurn.Value);
        }

        public override void OnNetworkDespawn()
        {
            if (NetworkManager != null && NetworkManager.NetworkTickSystem != null)
                NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;

            // Technically these should not be necessary since its getting destroyed?
            // m_ReplicatedBoardState.ValueChanged -= OnBoardStateValueChanged;
            // m_ReplicatedBoardTurn.ValueChanged -= OnCurrentTurnValueChanged;
            // m_ReplicatedTimer.ValueChanged -= OnTimerValueChanged;
            // m_ReplicatedMoveList.OnListChanged -= OnReplicatedMoveListChanged;

            // Reset state of game may not be neccessary as we can just reload the whole game
            // m_ChessBoard.OnGameConnectChanged(false);
        }

        void OnReplicatedMoveListChanged(NetworkListEvent<ushort> changeEvent)
        {
            if (IsOwner) return;

            if (changeEvent.Type == NetworkListEvent<ushort>.EventType.Add)
            {
                m_LocalMoveList.Add(changeEvent.Value);
                Move move = new(changeEvent.Value);
                m_ChessBoard.MoveListModelAdded(move, changeEvent.Index);
            }
            else if (changeEvent.Type == NetworkListEvent<ushort>.EventType.Remove)
            {
                m_LocalMoveList.Remove(changeEvent.Value);
                Move move = new(changeEvent.Value);
                m_ChessBoard.MoveListModelRemoved(changeEvent.Index, move);
            }
            else if (changeEvent.Type == NetworkListEvent<ushort>.EventType.RemoveAt)
            {
                // RemoveAt doesn't send value from list, just sends index, so get value from local list.
                Move move = new(m_LocalMoveList[changeEvent.Index]);
                m_LocalMoveList.RemoveAt(changeEvent.Index);
                m_ChessBoard.MoveListModelRemoved(changeEvent.Index, move);
            }
            else if (changeEvent.Type == NetworkListEvent<ushort>.EventType.Full)
            {
                Debug.LogWarning("Full Sync on list! (don't know what causes this)");
                foreach (var moveValue in m_ReplicatedMoveList)
                {
                    m_LocalMoveList.Add(changeEvent.Value);
                    Move move = new(moveValue);
                    m_ChessBoard.MoveListModelAdded(move, changeEvent.Index);
                }
            }
            else if (changeEvent.Type == NetworkListEvent<ushort>.EventType.Clear)
            {
                // TODO this probably needed
                // foreach (var moveValue in m_ReplicatedMoveList)
                // {
                //     Move move = new(moveValue);
                //     m_ChessBoardController.MoveListModelAdded(move, changeEvent.Index);
                // }
            }
        }

        void OnTimerValueChanged(Vector2 previousValue, Vector2 newValue)
        {
            if (IsOwner) return;

            m_ChessBoard.UpdateMoveTimer(newValue);
        }

        void OnCurrentTurnValueChanged(ChessColor previousValue, ChessColor newValue)
        {
            if (IsOwner) return;

            m_ChessBoard.EndTurnRemote(newValue);
        }

        void OnBoardStateValueChanged(ChessBoardMessage previousValue, ChessBoardMessage newValue)
        {
            if (IsOwner) return;

            m_ChessBoard.boardRotation.Value = newValue.gameRotation;

            var currentOptionsState = new OptionState(m_ChessBoard.optionState)
            {
                slideBoard = newValue.slideChessBoard,
                timeControl = newValue.timeControl,
                timeAmountIndex = newValue.timeAmountIndex,
            };
            m_ChessBoard.UpdateOptionsState(currentOptionsState);

            m_ChessBoard.SetGameStartedRemote(newValue.gameMode);
            m_ChessBoard.SetShowingOptions(newValue.showingOptions);
        }

        /*TempCommentOut*/
        // void OnDidConnectToRoom(Realtime realtime)
        // {
        //     realtime.room.datastore.prefabViewModels.modelAdded += m_ChessBoardController.OnModelAdded;
        //     realtime.room.datastore.prefabViewModels.modelRemoved += m_ChessBoardController.OnModelRemoved;
        //     m_NetworkUpdate = StartCoroutine(NetworkUpdate((float)realtime.room.datastoreFrameDuration));
        //
        //     // It is possible that this subscription occurs after the modelAdded events are fired
        //     // if user is joining an already in session game, so iterate through spawned prefabs
        //     // and ensure they are setup.
        //     foreach (var viewModel in realtime.room.datastore.prefabViewModels)
        //     {
        //         m_ChessBoardController.OnModelAdded(null, viewModel, true);
        //     }
        //
        //     // Delay the actual connect call to spread load across more frames.
        //     m_DelayConnectCoroutine = StartCoroutine(DelayGameConnect());
        // }

        void OnNetworkTick()
        {
            m_ChessBoard.OnNetworkTick(); // Should this be gotten rid of? Can each Chesspiece just subscribe themselves?

            if (!IsOwner) return;

            if (m_ChessBoard.Moves.Count > m_LocalMoveList.Count)
            {
                for (int i = m_LocalMoveList.Count; i < m_ChessBoard.Moves.Count; ++i)
                {
                    m_LocalMoveList.Add(m_ChessBoard.Moves[i].Value);
                    CommitAddMoveServerRpc(m_ChessBoard.Moves[i].Value);
                }
                m_ReplicatedTimer.Value = m_ChessBoard.moveTimer;
            }
            else if (m_ChessBoard.Moves.Count < m_LocalMoveList.Count)
            {
                for (int i = m_LocalMoveList.Count - 1; i > m_ChessBoard.Moves.Count - 1; --i)
                {
                    m_LocalMoveList.RemoveAt(i);
                    CommitRemoveMoveServerRpc(i);
                }
                m_ReplicatedTimer.Value = m_ChessBoard.moveTimer;
            }

            ChessBoardMessage boardMessage = new()
            {
                gameMode = m_ChessBoard.GameMode.Value,
                gameRotation = m_ChessBoard.boardRotation.Value,
                showingOptions = m_ChessBoard.showingOptions.Value,
                slideChessBoard = m_ChessBoard.optionState.slideBoard,
                timeAmountIndex = m_ChessBoard.optionState.timeAmountIndex,
                timeControl = m_ChessBoard.optionState.timeControl
            };
            m_ReplicatedBoardState.Value = boardMessage;
            if (m_ReplicatedBoardState.IsDirty()) m_ReplicatedTimer.Value = m_ChessBoard.moveTimer;

            m_ReplicatedBoardTurn.Value = m_ChessBoard.currentTurn;
            if (m_ReplicatedBoardTurn.IsDirty()) m_ReplicatedTimer.Value = m_ChessBoard.moveTimer;
        }

        [ServerRpc(RequireOwnership = false)]
        void CommitAddMoveServerRpc(ushort move)
        {
            m_ReplicatedMoveList.Add(move);
        }

        [ServerRpc(RequireOwnership = false)]
        void CommitRemoveMoveServerRpc(int index)
        {
            m_ReplicatedMoveList.RemoveAt(index);
        }
    }
}
